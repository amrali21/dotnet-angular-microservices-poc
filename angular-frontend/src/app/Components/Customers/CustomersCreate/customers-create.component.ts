import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { CustomersService } from '../../../Services/Customers/customers.service';

@Component({
  selector: 'app-customers-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, InputTextModule],
  templateUrl: './customers-create.component.html',
  styleUrl: './customers-create.component.css'
})
export class CustomersCreateComponent {

  private customersService = inject(CustomersService);

  readonly creating = this.customersService.creating;
  readonly error = this.customersService.error;

  get form(): FormGroup {
    return this.customersService.createCustomerForm;
  }

  /** Only complain once the user has actually interacted with the field. */
  showError(control: string): boolean {
    const field = this.form.get(control);
    return !!field && field.invalid && (field.dirty || field.touched);
  }

  onSubmit(): void {
    this.customersService.createCustomer();
  }
}
