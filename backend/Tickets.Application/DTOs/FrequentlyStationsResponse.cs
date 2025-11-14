

using System.ComponentModel.DataAnnotations;

namespace Tickets.Application.DTOs
{
    public record FrequentlyStationsResponse([Required] List<string> Titles);
}
