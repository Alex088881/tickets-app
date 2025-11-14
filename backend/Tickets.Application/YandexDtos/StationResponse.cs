using System.Text.Json.Serialization;

public class StationResponse
{
    [JsonPropertyName("countries")]
    public List<Country> Countries { get; set; } = new();
}

public class Country
{
    [JsonPropertyName("regions")]
    public List<Region> Regions { get; set; } = new();

    [JsonPropertyName("title")]
    public string Title {  get; set; } = string.Empty;
}

public class Region
{
    [JsonPropertyName("settlements")]
    public List<Settlement> Settlements { get; set; } = new();
}

public class Settlement
{
    [JsonPropertyName("stations")]
    public List<StationItem> Stations { get; set; } = new();
}

public class StationItem
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("codes")]
    public StationCodes Codes { get; set; } = new();

    [JsonPropertyName("transport_type")]
    public string TransportType { get; set; } = string.Empty;

    [JsonPropertyName("station_type")]
    public string StationType { get; set; } = string.Empty;
}

public class StationCodes
{
    [JsonPropertyName("yandex_code")]
    public string YandexCode { get; set; } = string.Empty;
}
