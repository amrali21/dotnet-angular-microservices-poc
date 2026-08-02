import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, map, startWith } from 'rxjs';
import { TooltipModule } from 'primeng/tooltip';
import { SideNavComponent } from './Components/Common/side-nav/side-nav.component';
import { ThemeService } from './Services/Theme/theme.service';
import { AuthService } from './Services/Auth/auth.service';
import { avatarColor, initials } from './Shared/display';

const AUTH_ROUTES = ['login', 'register'];

const SECTION_TITLES: Record<string, string> = {
  '': 'Overview',
  invoices: 'Invoices',
  customers: 'Customers',
};

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, SideNavComponent, TooltipModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {

  readonly theme = inject(ThemeService);
  readonly auth = inject(AuthService);
  private router = inject(Router);

  /** Drawer state — only meaningful below the tablet breakpoint. */
  readonly navOpen = signal(false);

  private url = toSignal(
    this.router.events.pipe(
      filter((event): event is NavigationEnd => event instanceof NavigationEnd),
      map(event => event.urlAfterRedirects),
      startWith(this.router.url),
    ),
    { initialValue: this.router.url },
  );

  private segment = computed(() => this.url().split('?')[0].split('/').filter(Boolean)[0] ?? '');

  readonly sectionTitle = computed(() => SECTION_TITLES[this.segment()] ?? 'Overview');

  /** Login/register are full-page views — the dashboard shell has nothing to show yet. */
  readonly isAuthRoute = computed(() => AUTH_ROUTES.includes(this.segment()));

  readonly initialsOf = initials;
  readonly colorFor = avatarColor;

  toggleNav(): void {
    this.navOpen.update(open => !open);
  }

  closeNav(): void {
    this.navOpen.set(false);
  }

  logout(): void {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }
}
