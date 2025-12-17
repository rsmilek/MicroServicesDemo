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
        private readonly string sendEmailQueueName;
        private ServiceBusProcessor _sendEmailProcessor;

        public AzureServiceBusConsumer(
            IConfiguration configuration, 
            ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _emailService = new EmailService(_configuration, _logger);

            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString")
                ?? throw new Exception("ServiceBusConnectionString isn't defined!");

            sendEmailQueueName = _configuration.GetValue<string>("TopicAndQueueNames:SendEmailQueue")
                ?? throw new Exception("TopicAndQueueNames:SendEmailQueue isn't defined!");

            var client = new ServiceBusClient(serviceBusConnectionString);
            _sendEmailProcessor = client.CreateProcessor(sendEmailQueueName);
        }

        public async Task Start()
        {
            _sendEmailProcessor.ProcessMessageAsync += OnSendEmailRequestReceived;
            _sendEmailProcessor.ProcessErrorAsync += ErrorHandler;
            await _sendEmailProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _sendEmailProcessor.StopProcessingAsync();
            await _sendEmailProcessor.DisposeAsync();
        }

        private async Task OnSendEmailRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            try
            {
                var emailMessage = JsonConvert.DeserializeObject<SendEmailRequestDto>(body)
                    ?? throw new Exception("Invalid email message!");
                await _emailService.SendEmailAsync(emailMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email message: {body}!");
                throw;
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "Error processing message: {ErrorSource}", args.ErrorSource);
            return Task.CompletedTask;
        }
       
    }
}
