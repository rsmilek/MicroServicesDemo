# Msd.Services.AuthApi

.NET 10 Microservice for Authentication and Authorization featuring Microsoft OAuth 2.0, ASP.NET Identity, JWT authentication, and comprehensive role-based authorization.

## Features

- ✅ Microsoft OAuth 2.0 integration
- ✅ ASP.NET Identity for user management
- ✅ JWT token authentication
- ✅ Role-based authorization (User, Admin)
- ✅ Entity Framework Core with SQL Server
- ✅ Swagger/OpenAPI documentation
- ✅ CORS configuration
- ✅ RESTful API endpoints

## Requirements

- .NET 10.0 SDK
- SQL Server (LocalDB)
- Social Provider Account
- Microsoft Azure account (for Microsoft login)

## Installation and Setup

1. **Navigate to the API directory**
   ```bash
   cd Services
   ```

2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure Microsoft App**
   - Register an application in [Azure Portal](https://portal.azure.com/) under Azure Active Directory > App registrations
   - Get Application (client) ID and create a Client Secret
   - Configure Redirect URIs: `https://localhost:5001/signin-microsoft`
   - Under "API permissions", add Microsoft Graph > User.Read (delegated)


4. **Configure Facebook App** (optional - not currently implemented)
   - This feature is planned for future implementation

5. **Configure appsettings.json**
   ```json
   {
     "Microsoft": {
       "ClientId": "YOUR_MICROSOFT_CLIENT_ID",
       "ClientSecret": "YOUR_MICROSOFT_CLIENT_SECRET"
     },
     "Jwt": {
       "Key": "YOUR_SUPER_SECRET_KEY_AT_LEAST_32_CHARACTERS",
       "Issuer": "https://localhost:5001",
       "Audience": "https://localhost:4200"
     }
   }
   ```

6. **Database migration**
   ```bash
   Add-Migration <MigrationName>
   Update-Database
   ```
   or
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

7. **Run the application**
   ```bash
   dotnet run --project Msd.Services.AuthApi
   ```
   
   Or from the solution directory:
   ```bash
   dotnet run
   ```

8. **Access Swagger documentation**
   - Open browser and go to: `https://localhost:5001`

## API Endpoints

### Authentication
- `GET /authentication/signin/microsoft?redirectUrl={url}` - Complete Microsoft OAuth flow and redirect with token
- `POST /authentication/signup` - Register new user with email/password
- `POST /authentication/signin/email` - Sign in with email and password

### Users
- `GET /api/user/me` - Get current user information (requires JWT token, User or Admin role)

### Administration (Admin only)
- `POST /api/admin/add-role/{email}?role=Admin` - Add role to user
- `DELETE /api/admin/remove-role/{email}?role=Admin` - Remove role from user
- `GET /api/admin/users` - List of all users with their roles
- `GET /api/admin/roles` - List of all roles

## Using JWT Token

After successful login (Microsoft OAuth or email/password), you will receive a JWT token. Use this token in the Authorization header:

```
Authorization: Bearer YOUR_JWT_TOKEN
```

### Login Flow Examples:

**Microsoft OAuth Flow:**
1. Navigate to: `GET /authentication/signin/microsoft?redirectUrl=https://localhost:4200/auth-callback`
2. Complete Microsoft authentication
3. Get redirected to your app with token: `https://localhost:4200/auth-callback?token=YOUR_JWT_TOKEN`

**Email/Password Flow:**
1. Register: `POST /authentication/signup` with body:
   ```json
   {
     "email": "user@example.com",
     "password": "YourPassword123!",
     "name": "Your Name"
   }
   ```
2. Login: `POST /authentication/signin/email` with body:
   ```json
   {
     "email": "user@example.com",
     "password": "YourPassword123!"
   }
   ```
3. Response contains the JWT token

## Project Structure

```
Msd.Services.AuthApi/
├── Controllers/              # API Controllers
│   ├── AdminController.cs    # Admin operations
│   ├── AuthenticationController.cs # Auth operations
│   └── UserController.cs     # User operations
├── Data/                     # DbContext and configurations
│   └── AppDbContext.cs       # Entity Framework context
├── Models/                   # DTOs and data models
│   ├── Dtos/                 # Data Transfer Objects
│   ├── ApiResponse.cs        # API response wrapper
│   └── UserInfo.cs           # User information model
├── Services/                 # Business logic services
│   ├── IAuthService.cs       # Auth service interface
│   ├── AuthService.cs        # Auth service implementation
│   ├── ITokenService.cs      # Token service interface
│   └── TokenService.cs       # JWT token service
├── Migrations/               # EF Core migrations
├── Properties/               # Launch settings
├── Utilities/                # Utility classes
├── Program.cs                # Application startup
├── appsettings.json          # Configuration
└── Msd.Services.AuthApi.csproj # Project file
```

## Technologies

- **Framework**: .NET 10.0
- **ORM**: Entity Framework Core 10.0
- **Database**: SQL Server
- **Authentication**: ASP.NET Identity + Social OAuth + Microsoft OAuth
- **Authorization**: JWT Bearer tokens
- **Documentation**: Swagger/OpenAPI
- **CORS**: Configured for development

## Development

### Adding new migration
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Adding new role
Use Admin endpoint or add manually via Swagger:
```
POST /api/admin/add-role/user@example.com?role=Admin
```

## Security

- JWT tokens are valid for 1 hour
- Change passwords in appsettings.json for production
- Keep Social App Secret and Microsoft Client Secret secure
- Use stronger JWT key in production (at least 32 characters)

## License

This project is intended for educational purposes.This project is intended for educational purposes.