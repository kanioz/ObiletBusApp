using Newtonsoft.Json;

namespace ObiletBusApp.Models
{
    public class ObiletSession
    {
        [JsonProperty("session-id")]
        public string? SessionId { get; set; }

        [JsonProperty("device-id")]
        public string? DeviceId { get; set; }
    }

    public class ObiletSessionResponse
    {
        [JsonProperty("data")]
        public ObiletSessionData? Data { get; set; }
    }

    public class ObiletSessionData
    {
        [JsonProperty("session-id")]
        public string? SessionId { get; set; }

        [JsonProperty("device-id")]
        public string? DeviceId { get; set; }
    }
} 