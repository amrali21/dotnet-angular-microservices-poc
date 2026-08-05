import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { KPIService } from '../../Services/KPI/kpi.service';
import { byCalendarMonth, formatCents, formatDollars } from '../../Shared/display';

/** One bar's geometry, precomputed so the template stays declarative. */
type Bar = {
  month: string;
  revenue: number;
  x: number;
  y: number;
  width: number;
  height: number;
  centre: number;
  isMax: boolean;
  /** Only the data-end is rounded; the bar stays square on the baseline. */
  path: string;
};

// SVG user units. The chart scales with its container; the viewBox is fixed.
const VIEW_W = 760;
const VIEW_H = 240;
const PAD = { top: 18, right: 12, bottom: 28, left: 54 };
const PLOT_W = VIEW_W - PAD.left - PAD.right;
const PLOT_H = VIEW_H - PAD.top - PAD.bottom;
const BAR_GAP = 16;

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {

  private kpiService = inject(KPIService);

  readonly cardData = this.kpiService.cardData;
  readonly loading = this.kpiService.loading;
  readonly error = this.kpiService.error;

  readonly hovered = signal<number | null>(null);

  /** The chart always ships an equivalent table view (accessibility fallback). */
  readonly tableView = signal(false);

  readonly view = {
    w: VIEW_W,
    h: VIEW_H,
    left: PAD.left,
    right: VIEW_W - PAD.right,
    baseline: PAD.top + PLOT_H,
    labelY: PAD.top + PLOT_H + 18,
  };

  /** The DB returns months in its own order; a chart needs calendar order. */
  readonly revenue = computed(() => byCalendarMonth(this.kpiService.revenue()));

  readonly totalRevenue = computed(() =>
    this.revenue().reduce((sum, r) => sum + (Number(r.revenue) || 0), 0)
  );

  readonly paidCents = computed(() => Number(this.cardData()?.collected) || 0);
  readonly pendingCents = computed(() => Number(this.cardData()?.outstanding) || 0);
  readonly billedCents = computed(() => Number(this.cardData()?.totalBilled) || 0);

  /** Share of billed value already collected — drives the meter and its label. */
  readonly collectedShare = computed(() => {
    const billed = this.billedCents();
    return billed === 0 ? 0 : Math.round((this.paidCents() / billed) * 100);
  });

  /** Axis top, rounded up to a readable step so tick labels stay whole. */
  readonly scaleMax = computed(() => {
    const peak = Math.max(0, ...this.revenue().map(r => Number(r.revenue) || 0));
    if (peak <= 0) return 1000;
    const step = Math.pow(10, Math.floor(Math.log10(peak))) / 2;
    return Math.ceil(peak / step) * step;
  });

  readonly ticks = computed(() => {
    const max = this.scaleMax();
    return [1, 0.75, 0.5, 0.25, 0].map(fraction => ({
      value: max * fraction,
      y: PAD.top + PLOT_H - PLOT_H * fraction,
    }));
  });

  readonly bars = computed<Bar[]>(() => {
    const rows = this.revenue();
    if (rows.length === 0) return [];

    const max = this.scaleMax();
    const slot = PLOT_W / rows.length;
    const width = Math.max(6, slot - BAR_GAP);
    const peak = Math.max(...rows.map(r => Number(r.revenue) || 0));

    return rows.map((row, index) => {
      const value = Number(row.revenue) || 0;
      const height = max === 0 ? 0 : (value / max) * PLOT_H;
      const x = PAD.left + slot * index + (slot - width) / 2;
      const y = PAD.top + PLOT_H - height;
      const r = Math.min(4, height, width / 2);
      const base = PAD.top + PLOT_H;

      return {
        month: row.month,
        revenue: value,
        x,
        y,
        width,
        height,
        centre: x + width / 2,
        isMax: value === peak,
        path: `M${x} ${base} V${y + r} A${r} ${r} 0 0 1 ${x + r} ${y}`
          + ` H${x + width - r} A${r} ${r} 0 0 1 ${x + width} ${y + r}`
          + ` V${base} Z`,
      };
    });
  });

  readonly hoveredBar = computed(() => {
    const index = this.hovered();
    return index === null ? null : this.bars()[index] ?? null;
  });

  /** Tooltip position as a percentage of the chart box, so it tracks on resize. */
  readonly tooltipStyle = computed(() => {
    const bar = this.hoveredBar();
    if (!bar) return {};
    return {
      left: `${(bar.centre / VIEW_W) * 100}%`,
      top: `${(bar.y / VIEW_H) * 100}%`,
    };
  });

  ngOnInit(): void {
    this.kpiService.load();
  }

  refresh(): void {
    this.kpiService.load();
  }

  readonly money = formatCents;
  readonly dollars = formatDollars;
}
