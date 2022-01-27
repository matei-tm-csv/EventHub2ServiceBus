using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Hero.Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace EventHub2ServiceBus
{
    public class BridgeFunction
    {
        [FunctionName("BridgeFunction")]
        public async Task
            Run(
            [EventHubTrigger("qlikeventhub", Connection = "AzureQlikEventHub")] EventData[] events,
            [ServiceBus("q-one", Connection = "AzureQlikServiceBus")] IAsyncCollector<MessageInfo> collector,
            ILogger log,
            CancellationToken token)
        {
            var exceptions = new List<Exception>();


            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.ToArray(), 0, eventData.Body.ToArray().Count());
                    var newMessage = new MessageInfo() { Message = messageBody };

                    // Replace these two lines with your processing logic.

                    await collector.AddAsync(newMessage, token);
                    log.LogInformation($"C# Event Hub trigger function processed a message: {messageBody}");
                    await Task.Yield();
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();

            await collector.FlushAsync(token);
        }
    }
}
