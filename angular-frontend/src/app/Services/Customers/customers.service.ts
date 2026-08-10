import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { Customer, PagedResult, CustomerFiler } from '../Models/models';
import { ACTION_URL } from '../../app.settings';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class CustomersService {

  constructor(private http: HttpClient, private router: Router) { }

  customersList = signal<Customer[]>([]);
  length = signal<number>(0);

  /** Unpaged list, used by the invoice-create customer picker. */
  allCustomers = signal<Customer[]>([]);

  /** Drives the table's loading / empty / error states. */
  loading = signal<boolean>(false);
  error = signal<string | null>(null);

  query: string = '';

  filter: CustomerFiler = { pageSize: 5, pageIndex: 0, query: '' };

  /** The customer currently open in the edit screen. */
  currentCustomer = signal<Customer | null>(null);
  saving = signal<boolean>(false);

  editCustomerForm: FormGroup = new FormGroup({
    id: new FormControl(''),
    name: new FormControl('', Validators.required),
    email: new FormControl('', [Validators.required, Validators.email]),
    image_url: new FormControl(''),
  });

  createCustomerForm: FormGroup = new FormGroup({
    name: new FormControl('', Validators.required),
    email: new FormControl('', [Validators.required, Validators.email]),
    image_url: new FormControl(''),
  });

  creating = signal<boolean>(false);

  load(Filter: CustomerFiler, Callback: () => void): void {
    this.loading.set(true);
    this.error.set(null);

    this.search(Filter).subscribe({
      next: response => {
        this.customersList.set(response.data ?? []);
        this.length.set(response.count ?? 0);
        this.loading.set(false);
        Callback();
      },
      error: () => {
        this.customersList.set([]);
        this.length.set(0);
        this.loading.set(false);
        this.error.set('Could not reach cust-service through the API gateway.');
      },
    });
  }

  search(Filter: CustomerFiler): Observable<PagedResult> {

    const url = `${ACTION_URL}/CustomerGW/Customer/fetchFilteredCustomers`;
    const params = {
      itemsPerPage: Filter.pageSize,
      offset: (Filter.pageIndex) * Filter.pageSize,
      query: Filter.query
    };

    const headers = new HttpHeaders().set('Accept', 'application/json');
    return this.http.get<PagedResult>(url, { params, headers });
  }

  loadAllCustomers(): void {
    const url = `${ACTION_URL}/CustomerGW/Customer/fetchCustomers`;
    const headers = new HttpHeaders().set('Accept', 'application/json');

    this.http.get<Customer[]>(url, { headers }).subscribe({
      next: list => this.allCustomers.set(list ?? []),
      error: () => this.allCustomers.set([]),
    });
  }

  searchById(id: string | null, callback?: (customer: Customer | null) => void): void {
    const url = `${ACTION_URL}/CustomerGW/Customer/fetchCustomerByID`;

    this.loading.set(true);
    this.error.set(null);
    this.currentCustomer.set(null);

    const headers = new HttpHeaders().set('Accept', 'application/json');
    this.http.get<Customer | null>(url, { params: { id: id ?? '' }, headers }).subscribe({
      next: response => {
        this.currentCustomer.set(response ?? null);
        if (response) {
          this.editCustomerForm.reset({
            id: response.id,
            name: response.name,
            email: response.email,
            image_url: response.image_url,
          });
        }
        this.loading.set(false);
        callback?.(response ?? null);
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Could not load this customer from cust-service.');
        callback?.(null);
      },
    });
  }

  editCustomer(): void {
    if (this.editCustomerForm.invalid) {
      this.editCustomerForm.markAllAsTouched();
      return;
    }

    const url = `${ACTION_URL}/CustomerGW/Customer/updateCustomer`;
    const headers = new HttpHeaders().set('Accept', 'application/json');
    const customer = this.editCustomerForm.getRawValue();

    this.saving.set(true);
    this.error.set(null);

    this.http.put(url, customer, { headers }).subscribe({
      next: () => {
        this.saving.set(false);
        this.currentCustomer.set(customer);
        this.router.navigate(['customers', customer.id]);
      },
      error: () => {
        this.saving.set(false);
        this.error.set('Saving the customer failed. Please try again.');
      },
    });
  }

  createCustomer(): void {
    if (this.createCustomerForm.invalid) {
      this.createCustomerForm.markAllAsTouched();
      return;
    }

    const url = `${ACTION_URL}/CustomerGW/Customer/insertCustomer`;
    const headers = new HttpHeaders().set('Accept', 'application/json');
    const customer = this.createCustomerForm.getRawValue();

    this.creating.set(true);
    this.error.set(null);

    this.http.post(url, customer, { headers }).subscribe({
      next: () => {
        this.creating.set(false);
        this.createCustomerForm.reset({ name: '', email: '', image_url: '' });
        this.router.navigate(['customers']);
      },
      error: () => {
        this.creating.set(false);
        this.error.set('Creating the customer failed. Please try again.');
      },
    });
  }

  queryStringChangeEvent(params: any): void {
    // Query params arrive as strings; the table needs real numbers.
    const tempFilter: CustomerFiler = {
      query: params['query'] || '',
      pageSize: Number(params['pageSize']) || 5,
      pageIndex: Number(params['pageIndex']) || 0,
    };

    this.load(tempFilter, () => {
      this.query = tempFilter.query;
      this.filter = tempFilter;
    });
  }

  setQuery(query: string): void {
    this.query = query;
  }
}
