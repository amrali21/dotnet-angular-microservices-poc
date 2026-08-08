import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { Customer, PagedResult, CustomerFiler } from '../Models/models';
import { ACTION_URL } from '../../app.settings';

@Injectable({
  providedIn: 'root'
})
export class CustomersService {

  constructor(private http: HttpClient) { }

  customersList = signal<Customer[]>([]);
  length = signal<number>(0);

  /** Unpaged list, used by the invoice-create customer picker. */
  allCustomers = signal<Customer[]>([]);

  /** Drives the table's loading / empty / error states. */
  loading = signal<boolean>(false);
  error = signal<string | null>(null);

  query: string = '';

  filter: CustomerFiler = { pageSize: 5, pageIndex: 0, query: '' };

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

  /**
   * There is no fetch-by-id endpoint on cust-service, so the detail screen
   * looks the customer up in the page that is already loaded. Returns null when
   * the page was opened directly by URL.
   */
  findLoaded(id: string | null): Customer | null {
    if (!id) return null;
    return this.customersList().find(customer => customer.id === id) ?? null;
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
