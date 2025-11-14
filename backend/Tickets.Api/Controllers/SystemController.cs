using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tickets.Application.Interfaces;
using Tickets.Infrastructure.Data;

namespace Tickets.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemController:ControllerBase
    {
        private readonly IDataInitializer _dataInitializer;
        private readonly AppDbContext _db;

        public SystemController(IDataInitializer dataInitializer, AppDbContext db)
        {
            _dataInitializer = dataInitializer;
            _db = db;
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            var stationCount = await _db.Stations.CountAsync();

            return Ok(new
            {
                IsInitialized = _dataInitializer.IsInitialized,
                IsInitializing = _dataInitializer.IsInitializing,
                TotalStations = stationCount,
                HasData = stationCount > 0
            });
        }

        [HttpPost("initialize")]
        public async Task<IActionResult> InitializeManually(CancellationToken cancellationToken = default)
        {
            if (_dataInitializer.IsInitializing)
            {
                return BadRequest(new { message = "Инициализация уже выполняется" });
            }

            await _dataInitializer.InitializeAsync(cancellationToken);

            return Ok(new { message = "Инициализация завершена" });
        }
    }
}
