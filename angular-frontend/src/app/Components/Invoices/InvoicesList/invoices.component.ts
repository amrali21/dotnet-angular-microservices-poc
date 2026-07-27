import { Component, DestroyRef, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ConfirmationService } from 'primeng/api';
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { TooltipModule } from 'primeng/tooltip';
import { InvoicesService } from '../../../Services/Invoices/invoices.service';
import { InvoiceFilter, LatestInvoice } from '../../../Services/Models/models';
import { avatarColor, formatCents, formatDate, initials } from '../../../Shared/display';

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TableModule, InputTextModule, TooltipModule, ConfirmPopupModule],
  providers: [ConfirmationService],
  templateUrl: './invoices.component.html',
  styleUrl: './invoices.component.css'
})
export class InvoicesComponent implements OnInit {

  private destroyRef = inject(DestroyRef);
  private confirmationService = inject(ConfirmationService);
  private invoiceService = inject(InvoicesService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  readonly loading = this.invoiceService.loading;
  readonly error = this.invoiceService.error;

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

  /** Deleting is destructive, so it asks first — anchored to the button clicked. */
  confirmDelete(event: Event, invoice: LatestInvoice): void {
    this.confirmationService.confirm({
      target: event.currentTarget as EventTarget,
      message: `Delete invoice #${invoice.id} for ${invoice.name}? This cannot be undone.`,
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Delete',
      rejectLabel: 'Cancel',
      acceptButtonStyleClass: 'p-button-danger p-button-sm',
      rejectButtonStyleClass: 'p-button-text p-button-sm',
      accept: () => this.invoiceService.deleteInvoice(invoice.id),
    });
  }

  onPage(event: any): void {
    const newFilter = { ...this.filter, pageSize: event.rows, pageIndex: event.first / event.rows };
    this.router.navigate([], {
      queryParams: { ...newFilter },
      queryParamsHandling: 'merge',
    });
  }

  search(): void {
    this.router.navigate([], {
      queryParams: { query: this.filter.query, pageIndex: 0 },
      queryParamsHandling: 'merge',
    });
  }

  clear(): void {
    this.query = '';
    this.search();
  }

  ngOnInit(): void {
    this.route.queryParams
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(params => this.invoiceService.queryStringChangeEvent(params));
  }

  // Presentation helpers, exposed for the template
  readonly money = formatCents;
  readonly date = formatDate;
  readonly initialsOf = initials;
  readonly colorFor = avatarColor;
}
