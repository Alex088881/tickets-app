using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tickets.Application.DTOs;
using Tickets.Application.Interfaces;
using Tickets.Infrastructure.Data;
using Tickets.Infrastructure.Services;

namespace Tickets.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _routeService;

        public RouteController(IRouteService routeService)
        {
            _routeService = routeService;
        }

        [HttpPost]
        public async Task<IActionResult> GetSchedule([FromBody] RouteRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(request.FromStation.ToString() == request.ToStation.ToString())
            {
                return BadRequest(ModelState);
            }

            var result = await _routeService.GetScheduleAsync(request);
            if (result is null)
            {
                return NotFound(new {message = "Маршрут не найден!"});
            }
            return Ok(result);
        }
    }
}
