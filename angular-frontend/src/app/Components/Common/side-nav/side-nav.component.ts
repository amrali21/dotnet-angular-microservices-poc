import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterModule, RouterOutlet } from '@angular/router';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-side-nav',
  standalone: true,
  templateUrl: './side-nav.component.html',
  styleUrl: './side-nav.component.css',
  imports: [RouterModule, ButtonModule, RouterModule, RouterOutlet, RouterLink, RouterLinkActive]
})
export class SideNavComponent {
  constructor(private router: Router) { }

  showFiller = false;

  route = (route: string) => {
    this.router.navigate([route]);
  }
}
