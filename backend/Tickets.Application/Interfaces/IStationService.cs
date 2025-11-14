
using Tickets.Application.DTOs;
using Tickets.Domain.Models;
using Tickets.Infrastructure.Repositories;

namespace Tickets.Application
{
    public interface IStationService
    {
        Task<int> LoadStationsAsync(CancellationToken cancellation = default);
        Task<List<Station>> SearchStationAsync(string name, CancellationToken cancellation = default);
        Task<DefaultStationResponce?> GetDefaultStationAsync(CancellationToken cancellation = default);
        Task<DefaultStationResponce?> SetDefaultStationAsync(string name, CancellationToken cancellation = default);
        Task<FrequentlyStationsResponse> GetFrequentlyUsedStationsAsync();
    }
}