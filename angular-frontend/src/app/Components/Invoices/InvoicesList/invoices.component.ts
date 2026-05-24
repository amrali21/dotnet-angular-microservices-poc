import { Component, DestroyRef, inject } from '@angular/core';
import { InvoicesService } from '../../../Services/Invoices/invoices.service';
import { LatestInvoice, InvoiceFilter } from '../../../Services/Models/models';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink, RouterLinkActive } from '@angular/router';
import { PaginatorComponent } from '../../Common/Paginator/paginator.component';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { ReactiveFormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [TableModule, CommonModule, ReactiveFormsModule, InputTextModule, RippleModule, ButtonModule, PaginatorComponent, FormsModule, RouterLink, RouterLinkActive],
  templateUrl: './invoices.component.html',
  styleUrl: './invoices.component.css'
})

export class InvoicesComponent {
  // call api to get all invoices
  destroyRef = inject(DestroyRef);

  constructor(private invoiceService: InvoicesService, private router: Router, private route: ActivatedRoute) { }

  displayedColumns: string[] = ['id', 'amount', 'name', 'email', 'edit'];

  get query(): string {
    return this.invoiceService.filter.query;
  }

  set query(newQuery: string) {
    this.invoiceService.setQuery(newQuery);
  }

  get invoicesList(): LatestInvoice[] {
    return this.invoiceService.invoicesList();
  }

  get invoiceListLength(): number {
    return this.invoiceService.length();
  }

  get filter(): InvoiceFilter {
    return this.invoiceService.filter;
  }


  deleteInvoice(id: string): void {
    this.invoiceService.deleteInvoice(id);
  }

  onPage(event: any): void {
    let newFilter = { ...this.filter, pageSize: event.rows, pageIndex: (event.first / event.rows) }
    this.router.navigate([], {
      queryParams: { ...newFilter },
      queryParamsHandling: 'merge',
    });
  }

  async search(): Promise<any> {
    console.log('searching for: ', this.filter.query);
    this.router.navigate([], {
      queryParams: { query: this.filter.query, pageIndex: 0 },
      queryParamsHandling: 'merge',
    });
  }

  ngOnInit(): void {
    this.route.queryParams
    .pipe(takeUntilDestroyed(this.destroyRef)).subscribe(params => {

      this.invoiceService.queryStringChangeEvent(params);
    }
    )
  }
}