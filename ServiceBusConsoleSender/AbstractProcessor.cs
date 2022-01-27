using Azure.Messaging.ServiceBus;
using Hero.Contracts;
using Newtonsoft.Json;
using System.Text;

namespace MessageBusDemo.Sender
{
    internal class AbstractProcessor
    {
        internal virtual ServiceBusMessage BuildMessage(string info, string sessionId)
        {
            var messagebody = new MessageInfo() { Message = info };
            var json = JsonConvert.SerializeObject(messagebody);
            ServiceBusMessage message = new ServiceBusMessage(Encoding.UTF8.GetBytes(json))
            {
                ContentType = "Application/json",
                SessionId = sessionId
            };
            return message;
        }
    }
}