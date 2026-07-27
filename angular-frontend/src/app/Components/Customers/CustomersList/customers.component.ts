import { Component, DestroyRef, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { TooltipModule } from 'primeng/tooltip';
import { CustomersService } from '../../../Services/Customers/customers.service';
import { Customer, CustomerFiler } from '../../../Services/Models/models';
import { avatarColor, initials } from '../../../Shared/display';

@Component({
  selector: 'app-customers',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TableModule, InputTextModule, TooltipModule],
  templateUrl: './customers.component.html',
  styleUrl: './customers.component.css'
})
export class CustomersComponent implements OnInit {

  private destroyRef = inject(DestroyRef);
  private customerService = inject(CustomersService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  readonly loading = this.customerService.loading;
  readonly error = this.customerService.error;

  get query(): string {
    return this.customerService.query;
  }

  set query(newQuery: string) {
    this.customerService.setQuery(newQuery);
  }

  get customerList(): Customer[] {
    return this.customerService.customersList();
  }

  get customerListLength(): number {
    return this.customerService.length();
  }

  get filter(): CustomerFiler {
    return this.customerService.filter;
  }

  onPage(event: any): void {
    const newFilter = { ...this.filter, pageSize: event.rows, pageIndex: event.first / event.rows };
    this.router.navigate([], {
      queryParams: { ...newFilter },
      queryParamsHandling: 'merge',
    });
  }

  search(): void {
    this.router.navigate([], {
      queryParams: { query: this.query, pageIndex: 0 },
      queryParamsHandling: 'merge',
    });
  }

  clear(): void {
    this.query = '';
    this.search();
  }

  ngOnInit(): void {
    this.route.queryParams
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(params => this.customerService.queryStringChangeEvent(params));
  }

  /** GUIDs are too long for a table cell; the leading block is enough to scan. */
  shortId(id: string): string {
    return id ? `${id.slice(0, 8)}…` : '—';
  }

  readonly initialsOf = initials;
  readonly colorFor = avatarColor;
}
