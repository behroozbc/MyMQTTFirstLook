using MQTTnet.Server;
using System;
using System.Text;
using static System.Console;
namespace SharedItems
{
    public class LogMessages
    {

        public static void OnNewConnection(MqttConnectionValidatorContext context)
        {
            WriteLine(
                    "New connection: ClientId = {0}, Endpoint = {1}",
                    context.ClientId,
                    context.Endpoint);
        }

        public static void OnNewMessage(MqttApplicationMessageInterceptorContext context)
        {
            var payload = context.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(context.ApplicationMessage?.Payload);


            WriteLine(
                " TimeStamp: {0} -- Message: ClientId = {1}, Topic = {2}, Payload = {3}, QoS = {4}, Retain-Flag = {5}",

                DateTime.Now,
                context.ClientId,
                context.ApplicationMessage?.Topic,
                payload,
                context.ApplicationMessage?.QualityOfServiceLevel,
                context.ApplicationMessage?.Retain);

        }
    }
}
