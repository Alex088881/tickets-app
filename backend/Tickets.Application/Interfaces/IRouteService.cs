
using Tickets.Application.DTOs;
using Tickets.Domain.Models;

namespace Tickets.Application.Interfaces
{
    public interface IRouteService
    {
        Task<RouteResponse?> GetScheduleAsync(RouteRequest request);

        
    }
}