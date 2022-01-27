using System.Text;
using Azure.Messaging.ServiceBus;
using Hero.Contracts;
using Newtonsoft.Json;

namespace MessageBusDemo.Sender
{
    internal class BatchProcessor:AbstractProcessor
    {
        internal async Task SendDemoMessages(ServiceBusSender _messageSender, int numberOfMessages)
        {

            using ServiceBusMessageBatch messageBatch = await _messageSender.CreateMessageBatchAsync();
            var sessionId = Guid.NewGuid().ToString();
            Console.WriteLine($"Created session: { sessionId }, Time:{DateTime.Now.ToLongTimeString()}");

            try
            {
                for (int i = 0; i < numberOfMessages; i++)
                {
                    AddMessage(messageBatch, string.Format("b message#{0:d}",i), sessionId);
                }

                Console.WriteLine($"Created Messages: { numberOfMessages }");

                await _messageSender.SendMessagesAsync(messageBatch);
                Console.WriteLine("A batch of messages has been published");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void AddMessage(ServiceBusMessageBatch messages, string info, string sessionId)
        {
            ServiceBusMessage message = BuildMessage(info, sessionId);
            messages.TryAddMessage(message);
        }
    }
}
