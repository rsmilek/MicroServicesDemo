import { Component, OnInit } from '@angular/core';
import { AdminApiService } from '../../services/admin-api.service';
import { User } from '../../../../models';
import { role } from '../../../../utilities/role';
import { NotificationService } from '../../../../services/notification.service';

@Component({
    selector: 'app-role-management',
    templateUrl: './role-management.component.html',
    styleUrls: ['./role-management.component.scss'],
    standalone: false
})
export class RoleManagementComponent implements OnInit {
  userEmail = '';
  selectedRole = role.admin;
  isLoading = false;
  isLoadingUsers = false;
  isLoadingRoles = false;
  isTestingAccess = false;
  
  users: User[] = [];
  roles: string[] = [];
  displayedColumns: string[] = ['userName', 'email', 'roles'];

  constructor(
    private adminApiService: AdminApiService,
    private notificationService: NotificationService
  ) { }

  ngOnInit(): void {
    this.loadUsersInternal({ notification: false });
    this.loadRolesInternal({ notification: false });
  }

  addRoleToUser(): void {
    if (!this.userEmail.trim()) {
          this.notificationService.openErrorNotification('Please enter a valid email address');
      return;
    }

    this.isLoading = true;

    this.adminApiService.addRoleToUser(this.userEmail, this.selectedRole).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success) {
          this.notificationService.openSuccessNotification(response.message);
          this.loadUsersInternal({ notification: false }); // Refresh users list
        } else {
          this.notificationService.openErrorNotification(response.message);
        }
      },
      error: (error) => {
        this.isLoading = false;
        var message = error.error.message || 'An error occurred user role adding!';
        if (error.error.data) {
          message += ` ${error.error.data}`;
        }
        this.notificationService.openErrorNotification(message);
      }
    });
  }

  removeRoleFromUser(): void {
    if (!this.userEmail.trim()) {
      this.notificationService.openErrorNotification('Please enter a valid email address');
      return;
    }

    this.isLoading = true;

    this.adminApiService.removeRoleFromUser(this.userEmail, this.selectedRole).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success) {
          this.notificationService.openSuccessNotification(response.message);
          this.loadUsersInternal({ notification: false }); // Refresh users list
        } else {
          this.notificationService.openErrorNotification(response.message);
        }
      },
      error: (error) => {
        this.isLoading = false;
        var message = error.error.message || 'An error occurred during role removal!';
        if (error.error.data) {
          message += ` ${error.error.data}`;
        }
        this.notificationService.openErrorNotification(message);
      }
    });
  }

  loadUsers(): void {
    this.loadUsersInternal();
  }

  loadRoles(): void {
    this.loadRolesInternal();
  }

  private loadUsersInternal(options: { notification?: boolean } = {}): void {
    const notification = options.notification ?? true;
    this.isLoadingUsers = true;
    this.adminApiService.getAllUsers().subscribe({
      next: (response) => {
        this.isLoadingUsers = false;
        if (response.success && response.data) {
          this.users = response.data;
          if (notification) {
            this.notificationService.openSuccessNotification('Users loaded successfully!');
          }
        }
      },
      error: (error) => {
        this.isLoadingUsers = false;
        this.notificationService.openErrorNotification('An error occurred during loading users!');
      }
    });
  }

   private loadRolesInternal(options: { notification?: boolean } = {}): void {
    const notification = options.notification ?? true;
    this.isLoadingRoles = true;
    this.adminApiService.getAllRoles().subscribe({
      next: (response) => {
        this.isLoadingRoles = false;
        if (response.success && response.data) {
          this.roles = response.data;
          if (notification) { 
            this.notificationService.openSuccessNotification('Roles loaded successfully!');
          }
        }
      },
      error: (error) => {
        this.isLoadingRoles = false;
        this.notificationService.openErrorNotification('An error occurred during loading roles!');
      }
    });
  }
 
}