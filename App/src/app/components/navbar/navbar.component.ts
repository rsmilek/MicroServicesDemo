import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { User } from '../../models';
import { Observable } from 'rxjs';
import { role } from 'src/app/utilities/role';

@Component({
    selector: 'app-navbar',
    templateUrl: './navbar.component.html',
    styleUrls: ['./navbar.component.scss'],
    standalone: false
})
export class NavbarComponent {
  currentUser$: Observable<User | null>;
  isAdmin = false;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {
    this.currentUser$ = this.authService.currentUser$;
    
    this.currentUser$.subscribe(user => {
      this.isAdmin = user?.roles?.includes(role.admin) || false;
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/signin']);
  }
}