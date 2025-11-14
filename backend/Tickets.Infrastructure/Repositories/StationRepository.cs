

using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Tickets.Domain.Models;
using Tickets.Infrastructure.Data;

namespace Tickets.Infrastructure.Repositories
{
    public class StationRepository : IStationRepository
    {
        private readonly AppDbContext _db;

        public StationRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task LoadStationsToDb(List<Station> stations, CancellationToken cancellation = default)
        {
            await _db.Database.ExecuteSqlRawAsync("DELETE FROM Stations; VACUUM;", cancellation);
            await _db.SaveChangesAsync(cancellation);

            // Быстрая вставка (BulkInsert)
            await _db.BulkInsertAsync(stations);
        }

        public async Task<List<Station>> SearchStations(string name, CancellationToken cancellation = default)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length <=2)
                return new List<Station>();

            var result = await _db.Stations
               .Where(s => EF.Functions.Like(s.Title, $"{name.ToLower()}%"))
               .Take(20)
               .ToListAsync(cancellation);

            if (result.Count == 0)
            {
               result =  await _db.Stations
                    .Where(s => s.Title.Contains(name.ToLower()))
                    .Take(20)
                    .ToListAsync(cancellation);
            }
            return result;
        }

        public async Task<Station?> GetStationByName(string name, CancellationToken cancellation = default)
        {
            var result = await _db.Stations.FirstOrDefaultAsync(s => s.Title == name.ToLower());
            return result;
        }

        public async Task SetDefaultStation(Station station, CancellationToken cancellation = default)
        {
            await _db.Stations
                .Where(s => s.IsDefault)
                .ExecuteUpdateAsync(setters => setters.SetProperty(s =>s.IsDefault, false), cancellation);

            station.IsDefault = true;
            _db.Update(station);
            await _db.SaveChangesAsync(cancellation);
        }

        public async Task<Station?> GetDefaultStationAsync(CancellationToken cancellation = default)
        {
            var result = await _db.Stations
                .FirstOrDefaultAsync(s => s.IsDefault == true);
            return result;
        }

        public async Task MarkStationAsUsedAsync(Station station, CancellationToken cancellation = default)
        {
            station.LastUsedAt = DateTime.UtcNow;
            station.UseCount++;
            await _db.SaveChangesAsync(cancellation);
        }

        public async Task<List<Station>> GetFrequentlyUsedStationsAsync(int count = 10, CancellationToken cancellation = default)
        {
            return await _db.Stations
                .Where(s => s.LastUsedAt != null && !s.IsDefault) 
                .OrderByDescending(s => s.LastUsedAt) 
                .ThenByDescending(s => s.UseCount) 
                .Take(count)
                .ToListAsync(cancellation) ?? new List<Station>();
        }
    }
}
