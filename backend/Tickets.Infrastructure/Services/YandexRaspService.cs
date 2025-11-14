using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Runtime;
using System.Text.Json;
using Tickets.Domain.Models;

namespace Tickets.Infrastructure.Services
{
    public class YandexRaspService : IYandexRaspService
    {
        private readonly HttpClient _http;
        private readonly YandexRaspSettings _settings;


        public YandexRaspService(HttpClient http, IOptions<YandexRaspSettings> options)
        {
            _http = http;
            _settings = options.Value;

        }

        public async Task<List<Station>> GetStationsAsync(CancellationToken cancellation = default)
        {
            var url = $"https://api.rasp.yandex.net/v3.0/stations_list/?apikey={_settings.ApiKey}&lang=ru_RU&format=json";
            var response = await _http.GetStringAsync(url, cancellation);

            var result = JsonSerializer.Deserialize<StationResponse>(response);

            var stations = result?.Countries
                .Where(c => c.Title == "Россия")
                .SelectMany(c => c.Regions)
                .SelectMany(r => r.Settlements)
                .SelectMany(s => s.Stations)
                .Where(s => !string.IsNullOrEmpty(s.Codes.YandexCode) &&
                 s.StationType == "train_station" 
                 || s.StationType == "station"
                 || s.StationType == "platform"
                 || s.StationType == "stop"
                 || s.StationType == "checkpoint"
                 || s.StationType == "post"
                 || s.StationType == "crossing"
                 || s.StationType == "overtaking_point"
                 && (s.TransportType == "train"))
                .Select(s => new Station
                {
                    Code = s.Codes.YandexCode,
                    Title = s.Title.ToLower()
                })
                .ToList();

            return stations ?? new List<Station>();
        }

        public async Task<RaspResponse?> GetScheduleAsync(string fromCode, string toCode, DateTime date, CancellationToken cancellation = default)
        {
            var url = $"https://api.rasp.yandex.net/v3.0/search/?apikey={_settings.ApiKey}&from={fromCode}&to={toCode}&transport_types=suburban&lang=ru_RU&format=json&date={date:yyyy-MM-dd}";
            var response = await _http.GetStringAsync(url, cancellation);
            var result = JsonSerializer.Deserialize<RaspResponse>(response);
            return result;
        }

        public class YandexRaspSettings
        {
            public string ApiKey { get; set; } = string.Empty;
            public string BaseUrl { get; set; } = string.Empty;
        }
    }
}
