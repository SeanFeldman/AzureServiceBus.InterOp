using NServiceBus;

namespace Shared
{
    public class Message1 : ICommand
    {
        public string Property { get; set; }
    }
}