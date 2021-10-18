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

WriteLine("Start App");
var wait = new ManualResetEvent(false);
var _url = "";
var _topic = "dev.to/topic/json";
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
    .WithTcpServer(_url, port);
var option = new ManagedMqttClientOptionsBuilder().WithAutoReconnectDelay(TimeSpan.FromSeconds(50)).WithClientOptions(builder.Build()).Build();
var client = new MqttFactory().CreateManagedMqttClient();
client.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnConnected);
client.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(OnConnectingFailed);
client.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnDisconnected);
 client.StartAsync(option).GetAwaiter().GetResult();
client.UseApplicationMessageReceivedHandler(e =>
{
    WriteLine(e.ApplicationMessage.Topic);
});
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
    WriteLine("Couldn't connect to broker.");

}

static void OnDisconnected(MqttClientDisconnectedEventArgs obj)
{
    WriteLine("Successfully disconnected.");
}