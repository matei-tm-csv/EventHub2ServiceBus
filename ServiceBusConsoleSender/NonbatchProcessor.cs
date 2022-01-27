using System.Text;
using Azure.Messaging.ServiceBus;
using Hero.Contracts;
using Newtonsoft.Json;

namespace MessageBusDemo.Sender
{
    internal class NonbatchProcessor: AbstractProcessor
    {
        internal async Task SendDemoMessages(ServiceBusSender _messageSender, int numberOfMessages)
        {

            var messageList = new List<ServiceBusMessage>();
            var sessionId = Guid.NewGuid().ToString();
            Console.WriteLine($"Created session: { sessionId }, Time:{DateTime.Now.ToLongTimeString()}");

            try
            {
                for (int i = 0; i < numberOfMessages; i++)
                {
                    AddMessage(messageList, string.Format("n message#{0:d}", i), sessionId);
                }

                Console.WriteLine($"Created Messages: { numberOfMessages }");

                await _messageSender.SendMessagesAsync(messageList);
                Console.WriteLine("A batch of messages has been published");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void AddMessage(List<ServiceBusMessage> messages, string info, string sessionId)
        {
            ServiceBusMessage message = BuildMessage(info, sessionId);
            messages.Add(message);
        }
    }
}
