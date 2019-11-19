#nullable disable warnings
using Microsoft.EntityFrameworkCore;

using Orbit.Models;

using System;

namespace Orbit.Data
{
    public class OrbitDbContext : DbContext
    {
        public OrbitDbContext(DbContextOptions<OrbitDbContext> options) : base(options)
        {
        }

        public DbSet<Limit> Limits { get; set; }

        public DbSet<BatteryReport> BatteryReports { get; set; }

        public DbSet<InternalCoolantLoop> InternalCoolantLoop { get; set; }

        public DbSet<ExternalCoolantLoop> ExternalCoolantLoop { get; set; }

        public void InsertSeedData()
        {
            var lim = new Limit(Guid.NewGuid(), 400, 300, 50);

            this.Limits.Add(lim);
            this.BatteryReports.Add(new BatteryReport(DateTimeOffset.Now, 360) { Limit = lim });
            this.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));
            modelBuilder.Entity<Limit>(e =>
            {
                // Members like this one are "shadow" properties that are represented in the database, but we don't need
                // to define as visible class members
                e.Property<Guid>("Id").ValueGeneratedOnAdd();
                e.HasKey("Id");
            });

            modelBuilder.Entity<ReportBase>(e =>
            {
                e.Property<Guid>("Id").ValueGeneratedOnAdd();
                e.HasKey("Id");
                
                e.HasOne(d => d.Limit).WithMany().HasForeignKey("LimitId");

                e.Property(p => p.ReportDateTime).ValueGeneratedOnAdd();
                e.HasAlternateKey(p => p.ReportDateTime);
            });

            modelBuilder.Entity<BatteryReport>(e =>
            {
                e.HasBaseType<ReportBase>();
            });
        }
    }
}