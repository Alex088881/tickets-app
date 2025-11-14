using Tickets.Domain.Models;

namespace Tickets.Infrastructure.Repositories
{
    public interface IStationRepository
    {
        Task LoadStationsToDb(List<Station> stations, CancellationToken cancellation = default);
        Task<List<Station>> SearchStations(string name, CancellationToken cancellation = default);
        Task<Station?> GetStationByName(string name, CancellationToken cancellation = default);

        Task SetDefaultStation(Station station, CancellationToken cancellation = default);
        Task<Station?> GetDefaultStationAsync(CancellationToken cancellation = default);
        Task MarkStationAsUsedAsync(Station station, CancellationToken cancellation = default);
        Task<List<Station>> GetFrequentlyUsedStationsAsync(int count = 10, CancellationToken cancellation = default);
    }
}