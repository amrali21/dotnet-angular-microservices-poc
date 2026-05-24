import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Customer, SearchFilter, PagedResult, CustomerFiler } from '../Models/models';
import { ACTION_URL } from '../../app.settings';

@Injectable({
  providedIn: 'root'
})
export class CustomersService {

  constructor(private http: HttpClient) { }

  customersList: Customer[] = [];
  length: number = 5;
  query: string = '';

  filter: CustomerFiler = { pageSize: 5, pageIndex: 0, query: '' };

  load(Filter: CustomerFiler, Callback: () => void): void {
    this.search(Filter).subscribe(response => {
      this.customersList = response.data;
      this.length = response.count;
      Callback();
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

  queryStringChangeEvent(params: any): any {
    console.log('query change event')
    let tempFilter = {
      query: params['query'] || '',
      pageSize: params['pageSize'] || 5,
      pageIndex: params['pageIndex'] || 0
    };

    this.load(tempFilter, () => {
      this.query = params['query'] || ''
      this.filter = tempFilter
    });
  }

  setQuery(query: string): void {
    this.query = query;
  }
}
