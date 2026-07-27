import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, map, startWith } from 'rxjs';
import { TooltipModule } from 'primeng/tooltip';
import { SideNavComponent } from './Components/Common/side-nav/side-nav.component';
import { ThemeService } from './Services/Theme/theme.service';

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

  readonly sectionTitle = computed(() => {
    const segment = this.url().split('?')[0].split('/').filter(Boolean)[0] ?? '';
    return SECTION_TITLES[segment] ?? 'Overview';
  });

  toggleNav(): void {
    this.navOpen.update(open => !open);
  }

  closeNav(): void {
    this.navOpen.set(false);
  }
}
