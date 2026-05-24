import { Component, OnInit, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { KPIService } from '../../Services/KPI/kpi.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, TableModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {

  private kpiService = inject(KPIService);

  revenue = this.kpiService.revenue;
  cardData = this.kpiService.cardData;

  totalRevenue = computed(() =>
    this.revenue().reduce((sum, r) => sum + (r.revenue ?? 0), 0)
  );

  ngOnInit(): void {
    this.kpiService.loadRevenue();
    this.kpiService.loadCardData();
  }

  formatCurrency(value: number | undefined | null): string {
    if (value === undefined || value === null) return '$0';
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', maximumFractionDigits: 0 }).format(value);
  }
}
