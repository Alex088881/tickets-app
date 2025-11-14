
using System.Linq;
using Tickets.Application.DTOs;
using Tickets.Application.Interfaces;
using Tickets.Domain;
using Tickets.Domain.Models;
using Tickets.Infrastructure.Repositories;
using Tickets.Infrastructure.Services;

namespace Tickets.Application
{
    public class RouteService : IRouteService
    {
        private readonly IYandexRaspService _yandexRaspService;
        private readonly IStationRepository _stationRepository;

        public RouteService(IYandexRaspService yandexRaspService, IStationRepository stationRepository)
        {
            _yandexRaspService = yandexRaspService;
            _stationRepository = stationRepository;
        }

        public async Task<RouteResponse?> GetScheduleAsync(RouteRequest request)
        {
            var fromStation = await _stationRepository.GetStationByName(request.FromStation);
            var toStation = await _stationRepository.GetStationByName(request.ToStation);

            if (fromStation == null || toStation == null)
                return null;

            var searchDate = DateTime.Now;
            if (request.Date is not null 
                && request.Date > DateTime.Now.Date.AddDays(-30) 
                && request.Date < DateTime.Now.AddDays(30))
            {
                searchDate = (DateTime)request.Date;
            }
            
            
            var scheduleTo = await _yandexRaspService.GetScheduleAsync(fromStation.Code, toStation.Code, searchDate);
            if (scheduleTo is null) return null;

            var filteredResultTo = FilterScheduleByDepartureTime(scheduleTo, new TimeSpan(10,0,0));

            var resultTo = filteredResultTo.Select(s => new RouteFromResponse(DateTimeOffset.Parse(s.Departure).ToString("HH:mm")
                , DateTimeOffset.Parse(s.Arrival).ToString("HH:mm"), s.Thread?.Number ?? string.Empty))
             .ToList();



            var scheduleBack = await _yandexRaspService.GetScheduleAsync(toStation.Code, fromStation.Code, searchDate);
            if (scheduleBack is null) return null;

            await _stationRepository.MarkStationAsUsedAsync(fromStation);
            await _stationRepository.MarkStationAsUsedAsync(toStation);

            var filtredScheduleBack = FilterScheduleByArrivalBack(scheduleBack, new TimeSpan(19, 0, 0));

            var resultBack = filtredScheduleBack.Select(s => new RouteBackResponse(DateTimeOffset.Parse(s.Departure).ToString("HH:mm")
                , DateTimeOffset.Parse(s.Arrival).ToString("HH:mm"), s.Thread?.Number ?? string.Empty))
             .ToList();


            return new RouteResponse(resultTo, resultBack);
        }

        private List<Segment> FilterScheduleByDepartureTime(RaspResponse raspResponse, TimeSpan maxDepartureTime)
        {
            if (raspResponse?.Segments == null)
                return new List<Segment>();

            var result =  raspResponse.Segments
                .Where(s =>
                !string.IsNullOrEmpty(s.Departure) &&
                DateTimeOffset.TryParse(s.Departure, out var departureTime) &&
                 departureTime.TimeOfDay <= maxDepartureTime)
                .ToList();
            return result;
        }

        private List<Segment> FilterScheduleByArrivalBack(RaspResponse raspResponse, TimeSpan minArrivalBackTime)
        {
            if (raspResponse?.Segments == null)
                return new List<Segment>();

            var result = raspResponse.Segments
                .Where(s =>
                !string.IsNullOrEmpty(s.Arrival) &&
                DateTimeOffset.TryParse(s.Arrival, out var departureTime) &&
                 departureTime.TimeOfDay >= minArrivalBackTime)
                .ToList();
            return result;
        }



    }
}
