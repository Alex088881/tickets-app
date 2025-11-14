
using Microsoft.Extensions.Caching.Memory;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using Tickets.Application.DTOs;
using Tickets.Domain.Models;
using Tickets.Infrastructure.Repositories;
using Tickets.Infrastructure.Services;

namespace Tickets.Application
{
    public class StationService : IStationService
    {
        private readonly IMemoryCache _cache;
        private readonly IYandexRaspService _rasp;
        private readonly IStationRepository _stationsRepository;

        public StationService(IYandexRaspService yandexRaspService, IStationRepository ticketRepository, IMemoryCache cache)
        {
            _rasp = yandexRaspService;
            _stationsRepository = ticketRepository;
            _cache = cache;
        }

        public async Task<int> LoadStationsAsync(CancellationToken cancellation = default)
        {
            var stations = await _rasp.GetStationsAsync(cancellation);
            await _stationsRepository.LoadStationsToDb(stations, cancellation);
            return stations.Count;
        }

        public async Task<List<Station>> SearchStationAsync(string name, CancellationToken cancellation = default)
        {
            var cacheKey = $"stations_search_{name}";

            if (!_cache.TryGetValue(cacheKey, out List<Station>? stations))
            {
                stations = await _stationsRepository.SearchStations(name, cancellation);
                _cache.Set(cacheKey, stations, TimeSpan.FromMinutes(10));
            }
            return stations ?? new();
        }

        public async Task<DefaultStationResponce?> SetDefaultStationAsync(string name, CancellationToken cancellation = default)
        {
            var result = await _stationsRepository.GetStationByName(name, cancellation);
            if (result is null) return null;

            await _stationsRepository.SetDefaultStation(result);
            return new DefaultStationResponce(result.Title);

        }

        public async Task<DefaultStationResponce?> GetDefaultStationAsync(CancellationToken cancellation = default)
        {
            var result = await _stationsRepository.GetDefaultStationAsync(cancellation);
            if (result is null) return null;
            return new DefaultStationResponce(result.Title);
        }

        public async Task<FrequentlyStationsResponse> GetFrequentlyUsedStationsAsync()
        {
            var result = await _stationsRepository.GetFrequentlyUsedStationsAsync();
            var listOfTitles = result.Select(x => x.Title).ToList() ?? [];
            return new FrequentlyStationsResponse(listOfTitles);
        }
    }
}
