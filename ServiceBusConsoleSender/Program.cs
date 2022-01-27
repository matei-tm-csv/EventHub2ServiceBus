using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MessageBusDemo.Sender
{
    internal static class Program
    {
        private static ServiceBusSender? _messageSender;

        static async Task Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args).Build();

            Console.WriteLine("ServiceBus Sender Test Console");
            Console.WriteLine();

            IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

            // Get values from the config given their key and their target type.
            string connectionString = config.GetValue<string>("ServiceBus:ConnectionString");
            string queueName = config.GetValue<string>("ServiceBus:Name");

            var client = new ServiceBusClient(connectionString);
            _messageSender = client.CreateSender(queueName);

            var nonbatchProcessor = new NonbatchProcessor();
            var batchProcessor = new BatchProcessor();

            while (true)
            {
                Console.WriteLine("Choose an option [3n]onbatched/[100n]onbatched/[3b]atched/[100b]atched/e[x]it/[h]elp");

                var messageType = Console.ReadLine()?.ToLower();

                if (messageType == "exit" || messageType == "x")
                {
                    break;
                }

                switch (messageType)
                {
                    case "3n":
                        await nonbatchProcessor.SendDemoMessages(_messageSender, 3);
                        break;
                    case "100n":
                        await nonbatchProcessor.SendDemoMessages(_messageSender, 100);
                        break;
                    case "3b":
                        await batchProcessor.SendDemoMessages(_messageSender, 3);
                        break;
                    case "100b":
                        await batchProcessor.SendDemoMessages(_messageSender, 100);
                        break;
                    case "h":
                    case "help":
                        ShowHelpInfo();
                        break;
                    default:
                        Console.WriteLine("What?");
                        break;
                }
            }

            await _messageSender.CloseAsync();
            await _messageSender.DisposeAsync();
            await client.DisposeAsync();
            await host.RunAsync();
        }


        private static void ShowHelpInfo()
        {
            Console.WriteLine(@"Service Bus sender with batches"
            );
        }


    }
}
