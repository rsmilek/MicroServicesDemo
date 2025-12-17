using Msd.Services.EmailApi.Extensions;
using Msd.Services.EmailApi.Messaging;
using Msd.Services.EmailApi.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------- Services ----------
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

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
