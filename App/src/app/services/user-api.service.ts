import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError, catchError } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiResponse, User } from '../models';
import { NotificationService } from './notification.service';

/**
 * Manages api controller user communication.
 */
@Injectable({
  providedIn: 'root'
})
export class UserApiService {
  
  private readonly authApiBaseUrl = environment.authApiBaseUrl;

  constructor(
    private http: HttpClient,
    private notificationService: NotificationService
  ) {}

  getCurrentUser(token: string): Observable<ApiResponse<User>> {
    const headers = this.getAuthHeaders(token);
    return this.http.get<ApiResponse<User>>(
        `${this.authApiBaseUrl}/api/user/me`, { headers })
      .pipe(
        catchError(this.handleError)
      );
  }
 
  private getAuthHeaders(token: string): HttpHeaders {
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  // Arrow function must be used here to preserve 'this' context
  private handleError = (error: any): Observable<never> => {
    console.error('User API service error:', error);  
    this.notificationService.openErrorNotification('User API service error!');
    return throwError(() => error);
  }
}