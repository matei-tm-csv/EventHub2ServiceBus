using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;

using IHost host = Host.CreateDefaultBuilder(args).Build();

IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

// Get values from the config given their key and their target type.
string connectionString = config.GetValue<string>("EventHub:ConnectionString");
string eventHubName = config.GetValue<string>("EventHub:Name");

// Application code which might rely on the config could start here.



// See https://aka.ms/new-console-template for more information
Console.WriteLine("Press \"A\" to generate a new sequence of events!");
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("Press any other key to quit the application!");
Console.ResetColor();

// number of events to be sent to the event hub
int numOfEvents = 3;

EventHubProducerClient producerClient;

producerClient = new EventHubProducerClient(connectionString, eventHubName);

// Create a batch of events 
using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();



try
{
    while (Console.ReadKey().Key == ConsoleKey.A)
    {

        for (int i = 1; i <= numOfEvents; i++)
        {
            if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes($"Event {i}"))))
            {
                // if it is too large for the batch
                throw new Exception($"Event {i} is too large for the batch and cannot be sent.");
            }
        }

        // Use the producer client to send the batch of events to the event hub
        await producerClient.SendAsync(eventBatch);
        Console.WriteLine($" batch of {numOfEvents} events has been published.");

    }

    await host.RunAsync();
}
finally
{
    await producerClient.DisposeAsync();
}