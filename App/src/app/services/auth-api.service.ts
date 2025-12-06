import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError, catchError } from 'rxjs';
import { environment } from '../../environments/environment';
import { LoginResponse, SignupRequest, LoginRequest } from '../models';
import { NotificationService } from './notification.service';

/**
 * Manages api controller authentication communication.
 */
@Injectable({
  providedIn: 'root'
})
export class AuthApiService {

  private readonly appBaseUrl = environment.appBaseUrl;
  private readonly authApiBaseUrl = environment.authApiBaseUrl;
    
  constructor(
    private http: HttpClient,
    private notificationService: NotificationService
  ) {}

  signInWithMicrosoft(): void {
    const baseUrl = `${this.authApiBaseUrl}/authentication/signin/microsoft`;
    const params = new URLSearchParams({
        redirecturl: `${this.appBaseUrl}/signin`
    });
    window.location.href = `${baseUrl}?${params.toString()}`;    
  }

  signInWithMicrosoftCallBack(): void {
    const baseUrl = `${this.authApiBaseUrl}/authentication/microsoft/signin`;
    const params = new URLSearchParams({
        redirecturi: `${this.appBaseUrl}/signin`
    });
    window.location.href = `${baseUrl}?${params.toString()}`;    
  }

  signUp(signupData: SignupRequest): Observable<LoginResponse> {
    const headers = this.getHeaders();
    return this.http.post<LoginResponse>(
      `${this.authApiBaseUrl}/authentication/signup`, signupData, { headers })
      .pipe(
        catchError(this.handleError)
      );
  }

  signIn(loginData: LoginRequest): Observable<LoginResponse> {
    const headers = this.getHeaders();
    return this.http.post<LoginResponse>(
      `${this.authApiBaseUrl}/authentication/signin/email`, loginData, { headers })
      .pipe(
        catchError(this.handleError)
      );
  }

  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json'
    });
  }

  // Arrow function must be used here to preserve 'this' context
  private handleError = (error: any): Observable<never> => {
    console.error('Auth API service error:', error);
    this.notificationService.openErrorNotification(`Auth API service error!`);    
    return throwError(() => error);
  }
}