using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Transport.AzureServiceBus;
using Shared;

namespace OldClientSendingByteArray
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            Console.Title = "asb-endpoint-on-byte-array";


            var endpointConfiguration = new EndpointConfiguration("endpoint-on-byte-array");
            endpointConfiguration.SendFailedMessagesTo("error");
            var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
            var connectionString = Environment.GetEnvironmentVariable("AzureServiceBus.ConnectionString");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("Could not read the 'AzureServiceBus.ConnectionString' environment variable. Check the sample prerequisites.");
            }
            transport.ConnectionString(connectionString);
            transport.UseForwardingTopology();

            transport.BrokeredMessageBodyType(SupportedBrokeredMessageBodyTypes.ByteArray);
            transport.Composition().UseStrategy<HierarchyComposition>().PathGenerator(path => "wire-compat/");

            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.EnableInstallers();
            var recoverability = endpointConfiguration.Recoverability();
            recoverability.DisableLegacyRetriesSatellite();

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);
            Console.WriteLine("Press 'enter' to send a message");
            Console.WriteLine("Press any other key to exit");

            while (true)
            {
                var key = Console.ReadKey();
                Console.WriteLine();

                if (key.Key != ConsoleKey.Enter)
                {
                    break;
                }

                var message = new Message1
                {
                    Property = "Hello from asb-endpoint-on-byte-array"
                };
                await endpointInstance.Send("asb-standard", message)
                    .ConfigureAwait(false);
                Console.WriteLine("Message1 sent");
            }
            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}
