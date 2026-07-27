import { DOCUMENT } from '@angular/common';
import { Injectable, computed, inject, signal } from '@angular/core';

export type ThemeMode = 'light' | 'dark';

const STORAGE_KEY = 'ledgerly.theme';

/**
 * Owns the light/dark switch. Two things move together:
 *   1. `data-theme` on <html>, which drives our own design tokens in styles.css
 *   2. the href of #primeng-theme, which swaps the PrimeNG Aura stylesheet
 *
 * The initial value is applied by the inline bootstrap in index.html so there is
 * no flash of the wrong theme; this service picks up from there.
 */
@Injectable({ providedIn: 'root' })
export class ThemeService {

  private document = inject(DOCUMENT);

  readonly mode = signal<ThemeMode>(this.readInitialMode());
  readonly isDark = computed(() => this.mode() === 'dark');

  toggle(): void {
    this.set(this.mode() === 'dark' ? 'light' : 'dark');
  }

  set(mode: ThemeMode): void {
    this.mode.set(mode);
    this.document.documentElement.setAttribute('data-theme', mode);

    const link = this.document.getElementById('primeng-theme') as HTMLLinkElement | null;
    if (link) {
      link.href = `themes/aura-${mode}-indigo/theme.css`;
    }

    try {
      localStorage.setItem(STORAGE_KEY, mode);
    } catch {
      // storage can be blocked (private mode); the theme still applies for this session
    }
  }

  private readInitialMode(): ThemeMode {
    const applied = this.document.documentElement.getAttribute('data-theme');
    return applied === 'dark' ? 'dark' : 'light';
  }
}
