using System;
using Hero.Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace QlikServiceBusConsumer
{
    public static class ServiceBusConsumerFunction
    {
        [FunctionName("ServiceBusConsumerFunction")]
        public static void Run([ServiceBusTrigger("q-two", IsSessionsEnabled =true, Connection = "AzureQlikServiceBus")] MessageInfo[] myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function messages count is: {myQueueItem.Length}");
            foreach (var item in myQueueItem)
            {
                log.LogInformation($"C# ServiceBus queue trigger function processed message: {item.Message}");
            }
        }
    }
}
