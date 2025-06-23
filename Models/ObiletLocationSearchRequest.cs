using Newtonsoft.Json;

namespace ObiletBusApp.Models
{
    public class ObiletDeviceSession
    {
        [JsonProperty("session-id")]
        public string? SessionId { get; set; }
        [JsonProperty("device-id")]
        public string? DeviceId { get; set; }
    }

    public class ObiletLocationSearchRequest
    {
        [JsonProperty("data")]
        public string Data { get; set; }
        [JsonProperty("device-session")]
        public ObiletDeviceSession DeviceSession { get; set; }
        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("language")]
        public string Language { get; set; }
    }
} 