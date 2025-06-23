using Newtonsoft.Json;

namespace ObiletBusApp.Models
{
    public class ObiletLocationResponse
    {
        [JsonProperty("data")]
        public List<ObiletLocation>? Data { get; set; }
    }

    public class ObiletLocation
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }
    }
} 