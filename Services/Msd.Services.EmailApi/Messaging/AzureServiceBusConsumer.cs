using Azure.Messaging.ServiceBus;
using Msd.Services.EmailApi.Models.Dtos;
using Msd.Services.EmailApi.Services;
using Newtonsoft.Json;
using System.Text;

namespace Msd.Services.EmailApi.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly EmailService _emailService;
        private readonly string serviceBusConnectionString;
        private readonly string registerUserQueue;
        private ServiceBusProcessor _registerUserProcessor;

        public AzureServiceBusConsumer(
            IConfiguration configuration, 
            ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _emailService = new EmailService(_configuration, _logger);

            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString")
                ?? throw new Exception("ServiceBusConnectionString isn't defined!");

            registerUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue")
                ?? throw new Exception("TopicAndQueueNames:RegisterUserQueue isn't defined!");

            var client = new ServiceBusClient(serviceBusConnectionString);
            _registerUserProcessor = client.CreateProcessor(registerUserQueue);
        }

        public async Task Start()
        {
            _registerUserProcessor.ProcessMessageAsync += OnUserRegisterRequestReceived;
            _registerUserProcessor.ProcessErrorAsync += ErrorHandler;
            await _registerUserProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _registerUserProcessor.StopProcessingAsync();
            await _registerUserProcessor.DisposeAsync();
        }

        private async Task OnUserRegisterRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            string email = JsonConvert.DeserializeObject<string>(body) 
                ?? throw new Exception("Invalid email address!");
            try
            {
                await _emailService.SendEmailAsync(new SendEmailRequestDto
                { 
                    To = email,
                    Subject = "Registration",
                    Body = $"User {email} has been registered successfully!"
                });
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
       
    }
}
