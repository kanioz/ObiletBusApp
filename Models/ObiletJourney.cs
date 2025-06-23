using Newtonsoft.Json;

namespace ObiletBusApp.Models
{
    public class ObiletJourneyResponse
    {
        [JsonProperty("data")]
        public List<ObiletJourney>? Data { get; set; }
    }

    public class ObiletJourney
    {
        [JsonProperty("journey")]
        public JourneyInfo? Journey { get; set; }

        [JsonProperty("feature")]
        public FeatureInfo? Feature { get; set; }
    }

    public class JourneyInfo
    {
        [JsonProperty("origin")]
        public string? Origin { get; set; }

        [JsonProperty("destination")]
        public string? Destination { get; set; }

        [JsonProperty("departure")]
        public DateTime Departure { get; set; }

        [JsonProperty("arrival")]
        public DateTime Arrival { get; set; }

        [JsonProperty("internet-price")]
        public decimal? InternetPrice { get; set; }
    }

    public class FeatureInfo
    {
        [JsonProperty("internet-price")]
        public decimal? Price { get; set; }
    }
} 