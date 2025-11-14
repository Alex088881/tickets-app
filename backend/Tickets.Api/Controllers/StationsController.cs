using Microsoft.AspNetCore.Mvc;
using Tickets.Application;
using Tickets.Domain.Models;


namespace Tickets.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StationsController : ControllerBase
    {
        private readonly IStationService _stationService;

        public StationsController(IStationService stationService)
        {
            _stationService = stationService;
        }

        // Загрузить станции в БД
        [HttpPost("load")]
        public async Task<IActionResult> LoadStations(CancellationToken cancellation = default)
        {
            var count = await _stationService.LoadStationsAsync(cancellation);
            return Ok(new { count });
        }

        // Поиск станции
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name, CancellationToken cancellation = default)
        {
            var result = await _stationService.SearchStationAsync(name, cancellation);
            return Ok(result);
        }

        // Установить станцию по умолчанию
        [HttpPost("default")]
        public async Task<IActionResult> SetDefaultStation([FromQuery] string name, CancellationToken cancellation = default)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest();
            }
            var result = await _stationService.SetDefaultStationAsync(name, cancellation);
            if (result is null)
            {
                return NotFound(new { message = "Станция не найдена в базе!" });
            }
            return Ok(result);
        }

        [HttpGet("default")]
        public async Task<IActionResult> GetDefaultStation(CancellationToken cancellation = default)
        {
            var result = await _stationService.GetDefaultStationAsync(cancellation);
            if (result is null)
            {
                return NotFound(new { message = "Станция не установлена" });
            }
            return Ok(result);
        }

        [HttpGet("frequently")]
        public async Task<IActionResult> GetFrequentlyUsedStations()
        {
            var result = await _stationService.GetFrequentlyUsedStationsAsync();
            return Ok(result);
        }

    }
}
