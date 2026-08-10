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
  /** True for placeholder items that aren't built yet; they don't navigate anywhere. */
  todo?: boolean;
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
    { label: 'Payments', route: '', icon: 'pi-credit-card', service: 'payments', exact: false, todo: true },
    { label: 'Reports', route: '', icon: 'pi-chart-line', service: 'reports', exact: false, todo: true },
    { label: 'Settings', route: '', icon: 'pi-cog', service: 'settings', exact: false, todo: true },
  ];

  readonly services = ['api-gateway', 'invoice-service', 'cust-service', 'dashboard-service', 'auth-service'];
}
