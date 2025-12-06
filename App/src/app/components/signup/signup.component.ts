import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { AuthApiService } from '../../services/auth-api.service';
import { LoginResponse } from '../../models';
import { NotificationService } from 'src/app/services/notification.service';

@Component({
    selector: 'app-signup',
    templateUrl: './signup.component.html',
    styleUrls: ['./signup.component.scss'],
    standalone: false
})
export class SignupComponent implements OnInit {
  signupForm: FormGroup;
  isLoading = false;
  isAuthenticated = false;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private authApiService: AuthApiService,
    private router: Router,
    private notificationService: NotificationService
  ) {
    this.signupForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
      name: ['']
    });
  }

  ngOnInit(): void {
    // Check if user is already authenticated
    this.isAuthenticated = this.authService.isAuthenticated();    
    if (this.isAuthenticated) {
      this.notificationService.openSuccessNotification('You are already signed-in!');
      // Redirect to dashboard after a short delay
      this.router.navigate(['/dashboard']);
    }
  }

  onSignup(): void {
    if (this.signupForm.valid) {
      this.isLoading = true;

      const signupData = this.signupForm.value;
      this.authApiService.signUp(signupData).subscribe({
        next: (response: LoginResponse) => {
          this.isLoading = false;
          if (response.success) {
            this.authService.setToken(response.data.token);
            this.authService.checkTokenAndLoadUser();            
            this.notificationService.openSuccessNotification('Account created successfully! Redirecting to sign-in...');
            this.router.navigate(['/signin']);
          } else {
            const message = response.message || 'Sign-up failed. Please try again.';
            this.notificationService.openErrorNotification(message);
          }
        },
        error: (error: any) => {
          this.notificationService.openErrorNotification('An error occurred during sign-up. Please try again.');
          this.isLoading = false;
        }
      });
    }
  }
}