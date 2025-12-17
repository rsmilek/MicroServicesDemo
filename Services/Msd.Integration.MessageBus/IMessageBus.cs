using Msd.Integration.MessageBus.Models.Dtos;

namespace Msd.Integration.MessageBus
{
    public interface IMessageBus
    {
        Task PublishMessage(object message, string topicEueueName);
        Task PublishEmail(SendEmailRequestDto email, string topicEueueName);
    }
}
