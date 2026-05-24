import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-customers-edit',
  standalone: true,
  imports: [],
  templateUrl: './customers-edit.component.html',
  styleUrl: './customers-edit.component.css'
})
export class CustomersEditComponent {
  constructor(private route: ActivatedRoute) { }

  ID: string | null = '';

  ngOnInit(): void {
    this.ID = this.route.snapshot.paramMap.get('id');
  }
}
