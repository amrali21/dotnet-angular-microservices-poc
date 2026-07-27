import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { catchError, finalize, forkJoin, of, tap } from 'rxjs';
import { ACTION_URL } from '../../app.settings';
import { KpiCardData, RevenuePoint } from '../Models/models';

@Injectable({
  providedIn: 'root'
})
export class KPIService {

  constructor(private http: HttpClient) { }

  revenue = signal<RevenuePoint[]>([]);
  cardData = signal<KpiCardData | null>(null);

  /** Both dashboard calls load together so the page has one honest state. */
  loading = signal<boolean>(false);
  error = signal<string | null>(null);

  load(): void {
    const headers = new HttpHeaders().set('Accept', 'application/json');

    this.loading.set(true);
    this.error.set(null);

    forkJoin({
      revenue: this.http.get<RevenuePoint[]>(`${ACTION_URL}/Dashboard/KPI/getRevenue`, { headers }),
      cardData: this.http.get<KpiCardData>(`${ACTION_URL}/Dashboard/KPI/getCardData`, { headers }),
    }).pipe(
      tap(({ revenue, cardData }) => {
        this.revenue.set(revenue ?? []);
        this.cardData.set(cardData);
      }),
      catchError(() => {
        this.error.set('Could not reach dashboard-service through the API gateway.');
        return of(null);
      }),
      finalize(() => this.loading.set(false)),
    ).subscribe();
  }
}
