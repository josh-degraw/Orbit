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
                UrineSystemData up = db.UrineProcessorData.First();
                WaterProcessorData wp = db.WaterProcessorData.First();

                // these should probably be moved out of the db connection method, but not sure how without breaking stuff
                up.ProcessData(wt.Level, rand.NextDouble() * 100, rand.Next(0, 3500));
                wt.ProcessData(up.SystemStatus, wp.SystemStatus);
                wp.ProcessData(wt.Level, rand.NextDouble() * 100);

                var nextup = new UrineSystemData {
                    SystemStatus = up.SystemStatus,
                    UrineTankLevel = up.UrineTankLevel,
                    SupplyPumpOn = up.SupplyPumpOn,
                    DistillerOn = up.DistillerOn,
                    DistillerTemp = rand.NextDouble() * 100,
                    DistillerSpeed = up.DistillerSpeed,
                    PurgePumpOn = up.PurgePumpOn,
                    BrineTankLevel = up.BrineTankLevel
                };
                var next = new WasteWaterStorageTankData {
                    TankId = "Main",
                    Level = wt.Level  //rand.NextDouble() * 100
                };

                var nextwp = new WaterProcessorData {
                    SystemStatus = wp.SystemStatus,
                    PumpOn = wp.PumpOn,
                    FiltersOk = wp.FiltersOk,
                    HeaterOn = wp.HeaterOn,
                    PostHeaterTemp = rand.NextDouble() * 100,
                    PostReactorQualityOK = wp.PostReactorQualityOK,
                    DiverterValvePosition = wp.DiverterValvePosition,
                    ProductTankLevel = wp.ProductTankLevel
                };

                db.UrineProcessorData.Add(nextup);
                db.WasteWaterStorageTankData.Add(next);
                db.WaterProcessorData.Add(nextwp);
                await db.SaveChangesAsync(token);
            }

            this.Stopped?.Invoke(this, EventArgs.Empty);
        }

    }
}
