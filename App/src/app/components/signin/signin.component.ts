import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { AuthApiService } from '../../services/auth-api.service';
import { LoginResponse } from '../../models';
import { NotificationService } from 'src/app/services/notification.service';

@Component({
    selector: 'app-signin',
    templateUrl: './signin.component.html',
    styleUrls: ['./signin.component.scss'],
    standalone: false
})
export class SigninComponent implements OnInit {
  isAuthenticated = false;
  signinForm: FormGroup;
  isLoading = false;

  constructor(
    private authService: AuthService,
    private authApiService: AuthApiService,
    private router: Router,
    private formBuilder: FormBuilder,
    private notificationService: NotificationService
  ) {
    this.signinForm = this.formBuilder.group({
      userName: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    // Handle OAuth callback if present
    this.authService.handleAuthCallback();
    // Check if user is already authenticated
    this.isAuthenticated = this.authService.isAuthenticated();    
    if (this.isAuthenticated) {
      this.notificationService.openSuccessNotification('You are already signed-in!');
      // Redirect to dashboard after a short delay
      this.router.navigate(['/dashboard']);
    }
  }

  get isUserNameValid(): boolean {
    const userNameControl = this.signinForm.get('userName');
    return userNameControl ? userNameControl.valid && userNameControl.value : false;
  }

  onSigninWithEmail(): void {
    if (this.signinForm.valid) {
      this.isLoading = true;

      const signinData = this.signinForm.value;
      this.authApiService.signIn(signinData).subscribe({
        next: (response: LoginResponse) => {
          this.isLoading = false;
          if (response.success) {
            this.authService.setToken(response.data.token);
            this.authService.checkTokenAndLoadUser();
            this.notificationService.openSuccessNotification('Sign-in successful!');
            // Check if user is already authenticated
            this.isAuthenticated = this.authService.isAuthenticated();            
            if (this.isAuthenticated) {
              // Redirect to dashboard after a short delay
              this.router.navigate(['/dashboard']);
            }
          } else {
            const message = response.message || 'Sign-in failed. Please try again.';
            this.notificationService.openErrorNotification(message);
          }
        },
        error: (error: any) => {
          this.notificationService.openErrorNotification('An error occurred during sign-in. Please try again.');
          this.isLoading = false;
        }
      });
    }
  }

  onSigninWithMicrosoft(): void {
    this.authApiService.signInWithMicrosoft();
  }

}