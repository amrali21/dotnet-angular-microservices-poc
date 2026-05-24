import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { SearchFilter, LatestInvoice, InvoiceFilter, PagedResult, Customer } from '../Models/models';
import { ACTION_URL } from '../../app.settings';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, FormGroup } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class InvoicesService {

  constructor(private http: HttpClient, private router: Router, private route: ActivatedRoute) {
  }

  invoicesList = signal<LatestInvoice[]>([]);

  length = signal<number>(5);

  filter: InvoiceFilter = { pageSize: 5, pageIndex: 0, query: '' };

  invoiceForm: FormGroup = new FormGroup({
    name: new FormControl(''),
    amount: new FormControl(''),
    status: new FormControl('')
  });

  load(Filter: InvoiceFilter, Callback: () => void): void {
    this.search(Filter).subscribe(response => {
      this.invoicesList.set(response.data);
      this.length.set(response.count);
      Callback();
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

    const headers = new HttpHeaders().set('Accept', 'application/json');
    this.http.get<LatestInvoice>(url, { headers }).subscribe(response => {

      this.invoiceForm = new FormGroup({
        name: new FormControl(response.name),
        amount: new FormControl(response.amount),
        status: new FormControl({ name: response.status, code: response.status }),//new FormControl(response.status)
        id: new FormControl(response.id),
        customerId: new FormControl('')
      });

    });
  }

  editInvoice(): void {
    const url = `${ACTION_URL}/InvoiceGW/Invoice/updateInvoice`;
    const headers = new HttpHeaders().set('Accept', 'application/json');

    let invoice: any = { id: this.invoiceForm.value.id, amount: this.invoiceForm.value.amount, status: this.invoiceForm.value.status?.code, customerId: '' };

    this.http.put<LatestInvoice>(url, invoice, { headers }).subscribe(response => {
      this.router.navigate(['invoices'], /*{ relativeTo:  }*/);

    });
  }

  deleteInvoice(id: string): void {
    if (!confirm("Are you sure you want to delete this invoice?")) {
      return;
    }

    console.log('proceeded to delete')
    const url = `${ACTION_URL}/InvoiceGW/Invoice/deleteInvoice/${id}`;

    const headers = new HttpHeaders().set('Accept', 'application/json');
    this.http.delete<LatestInvoice>(url, { headers }).subscribe(response => {

      this.load({ pageSize: 5, pageIndex: 0, query: '' },
        () => { }
      );

    });
  }

  queryStringChangeEvent(params: any): any {
    console.log('query change event')
    let tempFilter = {
      query: params['query'] || '',
      pageSize: params['pageSize'] || 5,
      pageIndex: params['pageIndex'] || 0
    };

    this.load(tempFilter, () => {
      //this.query = params['query'] || ''
      this.filter = tempFilter
    });
  }

  setQuery(query: string): void {
    this.filter.query = query;
  }
}
