import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';

type NavItem = {
  label: string;
  route: string;
  icon: string;
  /** The microservice that actually answers this screen's requests. */
  service: string;
  exact: boolean;
};

@Component({
  selector: 'app-side-nav',
  standalone: true,
  templateUrl: './side-nav.component.html',
  styleUrl: './side-nav.component.css',
  imports: [CommonModule, RouterLink, RouterLinkActive],
})
export class SideNavComponent {

  /** Lets the shell close the mobile drawer once a destination is picked. */
  @Output() navigated = new EventEmitter<void>();

  readonly version = '1.0.0';

  readonly items: NavItem[] = [
    { label: 'Overview', route: '/', icon: 'pi-chart-bar', service: 'dashboard', exact: true },
    { label: 'Invoices', route: '/invoices', icon: 'pi-file', service: 'invoices', exact: false },
    { label: 'Customers', route: '/customers', icon: 'pi-users', service: 'customers', exact: false },
  ];

  readonly services = ['api-gateway', 'invoice-service', 'cust-service', 'dashboard-service', 'auth-service'];
}
