import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { User } from '../models';
import { AuthApiService } from './auth-api.service';
import { UserApiService } from './user-api.service';
import { role } from '../utilities/role';

/**
 * Manages app token handling to secure apis communication.
 */
@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  private tokenKey = 'jwt_token';

  constructor(
    private userApiService: UserApiService
  ) {
    this.checkTokenAndLoadUser();
  }

  checkTokenAndLoadUser(): void {
    console.log('checkTokenAndLoadUser()');
    const token = this.getToken();
    if (token) {
      console.log('Token found:', token);
      this.userApiService.getCurrentUser(token).subscribe({
        next: (response) => {
          if (response.success && response.data) {
            console.log('User data loaded:', response.data);
            this.currentUserSubject.next(response.data);
          }
        },
        error: () => {
          this.logout();
        }
      });
    }
    else {
      console.log('No token found');
    }
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  setToken(token: string): void {
    console.log('Setting token:', token);
    localStorage.setItem(this.tokenKey, token);
  }

  removeToken(): void {
    localStorage.removeItem(this.tokenKey);
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  isAdmin(): boolean {
    const user = this.currentUserSubject.value;
    return user?.roles?.includes(role.admin) || false;
  }

  handleAuthCallback(): void {
    console.log('handleAuthCallback()');
    
    const urlParams = new URLSearchParams(window.location.search);
    const token = urlParams.get('token');
    if (token) {
      this.setToken(token);
      this.checkTokenAndLoadUser();
      // Clean URL - SECURITY IMPROVEMENT !!!
      window.history.replaceState({}, document.title, window.location.pathname);
    }
    else {
      console.log('Token: not found in URL parameters');
    }
  }

  logout(): void {
    this.removeToken();
    this.currentUserSubject.next(null);
  }

}