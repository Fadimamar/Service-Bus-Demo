using Microsoft.Extensions.Configuration;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.ServiceBus;
using System.Text.Json;
using System.Text;

namespace SBSender.Services
{
    public class QueueService : IQueueService
    {
        private readonly IConfiguration _configuration;
        public QueueService (IConfiguration configuration)
        {
            _configuration = configuration; //inject configuration
        }
        public async Task SendMessageAsync<T> (T ServiceBusMessage, string queueName)
        {
            var queueClient = new QueueClient(_configuration.GetConnectionString("AzureServiceBus"), queueName);
            string messageBody = JsonSerializer.Serialize(ServiceBusMessage); //Convert to json string
            var message = new Message(Encoding.UTF8.GetBytes(messageBody)); //convert to byte array
            await queueClient.SendAsync(message);

        }
    }
}
