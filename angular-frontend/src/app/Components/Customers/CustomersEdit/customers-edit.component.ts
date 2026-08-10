import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { CustomersService } from '../../../Services/Customers/customers.service';
import { InvoicesService } from '../../../Services/Invoices/invoices.service';
import { Customer, LatestInvoice } from '../../../Services/Models/models';
import { avatarColor, formatCents, formatDate, initials } from '../../../Shared/display';

@Component({
  selector: 'app-customers-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, InputTextModule],
  templateUrl: './customers-edit.component.html',
  styleUrl: './customers-edit.component.css'
})
export class CustomersEditComponent implements OnInit {

  private route = inject(ActivatedRoute);
  private customersService = inject(CustomersService);
  private invoicesService = inject(InvoicesService);

  ID: string | null = '';

  readonly customer = this.customersService.currentCustomer;
  readonly loading = this.customersService.loading;
  readonly saving = this.customersService.saving;
  readonly error = this.customersService.error;

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

  get form(): FormGroup {
    return this.customersService.editCustomerForm;
  }

  /** Only complain once the user has actually interacted with the field. */
  showError(control: string): boolean {
    const field = this.form.get(control);
    return !!field && field.invalid && (field.dirty || field.touched);
  }

  ngOnInit(): void {
    this.ID = this.route.snapshot.paramMap.get('id');

    this.customersService.searchById(this.ID, customer => {
      if (customer?.email) {
        this.loadInvoices(customer.email);
      }
    });
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

  onSubmit(): void {
    this.customersService.editCustomer();
  }

  readonly money = formatCents;
  readonly date = formatDate;
  readonly initialsOf = initials;
  readonly colorFor = avatarColor;
}
