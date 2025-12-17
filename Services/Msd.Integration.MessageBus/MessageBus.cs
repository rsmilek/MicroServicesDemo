using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Msd.Integration.MessageBus.Models.Dtos;
using Newtonsoft.Json;
using System.Text;

namespace Msd.Integration.MessageBus
{
    public class MessageBus : IMessageBus
    {
        private readonly IConfiguration _configuration;
        private string _connectionString;

        public MessageBus(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("ServiceBusConnectionString")
                ?? throw new Exception("ServiceBusConnectionString isn't defined!");
        }

        public async Task PublishMessage(object message, string topicEueueName)
        {
            await using var client = new ServiceBusClient(_connectionString);

            ServiceBusSender sender = client.CreateSender(topicEueueName);

            var jsonMessage = JsonConvert.SerializeObject(message);
            ServiceBusMessage finalMessage = new (Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString(),
            };

            await sender.SendMessageAsync(finalMessage);
            await client.DisposeAsync();
        }

        public async Task PublishEmail(SendEmailRequestDto email, string topicEueueName)
        {
            await PublishMessage(email, topicEueueName);
        }

    }
}
