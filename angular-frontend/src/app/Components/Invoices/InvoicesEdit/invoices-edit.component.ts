import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { InvoicesService } from '../../../Services/Invoices/invoices.service';
import { FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';

@Component({
  selector: 'app-invoices-edit',
  standalone: true,
  imports: [DropdownModule, InputTextModule, RippleModule, ButtonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './invoices-edit.component.html',
  styleUrl: './invoices-edit.component.css'
})
export class InvoicesEditComponent implements OnInit {

  constructor(private route: ActivatedRoute, private invoiceService: InvoicesService) { }

  ID: string | null = '';
  Status: any = [{ name: 'paid', code: 'paid' }, { name: 'pending', code: 'pending' }];
  Test: any = { name: 'paid', code: 'paid' };


  get invoiceForm(): FormGroup {
    return this.invoiceService.invoiceForm;
  }

  ngOnInit(): void {
    this.ID = this.route.snapshot.paramMap.get('id');
    this.invoiceService.searchById(this.ID);
  }

  // i need make a link between currentInvoice model and the dropdown
  // i can't directly link them because dropdown accepts an object not a property.
  // - workarounds?
  //   + i make a separate object and bind the dropdown to it.
  //   + then take values from that object into the service

  onSubmit(form: FormGroup) {
    this.invoiceService.editInvoice();
  }
}
