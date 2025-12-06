import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { User } from '../../models';
import { Observable, Subject, takeUntil } from 'rxjs';
import { role } from 'src/app/utilities/role';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss'],
    standalone: false
})
export class DashboardComponent implements OnInit, OnDestroy {
  currentUser$: Observable<User | null>;
  isAdmin = false;
  private destroy$ = new Subject<void>();

  constructor(private authService: AuthService) {
    this.currentUser$ = this.authService.currentUser$;
  }

  ngOnInit(): void {
    this.currentUser$
      .pipe(takeUntil(this.destroy$))
      .subscribe(user => {
        this.isAdmin = user?.roles?.includes(role.admin) || false;
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}