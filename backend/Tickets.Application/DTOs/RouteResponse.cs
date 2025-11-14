

namespace Tickets.Application.DTOs
{
    public record RouteResponse(List<RouteFromResponse> RouteFromResponses, List<RouteBackResponse> RouteBackResponses);
}
