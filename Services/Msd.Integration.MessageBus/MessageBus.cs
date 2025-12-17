using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace Msd.Integration.MessageBus
{
    public class MessageBus : IMessageBus
    {
        private string connectionString = "CHANGE_THIS_SECRET_KEY";

        public async Task PublishMessage(object message, string topicEueueName)
        {
            await using var client = new ServiceBusClient(connectionString);

            ServiceBusSender sender = client.CreateSender(topicEueueName);

            var jsonMessage = JsonConvert.SerializeObject(message);
            ServiceBusMessage finalMessage = new (Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString(),
            };

            await sender.SendMessageAsync(finalMessage);
            await client.DisposeAsync();
        }
    }
}
