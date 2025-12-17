# MicroServicesDemo - Authentication Service

A complete microservices demonstration project featuring:
- ASP.NET Core 10 Auth API with Microsoft OAuth integration
- ASP.NET Identity + role-based authorization
- JWT token authentication
- Angular 20.1 frontend with Material Design
- Comprehensive role management system
- Email service with Azure Communication Services

## See Also

- [Authentication API - Full Documentation](./Services/Msd.Services.AuthApi/README.md) - Microsoft OAuth, JWT, Role-based auth
- [Email API - Full Documentation](./Services/Msd.Services.EmailApi/README.md) - Azure Communication Services integration, setup guide
- [Frontend Application - Full Documentation](./App/README.md) - Angular SPA with Material Design

## Project Structure

```
MicroServicesDemo/
├── Services/                     # Backend API Services
│   ├── Msd.Services.sln         # Solution file
│   ├── Msd.Services.AuthApi/     # Authentication API Service
│   │   ├── Controllers/          # API Controllers
│   │   ├── Data/                 # Entity Framework DbContext
│   │   ├── Models/               # DTOs and Data Models
│   │   ├── Services/             # Business Logic Services
│   │   └── Migrations/           # Database Migrations
│   └── Msd.Services.EmailApi/    # Email API Service
│       ├── Controllers/          # API Controllers
│       ├── Models/               # DTOs and Data Models
│       └── Services/             # Email Services
└── App/                          # Angular Frontend Application
    ├── src/app/components/       # UI Components
    ├── src/app/services/         # Angular Services
    ├── src/app/guards/           # Route Guards
    └── src/app/models/           # TypeScript Models
```

## Quick Start

### 1. Backend API Setup

**Authentication API:**
```bash
cd Services
dotnet restore
dotnet ef database update --project Msd.Services.AuthApi
dotnet run --project Msd.Services.AuthApi
```
*API will be available at: https://localhost:5001*

**Email API:**
```bash
cd Services
dotnet restore
dotnet run --project Msd.Services.EmailApi
```
*API will be available at: https://localhost:7003*

### 2. Frontend App Setup
```bash
cd App
npm install
npm start
```
*App will be available at: http://localhost:4200*

## Default Authentication

**Admin Account:**
- Email: `admin@admin.com`
- Password: `Admin123`

## Documentation

- [🔧 Authentication API Documentation](./Services/Msd.Services.AuthApi/README.md)
- [📧 Email API Documentation](./Services/Msd.Services.EmailApi/README.md)
- [🚀 Frontend Documentation](./App/README.md)

## Technology Stack

**Backend:**
- .NET 10.0
- ASP.NET Core Identity
- Entity Framework Core
- JWT Authentication
- Microsoft OAuth 2.0
- Azure Communication Services
- SQL Server

**Frontend:**
- Angular 20.1
- Angular Material
- TypeScript 5.8
- RxJS
- SCSS


