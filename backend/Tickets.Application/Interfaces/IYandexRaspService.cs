using Tickets.Domain.Models;

namespace Tickets.Infrastructure.Services
{
    public interface IYandexRaspService
    {
        Task<RaspResponse?> GetScheduleAsync(string fromCode, string toCode, DateTime date, CancellationToken cancellation = default);
        Task<List<Station>> GetStationsAsync(CancellationToken cancellation = default);
    }
}