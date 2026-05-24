import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { ACTION_URL } from '../../app.settings';
import { KpiCardData, RevenuePoint } from '../Models/models';

@Injectable({
  providedIn: 'root'
})
export class KPIService {

  constructor(private http: HttpClient) { }

  revenue = signal<RevenuePoint[]>([]);
  cardData = signal<KpiCardData | null>(null);

  loadRevenue(): void {
    const url = `${ACTION_URL}/Dashboard/KPI/getRevenue`;
    const headers = new HttpHeaders().set('Accept', 'application/json');
    this.http.get<RevenuePoint[]>(url, { headers }).subscribe(response => {
      this.revenue.set(response);
    });
  }

  loadCardData(): void {
    const url = `${ACTION_URL}/Dashboard/KPI/getCardData`;
    const headers = new HttpHeaders().set('Accept', 'application/json');
    this.http.get<KpiCardData>(url, { headers }).subscribe(response => {
      this.cardData.set(response);
    });
  }
}
