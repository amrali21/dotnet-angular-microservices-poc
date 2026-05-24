import { Component, Input, OnInit } from '@angular/core';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { ActivatedRoute, Router, RouterLink, RouterLinkActive } from '@angular/router';
import { SearchFilter } from '../../../Services/Models/models';

@Component({
  selector: 'app-paginator',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, MatPaginatorModule, RouterLink],
  templateUrl: './paginator.component.html',
  styleUrl: './paginator.component.css'
})
export class PaginatorComponent implements OnInit {

  constructor(private route: ActivatedRoute, private router: Router) {

  }
  filter: SearchFilter = { pageSize: 5, pageIndex: 0 };

  @Input() length = 5;

  ngOnInit(): void {

    this.route.queryParams.subscribe(params => {

      this.filter = {
        pageSize: params['pageSize'] || 5,
        pageIndex: params['pageIndex'] || 0
      };
    })
  }

  handlePageEvent(e: PageEvent) {

    this.filter = { ...this.filter, ...e }
    this.router.navigate([], {
      queryParams: { ...this.filter },
      queryParamsHandling: 'merge',
    });
  }

}
