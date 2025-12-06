# MSD App

Angular 20.1 frontend application for the MicroServicesDemo project, featuring Microsoft OAuth authentication and comprehensive role-based access control.

## Features

- ✅ Multi-provider OAuth 2.0 integration (Microsoft)
- ✅ JWT token authentication
- ✅ Role-based access control (User, Admin)
- ✅ Modern Angular 20.1 with latest features
- ✅ Responsive design with Material Design and Font Awesome icons
- ✅ Protected routes with functional guards
- ✅ Admin panel for user management
- ✅ User profile management
- ✅ Real-time authentication state
- ✅ Modern control flow syntax (@if, @for)
- ✅ Consolidated SCSS architecture
- ✅ No animations for optimal performance

## Requirements

- Node.js 18+ 
- npm (standard install, no legacy peer deps needed)
- Angular CLI 20+
- TypeScript 5.8+
- Auth SSO API running (backend)

## Installation and Setup

1. **Install dependencies**
   ```bash
   npm install
   ```

2. **Configure API endpoint**
   Update `src/environments/environment.ts`:
   ```typescript
   export const environment = {
     production: false,
     appBaseUrl: 'http://localhost:4200',
     authApiBaseUrl: 'https://localhost:5001'  // Your Auth API URL
   };
   ```

3. **Start development server**
   ```bash
   npm start
   ```
   or
   ```bash
   ng serve
   ```

4. **Access the application**
   - Open browser and go to: `http://localhost:4200`

## Project Structure

```
src/
├── app/
│   ├── components/           # Angular components
│   │   ├── signin/          # Sign-in component with multi-provider auth
│   │   ├── signup/          # Sign-up component
│   │   ├── dashboard/       # Dashboard component
│   │   ├── navbar/          # Navigation component
│   │   ├── user-profile/    # User profile component
│   │   └── role-management/ # Admin role management
│   ├── services/            # Angular services
│   │   ├── auth.service.ts        # Authentication service
│   │   ├── auth-api.service.ts    # Auth API operations
│   │   ├── admin-api.service.ts   # Admin API operations
│   │   ├── user-api.service.ts    # User API operations
│   │   └── notification.service.ts # Notifications
│   ├── guards/              # Functional route guards
│   │   ├── auth.guard.ts    # Authentication guard
│   │   └── admin.guard.ts   # Admin role guard
│   ├── models/              # TypeScript models
│   │   ├── user.model.ts    # User model
│   │   ├── api-response.model.ts # API response model
│   │   └── index.ts         # Model exports
│   ├── utilities/           # Utility modules
│   │   ├── material/        # Material module
│   │   └── role.ts          # Role utilities
│   ├── app-routing.module.ts # App routing configuration
│   └── app.module.ts        # Main app module
├── environments/            # Environment configurations
└── styles.scss              # Global consolidated styles
```

## Available Routes

- `/signin` - Sign-in page with multiple OAuth providers
- `/signup` - Sign-up page for new accounts
- `/dashboard` - User dashboard (protected)
- `/profile` - User profile page (protected)
- `/admin` - Admin panel (protected, Admin role required)

## Authentication Flow

1. User chooses from multiple sign-in options (Microsoft, Google, GitHub, Facebook)
2. Redirects to selected OAuth provider
3. Provider redirects back with authorization code
4. Backend exchanges code for JWT token
5. Frontend stores JWT token
6. User can access protected routes based on roles

## API Integration

The app integrates with the Msd.Services.AuthApi endpoints:

**Authentication Endpoints:**
- `GET /authentication/signin/microsoft?redirectUrl={url}` - Microsoft OAuth flow
- `POST /authentication/signup` - Register new user
- `POST /authentication/signin/email` - Email/password sign-in

**User Endpoints:**
- `GET /api/user/me` - Get current user information

**Admin Endpoints (Admin role required):**
- `POST /api/admin/add-role/{email}?role=Admin` - Add role to user
- `DELETE /api/admin/remove-role/{email}?role=Admin` - Remove role from user
- `GET /api/admin/users` - Get all users with roles
- `GET /api/admin/roles` - Get all available roles## Development

### Build for production
```bash
npm run build
```

### Run tests
```bash
npm test
```

### Linting
```bash
ng lint
```

## Technologies

- **Framework**: Angular 20.1.0
- **Build System**: @angular/build (Application builder)
- **TypeScript**: 5.8.0
- **Styling**: SCSS with consolidated architecture + Angular Material 20
- **Icons**: Font Awesome 7.1.0
- **HTTP Client**: Modern Angular HttpClient with provideHttpClient
- **Routing**: Angular Router with functional guards
- **State Management**: RxJS Observables
- **Authentication**: JWT tokens
- **Animations**: Disabled for optimal performance (provideNoopAnimations)
- **UI Components**: Angular Material + custom components
- **Control Flow**: Modern @if/@for syntax (Angular 20)

## Configuration

### Environment Variables

**Development** (`src/environments/environment.ts`):
```typescript
export const environment = {
  production: false,
  appBaseUrl: 'http://localhost:4200',
  authApiBaseUrl: 'https://localhost:5001'
};
```

**Production** (`src/environments/environment.prod.ts`):
```typescript
export const environment = {
  production: true,
  appBaseUrl: 'https://your-app-domain.com',
  authApiBaseUrl: 'https://your-auth-api-domain.com'
};
```

## Security

- JWT tokens stored in localStorage
- Protected routes with authentication guards
- Role-based access control
- Automatic token validation
- Secure HTTP interceptors

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## License

This project is intended for educational purposes.