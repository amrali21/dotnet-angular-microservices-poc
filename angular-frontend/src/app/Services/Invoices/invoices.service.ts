import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { LatestInvoice, InvoiceFilter, PagedResult } from '../Models/models';
import { ACTION_URL } from '../../app.settings';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class InvoicesService {

  constructor(private http: HttpClient, private router: Router) {
  }

  invoicesList = signal<LatestInvoice[]>([]);

  length = signal<number>(5);

  /** Drives the table's loading / empty / error states. */
  loading = signal<boolean>(false);
  error = signal<string | null>(null);

  filter: InvoiceFilter = { pageSize: 5, pageIndex: 0, query: '' };

  /**
   * The edit form is created once and patched on load, so the template's
   * [formGroup] binding stays pointed at the same instance. `amount` is edited
   * in dollars and converted back to the cents the API stores.
   */
  invoiceForm: FormGroup = new FormGroup({
    id: new FormControl(''),
    name: new FormControl({ value: '', disabled: true }),
    email: new FormControl({ value: '', disabled: true }),
    amount: new FormControl<number | null>({ value: null, disabled: true }),
    status: new FormControl('pending', Validators.required),
  });

  /** The invoice currently open in the edit screen, for the summary panel. */
  currentInvoice = signal<LatestInvoice | null>(null);
  saving = signal<boolean>(false);

  /**
   * The create form is separate from invoiceForm: creating picks a customer and
   * an amount (fixed for the invoice's lifetime), editing only ever touches status.
   */
  createInvoiceForm: FormGroup = new FormGroup({
    customerId: new FormControl('', Validators.required),
    amount: new FormControl<number | null>(null, [Validators.required, Validators.min(0.01)]),
    status: new FormControl('pending', Validators.required),
  });

  creating = signal<boolean>(false);

  load(Filter: InvoiceFilter, Callback: () => void): void {
    this.loading.set(true);
    this.error.set(null);

    this.search(Filter).subscribe({
      next: response => {
        this.invoicesList.set(response.data ?? []);
        this.length.set(response.count ?? 0);
        this.loading.set(false);
        Callback();
      },
      error: () => {
        this.invoicesList.set([]);
        this.length.set(0);
        this.loading.set(false);
        this.error.set('Could not reach invoice-service through the API gateway.');
      },
    });
  }

  search(Filter: InvoiceFilter): Observable<PagedResult> {

    const url = `${ACTION_URL}/InvoiceGW/Invoice/fetchFilteredInvoices`;

    const params = {
      itemsPerPage: Filter.pageSize,
      offset: (Filter.pageIndex) * Filter.pageSize,
      query: Filter.query
    };

    const headers = new HttpHeaders().set('Accept', 'application/json');
    return this.http.get<PagedResult>(url, { params, headers });
  }

  searchById(Id: string | null): void {
    const url = `${ACTION_URL}/InvoiceGW/Invoice/fetchInvoiceById/${Id}`;

    this.loading.set(true);
    this.error.set(null);
    this.currentInvoice.set(null);

    const headers = new HttpHeaders().set('Accept', 'application/json');
    this.http.get<LatestInvoice>(url, { headers }).subscribe({
      next: response => {
        this.currentInvoice.set(response);
        this.invoiceForm.reset({
          id: response.id,
          name: response.name,
          email: response.email,
          amount: Number(response.amount) / 100,
          status: response.status,
        });
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Could not load this invoice from invoice-service.');
      },
    });
  }

  editInvoice(): void {
    if (this.invoiceForm.invalid) {
      this.invoiceForm.markAllAsTouched();
      return;
    }

    const url = `${ACTION_URL}/InvoiceGW/Invoice/updateInvoice`;
    const headers = new HttpHeaders().set('Accept', 'application/json');
    const form = this.invoiceForm.getRawValue();

    // Amount is fixed at creation — only status is editable here.
    const invoice = {
      id: form.id,
      status: form.status,
    };

    this.saving.set(true);
    this.error.set(null);

    this.http.put<LatestInvoice>(url, invoice, { headers }).subscribe({
      next: () => {
        this.saving.set(false);
        this.router.navigate(['invoices']);
      },
      error: () => {
        this.saving.set(false);
        this.error.set('Saving the invoice failed. Please try again.');
      },
    });
  }

  createInvoice(): void {
    if (this.createInvoiceForm.invalid) {
      this.createInvoiceForm.markAllAsTouched();
      return;
    }

    const url = `${ACTION_URL}/InvoiceGW/Invoice/insertInvoice`;
    const headers = new HttpHeaders().set('Accept', 'application/json');
    const form = this.createInvoiceForm.getRawValue();

    const invoice = {
      customerId: form.customerId,
      // dollars in the form -> cents the API stores
      amount: Math.round(Number(form.amount) * 100),
      status: form.status,
    };

    this.creating.set(true);
    this.error.set(null);

    this.http.post(url, invoice, { headers }).subscribe({
      next: () => {
        this.creating.set(false);
        this.createInvoiceForm.reset({ customerId: '', amount: null, status: 'pending' });
        this.router.navigate(['invoices']);
      },
      error: () => {
        this.creating.set(false);
        this.error.set('Creating the invoice failed. Please try again.');
      },
    });
  }

  /**
   * Confirmation lives in the component (it is a UI concern). After a delete we
   * reload the page the user is actually on rather than resetting the filter.
   */
  deleteInvoice(id: string): void {
    const url = `${ACTION_URL}/InvoiceGW/Invoice/deleteInvoice/${id}`;

    const headers = new HttpHeaders().set('Accept', 'application/json');
    this.http.delete<LatestInvoice>(url, { headers }).subscribe({
      next: () => this.load(this.filter, () => { }),
      error: () => this.error.set('Deleting the invoice failed. Please try again.'),
    });
  }

  queryStringChangeEvent(params: any): void {
    // Query params arrive as strings; the table needs real numbers.
    const tempFilter: InvoiceFilter = {
      query: params['query'] || '',
      pageSize: Number(params['pageSize']) || 5,
      pageIndex: Number(params['pageIndex']) || 0,
    };

    this.load(tempFilter, () => {
      this.filter = tempFilter;
    });
  }

  setQuery(query: string): void {
    this.filter.query = query;
  }
}
