using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
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
                await using var db = scope.ServiceProvider.GetRequiredService<OrbitDbContext>();

                var next = new WasteWaterStorageTankData {
                    TankId = "Main",
                    Level = rand.NextDouble() * 100
                };

                db.WasteWaterStorageTanks.Add(next);
                await db.SaveChangesAsync(token);
            }

            this.Stopped?.Invoke(this, EventArgs.Empty);
        }

    }
}
