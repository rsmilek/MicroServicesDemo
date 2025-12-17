using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Msd.Services.EmailApi.Extensions;
using Msd.Services.EmailApi.Messaging;
using Msd.Services.EmailApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------- Services ----------
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

// ---------- Authentication ----------
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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

var app = builder.Build();

// ---------- Configure the HTTP request pipeline ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Email API v1");
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
app.MapGet("/", () => new
{
    message = "Email API using Azure Communication Services",
    version = "1.0",
    swagger = "/swagger"
})
.WithTags("Root")
.WithSummary("Root endpoint")
.WithDescription("Basic API information");

// ---------- Register Azure Service Bus Consumer ----------
app.UseAzureServiceBusConsumer();

app.Run();
