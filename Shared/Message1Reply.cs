using NServiceBus;

namespace Shared
{
    public class Message1Reply : IMessage
    {
        public string Text { get; set; }
    }
}
