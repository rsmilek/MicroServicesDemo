import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError, catchError } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { AuthService } from '../../../services/auth.service';
import { ApiResponse, User } from '../../../models';
import { role } from '../../../utilities/role';
import { NotificationService } from '../../../services/notification.service';

/**
 * Manages api controller admin communication.
 */
@Injectable({
  providedIn: 'root'
})
export class AdminApiService {

  private readonly authApiBaseUrl = environment.authApiBaseUrl;

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private notificationService: NotificationService
  ) { }

  addRoleToUser(email: string, userRole: string): Observable<ApiResponse<any>> {
    const headers = this.getAuthHeaders();
    return this.http.post<ApiResponse<any>>(
      `${this.authApiBaseUrl}/api/admin/add-role/${encodeURIComponent(email)}?role=${userRole}`, {}, { headers }
    ).pipe(
      catchError(this.handleError)
    );
  }

  removeRoleFromUser(email: string, userRole: string): Observable<ApiResponse<any>> {
    const headers = this.getAuthHeaders();
    return this.http.delete<ApiResponse<any>>(
      `${this.authApiBaseUrl}/api/admin/remove-role/${encodeURIComponent(email)}?role=${userRole}`, { headers }
    ).pipe(
      catchError(this.handleError)
    );
  }

  getAllUsers(): Observable<ApiResponse<User[]>> {
    const headers = this.getAuthHeaders();
    return this.http.get<ApiResponse<User[]>>(
      `${this.authApiBaseUrl}/api/admin/users`, { headers }
    ).pipe(
      catchError(this.handleError)
    );
  }

  getAllRoles(): Observable<ApiResponse<string[]>> {
    const headers = this.getAuthHeaders();
    return this.http.get<ApiResponse<string[]>>(
      `${this.authApiBaseUrl}/api/admin/roles`,{ headers }
    ).pipe(
      catchError(this.handleError)
    );
  }
 
  private getAuthHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  // Arrow function must be used here to preserve 'this' context
  private handleError = (error: any): Observable<never> => {
    console.error('Admin API service error:', error);
    this.notificationService.openErrorNotification('User API service error!');
    return throwError(() => error);
  }
}