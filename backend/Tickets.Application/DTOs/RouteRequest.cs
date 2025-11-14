using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tickets.Application.DTOs
{
    public record RouteRequest(
        [Required]
        string FromStation,
        [Required]
        string ToStation,
        DateTime? Date);
}
