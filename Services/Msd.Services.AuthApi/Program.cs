using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Msd.Integration.MessageBus;
using Msd.Services.AuthApi.Data;
using Msd.Services.AuthApi.Extensions;
using Msd.Services.AuthApi.Services;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------- Database and Identity configuration ----------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Msd.Services.AuthApiDbConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ---------- Authentication ----------
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie()
.AddMicrosoftAccount(options =>
{
    options.ClientId = builder.Configuration["Microsoft:ClientId"]!;
    options.ClientSecret = builder.Configuration["Microsoft:ClientSecret"]!;
    options.CallbackPath = "/authentication/microsoft/callback";
    options.SaveTokens = true;
    // Scopes
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("User.Read");
    // Mapping MS claims to standard claims
    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "displayName");
    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "mail");
    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "userPrincipalName");
    options.ClaimActions.MapJsonKey("picture", "picture");
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration["Jwt:Key"] ?? throw new Exception("Jwt:Key is undefined!"))),
        ValidateLifetime = true,
    };
});

// ---------- Services ----------
builder.Services.AddScoped<IEmailMessageFactory, EmailMessageFactory>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IMessageBus, MessageBus>();

// ---------- Controllers and API ----------
builder.Services.AddControllers();

// ---------- CORS ----------
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// ---------- Swagger/OpenAPI ----------
builder.Services.AddSwaggerConfiguration();

// ---------- Authorization ----------
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

// ---------- Configure the HTTP request pipeline ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth SSO API v1");
        c.RoutePrefix = string.Empty; // Swagger on root URL
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// ---------- Controllers ----------
app.MapControllers();

// ---------- Root endpoint ----------
app.MapGet("/", () => new { 
    message = "Auth SSO API with ASP.NET Identity and Role-Based authorization",
    version = "1.0",
    swagger = "/swagger"
})
.WithTags("Root")
.WithSummary("Root endpoint")
.WithDescription("Basic API information");

app.Run();

