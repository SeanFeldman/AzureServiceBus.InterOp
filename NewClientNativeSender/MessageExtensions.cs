using System;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.InteropExtensions;
using Newtonsoft.Json;

namespace NewClientNativeSender
{
    public static class MessageExtensions
    {
        static string BOM = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

        public static T As<T>(this Message message)
        {
            var encoding = message.NServiceBusTransportEncoding();

            Func<Message, byte[]> byteArrayFunc;

            switch (encoding)
            {
                case "application/octect-stream":
                    byteArrayFunc = msg => msg.Body;
                    break;

                case "wcf/byte-array":
                    byteArrayFunc = msg => msg.GetBody<byte[]>();
                    break;

                default:
                    throw new Exception($"Unknown NServiceBus.Transport.Encoding of type '{encoding}'.");
            }

            var bodyWithoutBom = Encoding.UTF8.GetString(byteArrayFunc(message)).Replace(BOM, string.Empty);
            return JsonConvert.DeserializeObject<T>(bodyWithoutBom);
        }

        public static string NServiceBusReplyToAddress(this Message message)
        {
            return ((string)message.UserProperties["NServiceBus.ReplyToAddress"]).Split('@')[0];
        }
        public static string NServiceBusTransportEncoding(this Message message)
        {
            if (message.UserProperties.TryGetValue("NServiceBus.Transport.Encoding", out var encoding))
                return (string)encoding;

            return string.Empty;
        }
    }
}