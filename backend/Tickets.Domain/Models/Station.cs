namespace Tickets.Domain.Models
{
    public class Station
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
        public DateTime? LastUsedAt { get; set; } = null;
        public int UseCount { get; set; } = 0;
    }
}
