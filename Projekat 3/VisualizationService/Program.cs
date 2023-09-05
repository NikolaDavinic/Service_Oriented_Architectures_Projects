using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using MQTTnet.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using VisualizationService;

var port = 1883;
var address = "broker.hivemq.com";
var edgexTopic = "edgex/sensor_value";
int i = 1;

var mqttService = MqttService.Instance();

await mqttService.ConnectAsync(address, port);
var client = InfluxDBClientFactory.Create(url: "http://10.14.42.11:8086", "admin", "adminadmin".ToCharArray());
Console.WriteLine($"CLIENT is null {client == null}");
await mqttService.SubscribeToTopicsAsync(new List<string> { edgexTopic});

async Task ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
{
    try
    {
        string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
        var data = (JObject)JsonConvert.DeserializeObject(payload);
        var device = data.SelectToken("device").Value<string>();
        Console.WriteLine("Sada sam ovde");
        if (device != "SensorValueCluster2") return;
        var reading = JArray.Parse(data.SelectToken("readings").ToString())[0];
        var tempValue = reading.SelectToken("value").Value<String>();
        Console.WriteLine(tempValue);
        await WriteToDatabase(tempValue);
    }
    catch(Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

async Task WriteToDatabase(string temp)
{
    var point = PointData
        .Measurement("temperature")
        .Field("temperature", temp)
        .Timestamp(DateTime.UtcNow, WritePrecision.Ns);
        
    Console.WriteLine("Point sa podacima je napravljen");
    await client.GetWriteApiAsync().WritePointAsync(point, "iot3", "organization");
    Console.WriteLine($"Write in InfluxDB: temperature{i}");
    i++;
}

mqttService.AddApplicationMessageReceived(ApplicationMessageReceivedAsync);

while (true) ;