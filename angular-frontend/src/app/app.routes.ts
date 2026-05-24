import { Routes } from '@angular/router';
import { HomeComponent } from './Components/Home/home.component';
import { CustomersComponent } from './Components/Customers/CustomersList/customers.component';
import { InvoicesComponent } from './Components/Invoices/InvoicesList/invoices.component';
import { CustomersEditComponent } from './Components/Customers/CustomersEdit/customers-edit.component';
import { InvoicesEditComponent } from './Components/Invoices/InvoicesEdit/invoices-edit.component';

export const routes: Routes = [
    { path: '', component: HomeComponent },

    { path: 'customers', component: CustomersComponent },
    { path: 'customers/:id', component: CustomersEditComponent },

    { path: 'invoices', component: InvoicesComponent },
    { path: 'invoices/:id', component: InvoicesEditComponent },

];
