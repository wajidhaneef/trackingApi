using Microsoft.EntityFrameworkCore;
using trackingApi.Models;

namespace trackingApi.Data
{
    public class IssueDbContext : DbContext 
    {
        public IssueDbContext(DbContextOptions<IssueDbContext> options) : base(options)
        {

        }

        public DbSet<Issue> Issues { get; set; } = null!;

        public DbSet<Currency> Currency { get; set; } = null!;
        public DbSet<News> News { get; set; }
        public DbSet<Weather> Weathers { get; set; }
    }
}
