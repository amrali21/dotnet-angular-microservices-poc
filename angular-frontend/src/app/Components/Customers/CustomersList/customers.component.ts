import { Component, OnInit } from '@angular/core';
import { CustomersService } from '../../../Services/Customers/customers.service';
import { CustomerFiler } from '../../../Services/Models/models';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink, RouterLinkActive } from '@angular/router';
import { PaginatorComponent } from '../../Common/Paginator/paginator.component';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-customers',
  standalone: true,
  imports: [TableModule, CommonModule, InputTextModule, RippleModule, ButtonModule, PaginatorComponent, FormsModule, RouterLink, RouterLinkActive],
  templateUrl: './customers.component.html',
  styleUrl: './customers.component.css'
})
export class CustomersComponent implements OnInit {

  constructor(private customerService: CustomersService, private router: Router, private route: ActivatedRoute) { }

  displayedColumns: string[] = ['name', 'email', 'image_url'];

  get query(): string {
    return this.customerService.query;
  }

  set query(newQuery: string) {
    this.customerService.setQuery(newQuery);
  }

  get customerList(): any[] {
    return this.customerService.customersList;
  }

  get customerListLength(): number {
    return this.customerService.length;
  }

  get filter(): CustomerFiler {
    return this.customerService.filter;
  }
  // searchFilter: SearchFilter = {pageSize: null, pageIndex: null}

  onPage(event: any): void {
    let newFilter = { ...this.filter, pageSize: event.rows, pageIndex: (event.first / event.rows) }
    this.router.navigate([], {
      queryParams: { ...newFilter },
      queryParamsHandling: 'merge',
    });
  }

  async search(): Promise<any> {
    console.log(this.query);
    console.log('search event')
    this.router.navigate([], {
      queryParams: { query: this.query, pageIndex: 0 },
      queryParamsHandling: 'merge',
    });
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      // before setting the filter, i want to make sure total records are correct. 
      // 1. we need to query the api, wait for the response
      // 2. then after the response is back and we know total, we change filter 
      this.customerService.queryStringChangeEvent(params);
    }
    )
  }
}
