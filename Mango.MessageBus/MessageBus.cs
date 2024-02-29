using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public class MessageBus : IMessageBus
    {
        private string connectionString = "Endpoint=sb://mangowebvrk.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=TG/lVzou2jic0nEAs1dQui/59KcqfCf0S+ASbLBxxXk=";
        public async Task PublishMessage(object message, string topic_queue_name)
        {
            await using var client = new ServiceBusClient(connectionString);
            ServiceBusSender serviceBusSender = client.CreateSender(topic_queue_name);
            var jsonMessage = JsonConvert.SerializeObject(message);
            ServiceBusMessage serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString(),
            };
            await serviceBusSender.SendMessageAsync(serviceBusMessage);
            await client.DisposeAsync();
        }
    }
}
