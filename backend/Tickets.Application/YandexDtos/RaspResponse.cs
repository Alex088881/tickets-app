
using System.Text.Json.Serialization;

namespace Tickets.Domain.Models
{
    public class RaspResponse
    {
        [JsonPropertyName("segments")]
        public List<Segment> Segments { get; set; } = new();
    }

    public class Segment
    {
        [JsonPropertyName("arrival")]
        public string Arrival { get; set; } = string.Empty;

        [JsonPropertyName("departure")]
        public string Departure { get; set; } = string.Empty;

        //[JsonPropertyName("has_transfers")]
        //public bool Has_transfers { get; set; } = false;

        [JsonPropertyName("thread")]
        public Thread? Thread { get; set; }
    }

    public class Thread
    {
        [JsonPropertyName("number")]
        public string Number { get; set; } = string.Empty;

        //[JsonPropertyName("transport_type")]
        //public string Transport_type { get; set; } = string.Empty; //train || suburban
    }
}
