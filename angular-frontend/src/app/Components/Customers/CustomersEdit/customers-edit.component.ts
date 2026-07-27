import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CustomersService } from '../../../Services/Customers/customers.service';
import { InvoicesService } from '../../../Services/Invoices/invoices.service';
import { Customer, LatestInvoice } from '../../../Services/Models/models';
import { avatarColor, formatCents, formatDate, initials } from '../../../Shared/display';

@Component({
  selector: 'app-customers-edit',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './customers-edit.component.html',
  styleUrl: './customers-edit.component.css'
})
export class CustomersEditComponent implements OnInit {

  private route = inject(ActivatedRoute);
  private customersService = inject(CustomersService);
  private invoicesService = inject(InvoicesService);

  ID: string | null = '';

  readonly customer = signal<Customer | null>(null);
  readonly invoices = signal<LatestInvoice[]>([]);
  readonly loadingInvoices = signal(false);

  readonly billedCents = computed(() =>
    this.invoices().reduce((sum, invoice) => sum + (Number(invoice.amount) || 0), 0)
  );

  readonly paidCents = computed(() =>
    this.invoices()
      .filter(invoice => invoice.status === 'paid')
      .reduce((sum, invoice) => sum + (Number(invoice.amount) || 0), 0)
  );

  readonly outstandingCents = computed(() => this.billedCents() - this.paidCents());

  ngOnInit(): void {
    this.ID = this.route.snapshot.paramMap.get('id');

    // cust-service exposes no fetch-by-id endpoint, so the record comes from the
    // page the user navigated from. Opening this URL directly has nothing to read.
    const found = this.customersService.findLoaded(this.ID);
    this.customer.set(found);

    if (found?.email) {
      this.loadInvoices(found.email);
    }
  }

  /** invoice-service matches its query against customer name and email. */
  private loadInvoices(email: string): void {
    this.loadingInvoices.set(true);
    this.invoicesService.search({ query: email, pageSize: 50, pageIndex: 0 }).subscribe({
      next: response => {
        this.invoices.set(response.data ?? []);
        this.loadingInvoices.set(false);
      },
      error: () => this.loadingInvoices.set(false),
    });
  }

  readonly money = formatCents;
  readonly date = formatDate;
  readonly initialsOf = initials;
  readonly colorFor = avatarColor;
}
