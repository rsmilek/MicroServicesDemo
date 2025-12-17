# Msd.Integration.MessageBus

.NET 10 shared library providing Azure Service Bus integration for inter-service communication in the MicroServicesDemo microservices architecture. This library abstracts Azure Service Bus operations and enables asynchronous, event-driven communication between services.

## Features

- ✅ Azure Service Bus integration
- ✅ Message publishing to topics/queues
- ✅ Email-specific message publishing
- ✅ Automatic message serialization (JSON)
- ✅ Correlation ID tracking for distributed tracing
- ✅ Configuration-based connection management
- ✅ Async/await support
- ✅ Reusable across multiple services

## Overview

This library provides a unified messaging abstraction layer for the microservices. Instead of services directly interacting with Azure Service Bus, they use the `IMessageBus` interface, enabling loose coupling and simplified message handling.

### Architecture

```
Service A (AuthApi)
    ↓
IMessageBus (Interface)
    ↓
MessageBus (Implementation)
    ↓
Azure Service Bus Topic/Queue
    ↓
Service B (EmailApi - Consumer)
```

## Requirements

- .NET 10.0 SDK or later
- Azure subscription
- Azure Service Bus namespace
- NuGet packages:
  - `Azure.Messaging.ServiceBus` (7.20.1+)
  - `Microsoft.Extensions.Configuration` (10.0.1+)
  - `Newtonsoft.Json` (13.0.4+)

## Azure Service Bus Setup

### Step 1: Create Service Bus Namespace

