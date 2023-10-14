using AnalyticsService;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using MQTTnet.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

var sensorDummyTopic = "sensor_dummy/values";
var eKuiperTopic = "eKuiper/anomalies"; // broker.emqx.io
string address = "10.66.100.154";
var port = 1883;
var client = InfluxDBClientFactory.Create(url: "http://10.66.100.154:8086", "admin", "adminadmin".ToCharArray());
int i = 1;

var mqttService = MqttService.Instance();

await mqttService.ConnectAsync(address, port);
await mqttService.SubsribeToTopicsAsync(new List<string> { sensorDummyTopic, eKuiperTopic });
async Task ApplicationMessageRecievedAsync(MqttApplicationMessageReceivedEventArgs e)
{
    string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
    if(e.ApplicationMessage.Topic == sensorDummyTopic)
    {
        mqttService.PublishMessage("analytics/values", payload);
        return;
    }
    Console.WriteLine($"eKuiper send: {payload}");
    var data = (JObject)JsonConvert.DeserializeObject(payload);
    Console.WriteLine($"Podaci sa eKuiper-a: {data}");
    string frameNumber = data.SelectToken("frame_number").Value<string>();
    string frameTime= data.SelectToken("frame_time").Value<string>();
    string frameLen= data.SelectToken("frame_len").Value<string>();
    string ethSrc= data.SelectToken("eth_src").Value<string>();
    string ethDst= data.SelectToken("eth_dst").Value<string>();
    string ipSrc = data.SelectToken("ip_src").Value<string>();
    string ipDst = data.SelectToken("ip_dst").Value<string>();
    string ipProto = data.SelectToken("ip_proto").Value<string>();
    string ipLen = data.SelectToken("ip_len").Value<string>();
    string tcpLen = data.SelectToken("tcp_len").Value<string>();
    string tcpSrcport = data.SelectToken("tcp_srcport").Value<string>();
    string tcpDstport = data.SelectToken("tcp_dstport").Value<string>();
    string value = data.SelectToken("Value").Value<string>();
    string normality = data.SelectToken("normality").Value<string>();
    // string ID = data.SelectToken("ID").Value<string>();
    // string timestamp = data.SelectToken("timestamp").Value<string>();
    // string temperature = data.SelectToken("temperature").Value<string>();
    // string humidity = data.SelectToken("humidity").Value<string>();
    Console.WriteLine($"Write in InfluxDb check 1, ovo je frame number: {frameNumber}");
    await WriteToDatabase(frameNumber, frameLen, frameTime, ethSrc, ethDst, ipSrc, ipDst, ipProto, ipLen, tcpLen, tcpSrcport, tcpDstport, value, normality);
    // await WriteToDatabase(ID, timestamp, temperature, humidity);
}

async Task WriteToDatabase(string frameNumber, string frameLen, string frameTime, string ethSrc, string ethDst, string ipSrc, string ipDst, string ipProto, string ipLen, string tcpLen, string tcpSrcport, string tcpDstport, string value, string normality)
// async Task WriteToDatabase(string ID, string timestamp, string temperature, string humidity)
{
    var point = PointData
        .Measurement("network")
        .Tag("frame_number", frameNumber.ToString())
        .Field("frame_len", frameLen)
        .Field("frame_time", frameTime)
        .Field("eth_src", ethSrc)
        .Field("eth_dst", ethDst)
        .Field("ip_src", ipSrc)
        .Field("ip_dst", ipDst)
        .Field("ip_proto", ipProto)
        .Field("ip_len", ipLen)
        .Field("tcp_len", tcpLen)
        .Field("tcp_srcport", tcpSrcport)
        .Field("tcp_dstport", tcpDstport)
        .Field("value", value)
        .Field("normality", normality)
        // .Tag("timestamp", timestamp.ToString())
        // // .Field("id", ID)
        // .Field("temperature", temperature)
        // .Field("humidity", humidity)
        .Timestamp(DateTime.UtcNow, WritePrecision.Ns);
    Console.WriteLine($"Write in InfluxDb CHECK");

    await client.GetWriteApiAsync().WritePointAsync(point, "iot2", "organization");
    Console.WriteLine($"Write in InfluxDb: log{i}");
    i++;
}

mqttService.AddApplicationMessageReceived(ApplicationMessageRecievedAsync);

while(true);