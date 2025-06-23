using Newtonsoft.Json;

namespace ObiletBusApp.Models
{
    public class ObiletConnection
    {
        [JsonProperty("ip-address")]
        public string IpAddress { get; set; }
        [JsonProperty("port")]
        public string Port { get; set; }
    }

    public class ObiletBrowser
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
    }

    public class ObiletSessionRequest
    {
        [JsonProperty("type")]
        public int Type { get; set; }
        [JsonProperty("connection")]
        public ObiletConnection Connection { get; set; }
        [JsonProperty("browser")]
        public ObiletBrowser Browser { get; set; }
    }
} 