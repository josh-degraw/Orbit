#nullable disable warnings
using Microsoft.EntityFrameworkCore;

using Orbit.Models;

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Threading.Tasks;

namespace Orbit.Data
{
    public class OrbitDbContext : DbContext, IAsyncDisposable
    {
        public OrbitDbContext(DbContextOptions<OrbitDbContext> options) : base(options)
        {
        }

        public DbSet<UrineSystemData> UrineProcessors { get; set; }

        public DbSet<WaterProcessorData> WaterProcessors { get; set; }

        public DbSet<WasteWaterStorageTankData> WasteWaterStorageTanks { get; set; }

        public DbSet<Atmosphere> CabinAtmosphere { get; set; }

        public DbSet<OxygenGenerator> OxygenGenerator { get; set; }

        public DbSet<CarbonDioxideRemediation> CarbonDioxideRemover { get; set; }

        public void InsertSeedData()
        {
            //TODO: Use NSubstitute for generating random seed data
            this.UrineProcessors.Add(new UrineSystemData {
                BrineTankLevel = 5,
                DistillerSpeed = 20,
                DistillerTemp = 20,
                FluidControlPump = "Ok",
                ProcessorId = "Main",
                PurgePump = "Online",
                SystemStatus = "Ready",
                UrineTankLevel = 40,
            });
            this.WasteWaterStorageTanks.Add(new WasteWaterStorageTankData {
                TankId = "Main",
                Level = 30,
            });

            this.WaterProcessors.Add(new WaterProcessorData {
                CatalyticReactorTemp = 15,
                DeliveryPump ="Carbonated",
                PostFilterContaminateSensor = "Activated",
                PreHeaterTemp= 20,
                ProductTankLevel = 80.5,
                ProcessorId = "Main",
                ReactorHealthSensor = "Healthy",
                ReprocessDiverterValve = "Active",
                SupplyPump = "Supplied",
                SystemStatus= "Ready",
            });
            this.SaveChanges();
        }

        private static void MapModelCommonProps<T>(EntityTypeBuilder<T> e) where T : class, IModel
        {
            e.Property<Guid>("Id").ValueGeneratedOnAdd();
            e.HasKey("Id");

            e.Property(p => p.ReportDateTime).ValueGeneratedOnAdd();
            e.HasAlternateKey(p => p.ReportDateTime);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
                throw new ArgumentNullException(nameof(modelBuilder));

            // Do this for all of these so we don't need to worry about defining the ids in code
            modelBuilder.Entity<UrineSystemData>(MapModelCommonProps);
            modelBuilder.Entity<WaterProcessorData>(MapModelCommonProps);
            modelBuilder.Entity<WasteWaterStorageTankData>(MapModelCommonProps);
            modelBuilder.Entity<Atmosphere>(MapModelCommonProps);
            modelBuilder.Entity<CarbonDioxideRemediation>(MapModelCommonProps);
            modelBuilder.Entity<OxygenGenerator>(MapModelCommonProps);
        }

#if !NETSTANDARD_21
        public ValueTask DisposeAsync()
        {
            base.Dispose();
            return new ValueTask();
        }
#endif
    }
}