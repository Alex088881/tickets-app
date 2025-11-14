using Microsoft.EntityFrameworkCore;
using Tickets.Domain.Models;

namespace Tickets.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Station> Stations { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
