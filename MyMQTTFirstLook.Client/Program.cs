using System;
using MQTTnet;
using System.Text;
using static System.Console;
using MQTTnet.Client.Options;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Extensions.ManagedClient;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using SharedItems;
using System.IO;

WriteLine("Start App");
var wait = new ManualResetEvent(false);
var _url = "";
var _topic = "dev.to/topic/json";

List<X509Certificate> certs = new List<X509Certificate>
{
    ConfigCertificate.GetClientCrtCertificate()
};
do
{
    Write("Site Address:");
     _url = ReadLine();
} while (string.IsNullOrWhiteSpace(_url));
var port = 8200;
do
{
    Write("Port number:");

} while (!int.TryParse(ReadLine(), out port));
WriteLine("Enter topic( if empty use defualt topic):");
var _tempTopic = ReadLine();
if (string.IsNullOrWhiteSpace(_tempTopic))
{
    WriteLine("use defualt topic: " + _topic);
}
else
    _topic = _tempTopic;
var builder = new MqttClientOptionsBuilder()

    .WithClientId(Guid.NewGuid().ToString())
    .WithTls(c=> {
        c.UseTls = true;
        c.SslProtocol = System.Security.Authentication.SslProtocols.Tls12;
        c.Certificates = certs;

        c.CertificateValidationHandler = (certContext) =>
        {
            X509Chain chain = new X509Chain();
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
            chain.ChainPolicy.CustomTrustStore.Add(ConfigCertificate.GetCACrtCertificate());
            chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;

            // convert provided X509Certificate to X509Certificate2
            var x5092 = new X509Certificate2(certContext.Certificate);

            return chain.Build(x5092);
        };
    })
    .WithTcpServer(_url, port);
var option = new ManagedMqttClientOptionsBuilder().WithAutoReconnectDelay(TimeSpan.FromSeconds(50)).WithClientOptions(builder.Build()).Build();
var client = new MqttFactory().CreateManagedMqttClient();
client.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnConnected);
client.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(OnConnectingFailed);
client.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnDisconnected);

client.StartAsync(option).GetAwaiter().GetResult();
client.UseApplicationMessageReceivedHandler(context =>
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
});
await client.SubscribeAsync("ExampleController/publish/test");
while (true)
{
    if (!client.IsConnected)
        continue;
    string json = JsonSerializer.Serialize(new {id=Guid.NewGuid(), message = "Heyo :)", sent = DateTimeOffset.UtcNow });
    await client.PublishAsync(_topic, json);
    WriteLine("Send Data");
    await Task.Delay(1000);
}
static void OnConnected(MqttClientConnectedEventArgs obj)
{

    WriteLine("Successfully connected.");
}

static void OnConnectingFailed(ManagedProcessFailedEventArgs obj)
{
    
    WriteLine("Couldn't connect to broker."+obj.Exception.Message);

}

static void OnDisconnected(MqttClientDisconnectedEventArgs obj)
{
    
    WriteLine("Successfully disconnected. "+ obj?.Exception.Message??"");
}