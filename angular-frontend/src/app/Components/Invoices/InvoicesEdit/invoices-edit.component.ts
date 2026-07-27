import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InvoicesService } from '../../../Services/Invoices/invoices.service';
import { avatarColor, formatCents, formatDate, initials } from '../../../Shared/display';

@Component({
  selector: 'app-invoices-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, InputTextModule, DropdownModule],
  templateUrl: './invoices-edit.component.html',
  styleUrl: './invoices-edit.component.css'
})
export class InvoicesEditComponent implements OnInit {

  private route = inject(ActivatedRoute);
  private invoiceService = inject(InvoicesService);

  ID: string | null = '';

  readonly statuses = [
    { label: 'Paid', value: 'paid', icon: 'pi-check' },
    { label: 'Pending', value: 'pending', icon: 'pi-hourglass' },
  ];

  readonly invoice = this.invoiceService.currentInvoice;
  readonly loading = this.invoiceService.loading;
  readonly saving = this.invoiceService.saving;
  readonly error = this.invoiceService.error;

  get invoiceForm(): FormGroup {
    return this.invoiceService.invoiceForm;
  }

  /** Only complain once the user has actually interacted with the field. */
  showError(control: string): boolean {
    const field = this.invoiceForm.get(control);
    return !!field && field.invalid && (field.dirty || field.touched);
  }

  ngOnInit(): void {
    this.ID = this.route.snapshot.paramMap.get('id');
    this.invoiceService.searchById(this.ID);
  }

  onSubmit(): void {
    this.invoiceService.editInvoice();
  }

  readonly money = formatCents;
  readonly date = formatDate;
  readonly initialsOf = initials;
  readonly colorFor = avatarColor;
}
