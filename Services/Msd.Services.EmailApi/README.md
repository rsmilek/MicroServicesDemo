# Msd.Services.EmailApi

.NET 10 Microservice for sending emails using Azure Communication Services. Designed for simple, transactional email sending with minimal overhead - perfect for low-volume scenarios (few emails per day).

## Features

- ✅ Azure Communication Services Email integration
- ✅ Simple text-based email sending
- ✅ RESTful API endpoint
- ✅ Swagger/OpenAPI documentation
- ✅ CORS configuration
- ✅ Structured logging
- ✅ Standard API response format

## Requirements

- .NET 10.0 SDK
- Azure subscription
- Azure Communication Services resource with Email service configured

## Azure Communication Services Setup

### Step 1: Create Azure Communication Services Resource

1. **Sign in to Azure Portal**
   - Navigate to [Azure Portal](https://portal.azure.com/)

2. **Create Communication Services Resource**
   - Click **"Create a resource"**
   - Search for **"Communication Services"**
   - Click **"Create"**
   - Fill in the details:
     - **Subscription**: Select your subscription
     - **Resource Group**: Create new or use existing
     - **Resource Name**: Choose a unique name (e.g., `msd-email-service`)
     - **Data Location**: Select region closest to your users
   - Click **"Review + Create"** → **"Create"**

3. **Get Connection String**
   - Once deployed, go to your Communication Services resource
   - Navigate to **"Keys"** in the left menu
   - Copy the **"Primary connection string"**
   - Save it securely - you'll need it in `appsettings.json`

### Step 2: Configure Email Communication Service

1. **Add Email Service to Communication Services**
   - In your Communication Services resource, go to **"Email"** → **"Try Email"**
   - Or create a separate **"Email Communication Services"** resource

2. **Set Up Email Domain**

   **Option A: Use Azure Managed Domain (Quick Start)**
   - Azure provides a free subdomain (e.g., `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx.azurecomm.net`)
   - Navigate to **"Email"** → **"Provision domains"**
   - Click **"Add domain"** → **"Azure subdomain"**
   - Click **"Create"**
   - Note the sender address (e.g., `DoNotReply@xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx.azurecomm.net`)

   **Option B: Use Custom Domain (Production)**
   - Go to **"Email"** → **"Provision domains"**
   - Click **"Add domain"** → **"Custom domain"**
   - Enter your domain name (e.g., `yourdomain.com`)
   - Follow DNS verification steps (add TXT, SPF, DKIM records)
   - Wait for verification (can take up to 24 hours)
   - Configure sender address (e.g., `noreply@yourdomain.com`)

3. **Connect Domain to Communication Services**
   - In **"Email"** → **"Domains"**, select your domain
   - Click **"Connect domain"**
   - Select your Communication Services resource
   - Confirm connection

### Step 3: Configure Sender Address

1. **Get Verified Sender Address**
   - Navigate to your Email domain
   - Go to **"MailFrom addresses"**
   - Default is `DoNotReply@<your-domain>`
   - You can add custom mailFrom addresses if needed

2. **Set Up Recipients (Development)**
   - For development/testing, you may need to verify recipient addresses
   - Navigate to **"Email"** → **"Settings"** → **"Verified senders"**
   - Add test recipient email addresses

### Step 4: Check Quotas and Limits

- Free tier includes:
  - **500 emails/month** for Azure subdomain
  - **$100 credit** for custom domains (about 83,000 emails)
- Check current pricing: [Azure Communication Services Pricing](https://azure.microsoft.com/en-us/pricing/details/communication-services/)

## Installation and Setup

1. **Navigate to the Email API directory**
   ```bash
   cd Services/Msd.Services.EmailApi
   ```

2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure appsettings.json**
   
   Update the configuration with your Azure Communication Services details:

   ```json
   {
     "AzureCommunicationServices": {
       "ConnectionString": "endpoint=https://YOUR-RESOURCE.communication.azure.com/;accesskey=YOUR-ACCESS-KEY",
       "SenderAddress": "DoNotReply@xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx.azurecomm.net"
     }
   }
   ```

   **Connection String**: From Azure Portal → Communication Services → Keys → Primary connection string
   
   **SenderAddress**: From Azure Portal → Email → Domains → MailFrom addresses

4. **Run the application**
   ```bash
   dotnet run --project Msd.Services.EmailApi
   ```
   
   Or from the solution directory:
   ```bash
   dotnet run
   ```

5. **Access Swagger documentation**
   - Open browser and go to: `https://localhost:7003` or `http://localhost:5003`

## API Endpoints

### Email Operations

- `POST /api/email/send` - Send an email

**Request Body:**
```json
{
  "to": "recipient@example.com",
  "subject": "Test Email",
  "body": "This is a plain text email message."
}
```

**Response (Success):**
```json
{
  "success": true,
  "message": "Email sent successfully",
  "data": {
    "messageId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
    "status": "Succeeded"
  }
}
```

**Response (Error):**
```json
{
  "success": false,
  "message": "Failed to send email: [error details]",
  "data": null
}
```

## Testing the API

### Using Swagger UI

1. Navigate to `https://localhost:7003`
2. Click on **POST /api/email/send**
3. Click **"Try it out"**
4. Enter request body:
   ```json
   {
     "to": "your-email@example.com",
     "subject": "Test from Email API",
     "body": "This is a test email from the Email API."
   }
   ```
5. Click **"Execute"**

### Using cURL

```bash
curl -X POST "https://localhost:7003/api/email/send" \
  -H "Content-Type: application/json" \
  -d '{
    "to": "recipient@example.com",
    "subject": "Test Email",
    "body": "Hello from Email API!"
  }'
```

### Using PowerShell

```powershell
$body = @{
    to = "recipient@example.com"
    subject = "Test Email"
    body = "Hello from Email API!"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:7003/api/email/send" `
    -Method Post `
    -ContentType "application/json" `
    -Body $body
```

## Project Structure

```
Msd.Services.EmailApi/
├── Controllers/                    # API Controllers
│   └── EmailController.cs          # Email operations endpoint
├── Models/                         # Data models
│   ├── ApiResponse.cs              # Standard API response wrapper
│   └── Dtos/                       # Data Transfer Objects
│       ├── SendEmailRequestDto.cs  # Email request model
│       └── SendEmailResponseDto.cs # Email response model
├── Services/                       # Business logic services
│   ├── IEmailService.cs            # Email service interface
│   └── EmailService.cs             # Azure Communication Services implementation
├── Properties/                     # Launch settings
│   └── launchSettings.json         # Development profiles
├── Program.cs                      # Application startup
├── appsettings.json                # Configuration
├── appsettings.Development.json    # Development configuration
└── Msd.Services.EmailApi.csproj    # Project file
```

## Technologies

- **Framework**: .NET 10.0
- **Email Service**: Azure Communication Services
- **Documentation**: Swagger/OpenAPI
- **CORS**: Configured for development
- **Logging**: ASP.NET Core structured logging

## Configuration Options

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "AzureCommunicationServices": {
    "ConnectionString": "YOUR_CONNECTION_STRING",
    "SenderAddress": "DoNotReply@YOUR_DOMAIN.com"
  }
}
```

### Environment Variables (Alternative)

You can also configure using environment variables:

- `AzureCommunicationServices__ConnectionString`
- `AzureCommunicationServices__SenderAddress`

## Cost Estimation

For **low volume** (few emails per day):

- **Azure Managed Domain**: ~$0.0012 per email
  - 10 emails/day = ~$0.36/month
  - 100 emails/day = ~$3.60/month

- **Custom Domain**: First $100 free, then ~$0.0012 per email
  - Includes domain verification and management

**Free tier**: 500 emails/month with Azure subdomain

## Security Best Practices

1. **Never commit credentials**
   - Use Azure Key Vault for connection strings in production
   - Use User Secrets for local development:
     ```bash
     dotnet user-secrets set "AzureCommunicationServices:ConnectionString" "your-connection-string"
     ```

2. **Validate email addresses**
   - Consider adding email validation logic
   - Implement rate limiting for production

3. **Monitor usage**
   - Set up Azure Monitor alerts for quota limits
   - Track failed email attempts

4. **CORS configuration**
   - Restrict CORS in production to specific origins

## Troubleshooting

### Common Issues

**Issue**: "AzureCommunicationServices:ConnectionString is not configured"
- **Solution**: Ensure connection string is properly set in `appsettings.json`

**Issue**: Email sending fails with "Unauthorized"
- **Solution**: Verify connection string is correct and not expired

**Issue**: Email not received
- **Solution**: 
  - Check spam folder
  - Verify sender domain is properly configured
  - Check Azure portal for email delivery status
  - For Azure managed domains, recipient may need to be verified during development

**Issue**: "Invalid sender address"
- **Solution**: Ensure sender address matches a verified domain in Azure Communication Services

## Monitoring and Logging

### View Logs

The API uses structured logging. Check console output or configure file logging:

```csharp
builder.Logging.AddFile("logs/email-api-{Date}.txt");
```

### Azure Monitor Integration

To monitor email delivery in Azure:

1. Go to Azure Portal → Communication Services resource
2. Navigate to **"Monitoring"** → **"Metrics"**
3. Add metrics:
   - Email sends
   - Email delivery status
   - Failed deliveries

## Future Enhancements

- [ ] HTML email support
- [ ] Email templates
- [ ] Attachment support
- [ ] Bulk email sending
- [ ] Email queue management
- [ ] Retry logic for failed sends
- [ ] Authentication/Authorization
- [ ] Rate limiting

## License

This project is intended for educational purposes.

## See Also

- [Azure Communication Services Documentation](https://learn.microsoft.com/en-us/azure/communication-services/)
- [Azure Communication Services Email SDK for .NET](https://learn.microsoft.com/en-us/dotnet/api/overview/azure/communication.email-readme)
- [Email Quickstart Guide](https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/email/send-email)
