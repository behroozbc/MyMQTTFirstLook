using System;
using Serilog;
using MQTTnet;
using System.Text;
using static System.Console;
using static Serilog.Log;
using MQTTnet.Client.Options;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System.Text.Json;
using System.Threading.Tasks;

WriteLine("Hello World!");
var builder = new MqttClientOptionsBuilder()
    .WithClientId(Guid.NewGuid().ToString())
    .WithTcpServer("localhost", 707);
var option = new ManagedMqttClientOptionsBuilder().WithAutoReconnectDelay(TimeSpan.FromSeconds(50)).WithClientOptions(builder.Build()).Build();
var client = new MqttFactory().CreateManagedMqttClient();
client.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnConnected);
client.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(OnConnectingFailed);
client.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnDisconnected);
await client.StartAsync(option);
while (true)
{
    string json = JsonSerializer.Serialize(new { message = "Heyo :)", sent = DateTimeOffset.UtcNow });
    await client.PublishAsync("dev.to/topic/json", json);
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