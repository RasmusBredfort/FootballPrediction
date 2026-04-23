using Microsoft.EntityFrameworkCore;
using FootballPrediction.Models;

namespace FootballPrediction.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Match> Matches { get; set; }
    }
}
