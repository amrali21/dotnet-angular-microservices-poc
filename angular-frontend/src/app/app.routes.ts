import { Routes } from '@angular/router';
import { HomeComponent } from './Components/Home/home.component';
import { CustomersComponent } from './Components/Customers/CustomersList/customers.component';
import { InvoicesComponent } from './Components/Invoices/InvoicesList/invoices.component';
import { CustomersEditComponent } from './Components/Customers/CustomersEdit/customers-edit.component';
import { InvoicesEditComponent } from './Components/Invoices/InvoicesEdit/invoices-edit.component';
import { InvoicesCreateComponent } from './Components/Invoices/InvoicesCreate/invoices-create.component';
import { LoginComponent } from './Components/Auth/Login/login.component';
import { RegisterComponent } from './Components/Auth/Register/register.component';
import { authGuard } from './Guards/auth.guard';

export const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },

    { path: '', component: HomeComponent, canActivate: [authGuard] },

    { path: 'customers', component: CustomersComponent, canActivate: [authGuard] },
    { path: 'customers/:id', component: CustomersEditComponent, canActivate: [authGuard] },

    { path: 'invoices', component: InvoicesComponent, canActivate: [authGuard] },
    { path: 'invoices/create', component: InvoicesCreateComponent, canActivate: [authGuard] },
    { path: 'invoices/:id', component: InvoicesEditComponent, canActivate: [authGuard] },

];
