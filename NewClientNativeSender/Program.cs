using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using NewClientNativeSender;
using Newtonsoft.Json;
using Shared;

namespace NewClientNativeReceiver
{
    public class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureServiceBus.ConnectionString");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("Could not read the 'AzureServiceBus.ConnectionString' environment variable. Check the sample prerequisites.");
            }

            var queueClient = new QueueClient(connectionString, "wire-compat/asb-standard");
            queueClient.RegisterMessageHandler(
                async (message, token) =>
                {
                    var msg = message.As<Message1>();
                    Console.WriteLine(msg.Property);
                    await queueClient.CompleteAsync(message.SystemProperties.LockToken);

                    await SendMessages(connectionString, message.NServiceBusReplyToAddress(), message.NServiceBusTransportEncoding());
                }, 
                new MessageHandlerOptions(exc =>
                {
                    Console.WriteLine(exc.ExceptionReceivedContext.Action);
                    Console.WriteLine(exc.Exception.Message);
                    return Task.FromResult(0);
                })
                {
                    AutoComplete = false, MaxConcurrentCalls = 1
                });

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            await queueClient.CloseAsync();
        }

        private static async Task SendMessages(string connectionString, string replyTo, string transportEncoding)
        {
            var client1 = new QueueClient(connectionString, "wire-compat/" + replyTo);

            var reply = new Message1Reply {Text = "Reply from wire-compat/asb-standard"};
            var replyAsString = JsonConvert.SerializeObject(reply);
            var message = new Message(Encoding.UTF8.GetBytes(replyAsString));
            message.UserProperties["NServiceBus.EnclosedMessageTypes"] = "Shared.Message1Reply";
            message.UserProperties["NServiceBus.Transport.Encoding"] = transportEncoding;

            await client1.SendAsync(message);
            await client1.CloseAsync();
            Console.WriteLine($"Reply message sent to {replyTo}.");
        }
    }
}
