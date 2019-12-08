using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Orbit.Data;
using Orbit.Models;

namespace Orbit.Util
{
    public sealed class DataGenerator : IDataGenerator
    {
        private static readonly Lazy<IDataGenerator> _instance = new Lazy<IDataGenerator>(() => new DataGenerator());
        public static IDataGenerator Instance => _instance.Value;

        private DataGenerator()
        {
            AppDomain.CurrentDomain.ProcessExit += (s, e) => this._cancellationTokenSource.Cancel();
        }

        private Task? _eventThread;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private IServiceProvider ServiceProvider => OrbitServiceProvider.Instance;

        public void Start()
        {
            if (_eventThread == null)
            {
                _eventThread = Task.Run(SimulateDataGeneration, _cancellationTokenSource.Token);
            }
        }

        public void Stop()
        {
            if (!this._cancellationTokenSource.IsCancellationRequested)
            {
                this._cancellationTokenSource.Cancel();
            }
        }

        public event EventHandler? Started;

        public event EventHandler? Stopped;

        /// <summary>
        /// This method simulates generating real-world data and inserting it into the database.
        /// </summary>
        /// <returns> </returns>
        private async Task SimulateDataGeneration()
        {
            CancellationToken token = _cancellationTokenSource.Token;
            var rand = new Random();
            this.Started?.Invoke(this, EventArgs.Empty);
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(3), token).ConfigureAwait(true);

                if (token.IsCancellationRequested)
                {
                    break;
                }

                using var scope = ServiceProvider.CreateScope();
                using var db = scope.ServiceProvider.GetRequiredService<OrbitDbContext>();

                WasteWaterStorageTankData wt = db.WasteWaterStorageTankData.First();
               
                var next = new WasteWaterStorageTankData {
                    TankId = "Main",
                    Level = rand.NextDouble() * 100
                };

                UrineSystemData up = db.UrineProcessorData.First();
                var nextup = new UrineSystemData {
                    SystemStatus = up.SystemStatus,
                    UrineTankLevel = up.UrineTankLevel + 5,
                    SupplyPumpOn = up.SupplyPumpOn,
                    DistillerOn = up.DistillerOn,
                    DistillerTemp = (double)rand.NextDouble() * 1000,
                    PurgePumpOn = up.PurgePumpOn,
                    BrineTankLevel = up.BrineTankLevel + 2
                };

                WaterProcessorData wp = db.WaterProcessorData.First();
                var nextwp = new WaterProcessorData {
                    SystemStatus = wp.SystemStatus,
                    PumpOn = wp.PumpOn,
                    FiltersOK = wp.FiltersOK,
                    HeaterOn = wp.HeaterOn,
                    PostHeaterTemp = (double)rand.NextDouble() * 1000,
                    PostReactorQualityOK = wp.PostReactorQualityOK,
                    DiverterValvePosition = wp.DiverterValvePosition,
                    ProductTankLevel = rand.Next(0, 100)
                };

                db.WasteWaterStorageTankData.Add(next);
                await db.SaveChangesAsync(token);
            }

            this.Stopped?.Invoke(this, EventArgs.Empty);
        }

    }
}