1. **Sign in to Azure Portal**
   - Navigate to [Azure Portal](https://portal.azure.com/)

2. **Create Service Bus Namespace**
   - Click **"Create a resource"**
   - Search for **"Service Bus"**
   - Click **"Create"**
   - Fill in details:
     - **Subscription**: Select your subscription
     - **Resource Group**: Create new or use existing
     - **Namespace Name**: Choose unique name (e.g., `msd-servicebus`)
     - **Location**: Select nearest region
     - **Pricing Tier**: Select appropriate tier (Basic, Standard, or Premium)
   - Click **"Review + Create"** → **"Create"**

3. **Get Connection String**
   - Once deployed, go to your Service Bus namespace
   - Navigate to **"Shared access policies"** in the left menu
   - Click **"RootManageSharedAccessKey"**
   - Copy the **"Primary Connection String"**
   - Save it securely - you'll need it in `appsettings.json`

### Step 2: Create Topics/Queues

Topics are used for pub/sub scenarios where multiple services listen to messages.

**Create a Topic for Email Service:**

1. In your Service Bus namespace, click **"Topics"** in the left menu
2. Click **"+ Topic"**
3. Fill in details:
   - **Name**: `email-service-topic` (or your preferred name)
   - **Max topic size**: 1 GB (default)
   - Leave other settings as default
4. Click **"Create"**

**Create Subscription for Email Service:**

1. Click on the newly created topic
2. Click **"+ Subscription"**
3. Fill in details:
   - **Name**: `email-service-subscription`
   - Leave other settings as default
4. Click **"Create"**

### Step 3: Configure Access Policies (Optional)

For more granular control, create separate access policies:

1. In your Service Bus namespace, go to **"Shared access policies"**
2. Click **"+ Add"**
3. Create policies for:
   - **Publisher** (Send permission) - for services publishing messages
   - **Consumer** (Listen permission) - for services consuming messages

## Installation and Usage

### 1. Add Reference to Your Project

In your service project, add a reference to `Msd.Integration.MessageBus`:

```bash
dotnet add reference ../Msd.Integration.MessageBus/Msd.Integration.MessageBus.csproj
```

Or manually add to `.csproj`:
```xml
<ItemGroup>
  <ProjectReference Include="../Msd.Integration.MessageBus/Msd.Integration.MessageBus.csproj" />
</ItemGroup>
```

### 2. Configure appsettings.json

Add your Service Bus connection string to `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "ServiceBusConnectionString": "Endpoint=sb://YOUR-NAMESPACE.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=YOUR-ACCESS-KEY"
  }
}
```

### 3. Register in Dependency Injection (Program.cs)

```csharp
using Msd.Integration.MessageBus;

var builder = WebApplicationBuilder.CreateBuilder(args);

// Add MessageBus service
builder.Services.AddScoped<IMessageBus, MessageBus>();

var app = builder.Build();
app.Run();
```

### 4. Use in Your Service

Inject `IMessageBus` into your service and publish messages:

```csharp
using Msd.Integration.MessageBus;
using Msd.Integration.MessageBus.Models.Dtos;

namespace Msd.Services.AuthApi.Services
{
    public class AuthService
    {
        private readonly IMessageBus _messageBus;

        public AuthService(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        public async Task SendWelcomeEmailAsync(string userEmail)
        {
            var emailMessage = new SendEmailRequestDto
            {
                To = userEmail,
                Subject = "Welcome to MicroServicesDemo",
                Body = "Thank you for registering!"
            };

            // Publish email message to Service Bus
            await _messageBus.PublishEmail(emailMessage, "email-service-topic");
        }
    }
}
```

## API Reference

### IMessageBus Interface

```csharp
public interface IMessageBus
{
    /// <summary>
    /// Publishes a generic message to a Service Bus topic/queue
    /// </summary>
    /// <param name="message">The message object to publish</param>
    /// <param name="topicQueueName">The Service Bus topic or queue name</param>
    Task PublishMessage(object message, string topicQueueName);

    /// <summary>
    /// Publishes an email message to a Service Bus topic/queue
    /// </summary>
    /// <param name="email">The email request DTO</param>
    /// <param name="topicQueueName">The Service Bus topic or queue name</param>
    Task PublishEmail(SendEmailRequestDto email, string topicQueueName);
}
```

### Publishing a Generic Message

```csharp
var customMessage = new { 
    EventType = "UserRegistered", 
    UserId = "12345",
    Timestamp = DateTime.UtcNow
};

await _messageBus.PublishMessage(customMessage, "user-events-topic");
```

### Publishing an Email Message

```csharp
var emailRequest = new SendEmailRequestDto
{
    To = "user@example.com",
    Subject = "Test Subject",
    Body = "Email body"
};

await _messageBus.PublishEmail(emailRequest, "email-service-topic");
```

## Project Structure

```
Msd.Integration.MessageBus/
├── IMessageBus.cs                   # Message bus interface
├── MessageBus.cs                    # Azure Service Bus implementation
├── Models/                          # Data models
│   └── Dtos/
│       └── SendEmailRequestDto.cs   # Email request DTO
├── Msd.Integration.MessageBus.csproj # Project file
└── README.md                        # This file
```

## How It Works

### Message Publishing Flow

1. **Service publishes message**
   ```csharp
   await _messageBus.PublishEmail(emailRequest, "email-service-topic");
   ```

2. **MessageBus serializes to JSON**
   ```
   { "to": "user@example.com", "subject": "...", "body": "..." }
   ```

3. **Message sent to Service Bus**
   - Wrapped in `ServiceBusMessage`
   - Correlation ID added for tracing

4. **Message published to topic**
   - Available to all subscribers
   - Persisted for a configurable time period

### Message Consumption

Services subscribe to topics/queues (handled separately in consumer services like `EmailApi`):

```csharp
// In EmailApi - AzureServiceBusConsumer.cs
var processor = client.CreateProcessor("email-service-topic", "email-service-subscription");

processor.ProcessMessageAsync += async args =>
{
    var body = args.Message.Body.ToString();
    var emailRequest = JsonConvert.DeserializeObject<SendEmailRequestDto>(body);
    // Process email...
};
```

## Configuration

### Service Bus Connection String

Set in `appsettings.json`:
```json
"ConnectionStrings": {
    "ServiceBusConnectionString": "Endpoint=sb://namespace.servicebus.windows.net/;..."
}
```

Or via environment variable:
```bash
export ConnectionStrings__ServiceBusConnectionString="Endpoint=sb://namespace.servicebus.windows.net/;..."
```

### Correlation ID

Each message automatically receives a unique Correlation ID (GUID) for distributed tracing:

```csharp
CorrelationId = Guid.NewGuid().ToString()
```

This helps track messages across multiple services.

## Best Practices

1. **Use appropriate topic/queue names**
   - Use descriptive, kebab-case names: `email-service-topic`, `user-events-topic`
   - Indicate service intent: `email-*`, `user-*`, etc.

2. **Handle serialization errors**
   - Ensure DTO objects are properly serializable
   - Use appropriate naming conventions

3. **Monitor message delivery**
   - Use correlation IDs for tracing
   - Implement dead-letter queue handling for failed messages
   - Monitor Service Bus metrics in Azure Portal

4. **Consider message size**
   - Keep messages lean
   - Avoid large payloads (Service Bus has limits)
   - Use references to data instead of full objects when possible

5. **Implement retry logic**
   - Consider transient failures
   - Implement exponential backoff
   - Set appropriate timeout values

6. **Security**
   - Never commit connection strings
   - Use Azure Key Vault in production
   - Use separate access policies for publishers and consumers
   - Restrict network access to Service Bus

## Troubleshooting

### Issue: "ServiceBusConnectionString isn't defined!"
- **Solution**: Ensure `appsettings.json` includes the connection string under `ConnectionStrings.ServiceBusConnectionString`

### Issue: "Topic/Queue not found"
- **Solution**: Create the topic/queue in Azure Service Bus before publishing

### Issue: Timeout errors
- **Solution**: Check network connectivity and Service Bus namespace status in Azure Portal

### Issue: Message not being received
- **Solution**: 
  - Verify subscription exists for the topic
  - Check consumer service is running
  - Review Azure Service Bus metrics

### Issue: Authentication fails
- **Solution**:
  - Verify connection string is correct
  - Check Service Bus namespace access policies
  - Ensure access key hasn't expired

## Technologies

- **Framework**: .NET 10.0
- **Service Bus**: Azure Service Bus (Cloud-hosted message broker)
- **Serialization**: Newtonsoft.Json
- **Configuration**: Microsoft.Extensions.Configuration

## Pricing

Service Bus pricing varies by tier:

- **Basic**: ~$0.05/month + $0.0015 per operation
- **Standard**: ~$10/month + $0.0015 per operation  
- **Premium**: ~$360/month + dedicated capacity

For development, Basic tier is typically sufficient.

## Integration with Other Services

This library is used by:

- **AuthApi** (`Msd.Services.AuthApi`)
  - Publishes email events when users register
  - Example: `await _messageBus.PublishEmail(emailRequest, "email-service-topic")`

- **EmailApi** (`Msd.Services.EmailApi`)
  - Consumes email messages from Service Bus
  - Sends emails via Azure Communication Services
  - Uses `AzureServiceBusConsumer` to listen for messages

## Development

### Build the library
```bash
cd Msd.Integration.MessageBus
dotnet build
```

### Run tests
```bash
dotnet test
```

### Publish to local NuGet
```bash
dotnet pack -o ../nuget-output
```

## Future Enhancements

- [ ] Async message handlers
- [ ] Dead-letter queue management
- [ ] Message retry policies
- [ ] Circuit breaker pattern
- [ ] Metrics and monitoring integration
- [ ] Message filtering/routing
- [ ] Request-reply messaging pattern

## Security Best Practices

1. **Connection String Management**
   ```bash
   # Use User Secrets in development
   dotnet user-secrets set "ConnectionStrings:ServiceBusConnectionString" "your-connection-string"
   
   # Use Key Vault in production
   # Reference: https://learn.microsoft.com/en-us/azure/key-vault/general/overview
   ```

2. **Access Control**
   - Use Shared Access Policies with minimal required permissions
   - Create separate policies for publishers and consumers
   - Rotate keys regularly

3. **Network Security**
   - Use Service Bus firewall rules
   - Consider Private Endpoints for production
   - Restrict outbound network access

## License

This project is intended for educational purposes.

## See Also

- [Azure Service Bus Documentation](https://learn.microsoft.com/en-us/azure/service-bus-messaging/)
- [Azure SDK for .NET - Service Bus](https://learn.microsoft.com/en-us/dotnet/api/overview/azure/messaging.servicebus-readme)
- [Service Bus Messaging Patterns](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-messaging-patterns-and-workflows)
- [Msd.Services.AuthApi](../Msd.Services.AuthApi/README.md)
- [Msd.Services.EmailApi](../Msd.Services.EmailApi/README.md)
