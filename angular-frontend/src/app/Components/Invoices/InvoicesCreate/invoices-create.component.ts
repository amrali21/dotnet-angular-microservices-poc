import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InvoicesService } from '../../../Services/Invoices/invoices.service';
import { CustomersService } from '../../../Services/Customers/customers.service';
import { avatarColor, initials } from '../../../Shared/display';

@Component({
  selector: 'app-invoices-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, InputTextModule, DropdownModule],
  templateUrl: './invoices-create.component.html',
  styleUrl: './invoices-create.component.css'
})
export class InvoicesCreateComponent implements OnInit {

  private invoiceService = inject(InvoicesService);
  private customersService = inject(CustomersService);

  readonly statuses = [
    { label: 'Paid', value: 'paid', icon: 'pi-check' },
    { label: 'Pending', value: 'pending', icon: 'pi-hourglass' },
  ];

  readonly customers = this.customersService.allCustomers;
  readonly creating = this.invoiceService.creating;
  readonly error = this.invoiceService.error;

  get form(): FormGroup {
    return this.invoiceService.createInvoiceForm;
  }

  /** Only complain once the user has actually interacted with the field. */
  showError(control: string): boolean {
    const field = this.form.get(control);
    return !!field && field.invalid && (field.dirty || field.touched);
  }

  ngOnInit(): void {
    this.customersService.loadAllCustomers();
  }

  onSubmit(): void {
    this.invoiceService.createInvoice();
  }

  readonly initialsOf = initials;
  readonly colorFor = avatarColor;
}
