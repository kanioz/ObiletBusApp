using Newtonsoft.Json;

namespace ObiletBusApp.Models;

public class ObiletJourneySearchData
{
    [JsonProperty("origin-id")]
    public int OriginId { get; set; }

    [JsonProperty("destination-id")]
    public int DestinationId { get; set; }

    [JsonProperty("departure-date")]
    public string DepartureDate { get; set; }
}

public class ObiletJourneySearchRequest
{
    [JsonProperty("device-session")]
    public ObiletDeviceSession DeviceSession { get; set; }

    [JsonProperty("date")]
    public string Date { get; set; }

    [JsonProperty("language")]
    public string Language { get; set; }

    [JsonProperty("data")]
    public ObiletJourneySearchData Data { get; set; }
}