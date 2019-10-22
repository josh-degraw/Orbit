using Microsoft.EntityFrameworkCore;

using Orbit.Models;

namespace Orbit.Data
{
    public class OrbitDbContext : DbContext
    {
        public OrbitDbContext(DbContextOptions<OrbitDbContext> options) : base(options)
        {
        }

        public DbSet<Limit> Limits { get; set; }

        public DbSet<BatteryReport> BatteryReports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BatteryReport>().HasOne<Limit>().WithMany();
        }
    }
}