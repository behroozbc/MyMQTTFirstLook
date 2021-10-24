using MQTTnet.Server;
using System;
using MQTTnet;
using System.Text;
using static System.Console;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using SharedItems;

var wait = new ManualResetEvent(false);

var option = new MqttServerOptionsBuilder()
    .WithEncryptedEndpoint()
    .WithEncryptedEndpointPort(8201)
    .WithEncryptionCertificate(ConfigCertificate.GetCrtWithPriavteKey().Export(X509ContentType.Pfx))
    .WithEncryptionSslProtocol(System.Security.Authentication.SslProtocols.Tls12)
    .WithConnectionValidator(OnNewConnection)
    .WithApplicationMessageInterceptor(OnNewMessage);
var mqttServer = new MqttFactory().CreateMqttServer();
await mqttServer.StartAsync(option.Build());
wait.WaitOne();
await mqttServer.StopAsync();
 static void OnNewConnection(MqttConnectionValidatorContext context)
{
    WriteLine(
            "New connection: ClientId = {0}, Endpoint = {1}",
            context.ClientId,
            context.Endpoint);
}

 static void OnNewMessage(MqttApplicationMessageInterceptorContext context)
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