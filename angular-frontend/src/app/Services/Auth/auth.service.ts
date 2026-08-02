import { HttpClient } from '@angular/common/http';
import { Injectable, computed, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { ACTION_URL } from '../../app.settings';
import { AuthResponse, AuthUser, LoginRequest, RegisterRequest } from '../Models/models';

const TOKEN_KEY = 'ledgerly.auth_token';

type TokenPayload = {
  sub: string;
  email: string;
  name: string;
  exp: number;
};

/**
 * Talks to auth-service (through the gateway's /AuthGW route) and owns the
 * signed-in state for the whole app. The JWT itself lives in localStorage so a
 * page refresh doesn't lose the session — see CLAUDE.md/plan notes for why
 * that was chosen over an httpOnly cookie in this multi-origin setup.
 */
@Injectable({ providedIn: 'root' })
export class AuthService {

  constructor(private http: HttpClient) {
    this.currentUser.set(this.readUserFromStoredToken());
  }

  readonly currentUser = signal<AuthUser | null>(null);
  readonly isAuthenticated = computed(() => this.currentUser() !== null);

  register(request: RegisterRequest): Observable<AuthResponse> {
    const url = `${ACTION_URL}/AuthGW/Auth/Register`;
    return this.http.post<AuthResponse>(url, request).pipe(tap(response => this.setSession(response)));
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    const url = `${ACTION_URL}/AuthGW/Auth/Login`;
    return this.http.post<AuthResponse>(url, request).pipe(tap(response => this.setSession(response)));
  }

  logout(): void {
    this.clearSession();
  }

  getToken(): string | null {
    try {
      return localStorage.getItem(TOKEN_KEY);
    } catch {
      return null;
    }
  }

  private setSession(response: AuthResponse): void {
    try {
      localStorage.setItem(TOKEN_KEY, response.token);
    } catch {
      // storage can be blocked (private mode); the session still applies for this tab
    }
    this.currentUser.set(response.user);
  }

  private clearSession(): void {
    try {
      localStorage.removeItem(TOKEN_KEY);
    } catch {
      // ignore — nothing to clean up if storage isn't available
    }
    this.currentUser.set(null);
  }

  private readUserFromStoredToken(): AuthUser | null {
    const token = this.getToken();
    if (!token) return null;

    const payload = this.decodeToken(token);
    if (!payload || payload.exp * 1000 <= Date.now()) {
      this.clearSession();
      return null;
    }

    return { id: payload.sub, email: payload.email, displayName: payload.name };
  }

  private decodeToken(token: string): TokenPayload | null {
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const json = decodeURIComponent(
        atob(base64)
          .split('')
          .map(char => '%' + char.charCodeAt(0).toString(16).padStart(2, '0'))
          .join('')
      );
      return JSON.parse(json);
    } catch {
      return null;
    }
  }
}
