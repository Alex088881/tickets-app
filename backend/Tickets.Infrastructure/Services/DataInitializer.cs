

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Tickets.Application;
using Tickets.Application.Interfaces;
using Tickets.Infrastructure.Data;

namespace Tickets.Infrastructure.Services
{
    public class DataInitializer : IDataInitializer
    {
        public bool IsInitialized { get; private set; }
        public bool IsInitializing { get; private set; }

        private readonly AppDbContext _db;
        private readonly IStationService _stationService;
        private readonly IConfiguration _configuration;

        public DataInitializer(AppDbContext db, IStationService stationService, IConfiguration configuration)
        {
            _db = db;
            _stationService = stationService;
            _configuration = configuration;
        }
        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            IsInitializing = true;

            try
            {
                
                await _db.Database.MigrateAsync(cancellationToken);

                bool.TryParse(_configuration.GetSection("DataInitialization:AutoInitialize").Value
                    , out bool autoInitialize);
                if (autoInitialize)
                {
                    
                    var hasStations = await _db.Stations.AnyAsync(cancellationToken);

                    if (!hasStations)
                    {
                        await _stationService.LoadStationsAsync(cancellationToken);
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception();
            }
            finally { IsInitializing = false; }
            
        }
    }
}
