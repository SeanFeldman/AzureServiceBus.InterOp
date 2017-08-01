using System;
using System.Threading.Tasks;
using NServiceBus;
using Shared;

namespace OldClientSendingByteArray
{
    class Message1ReplyHandler : IHandleMessages<Message1Reply>
    {
        public Task Handle(Message1Reply message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Received reply message sent by ASB new client. Body: {message.Text}");
            return Task.FromResult(0);
        }
    }
}
