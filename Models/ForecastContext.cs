using Microsoft.EntityFrameworkCore;

namespace DuploCloudApi.Models
{
    public class ForecastContext : DbContext
    {
        public ForecastContext(DbContextOptions<ForecastContext> options)
        : base(options)
        {
        }
        public DbSet<Forecast> Forecasts { get; set; } = null!;
    }
}
