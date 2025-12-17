using System;

namespace Msd.Integration.MessageBus
{
    public interface IMessageBus
    {
        Task PublishMessage(object message, string topicEueueName);
    }
}
