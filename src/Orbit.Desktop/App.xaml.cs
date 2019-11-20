using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Extensions.DependencyInjection;

using NLog;

using Orbit.Data;
using Orbit.Desktop.Components;
using Orbit.Models;
using Orbit.Util;

namespace Orbit.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IDisposable
    {
        private static readonly Lazy<ILogger> _logger = new Lazy<ILogger>(LogManager.GetCurrentClassLogger);
        private static ILogger Logger => _logger.Value;

        public static IServiceProvider ServiceProvider => OrbitServiceProvider.Instance;

        private Task? _thread;

        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        //public IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            //IConfigurationBuilder builder = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            //this.Configuration = builder.Build();
            Logger.Info("Starting program");
            OrbitServiceProvider.OnRegisteringServices += this.ConfigureServices;

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            _thread = Task.Run(this.SimulateDataGeneration, _tokenSource.Token);
        }

        /// <summary>
        /// This method simulates generating real-world data and inserting it into the database.
        /// </summary>
        /// <returns> </returns>
        private async Task SimulateDataGeneration()
        {
            CancellationToken token = _tokenSource.Token;
            var rand = new Random();
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
                    Level = rand.NextDouble() * 100
                };

                db.WasteWaterStorageTanks.Add(next);
                await db.SaveChangesAsync(token);
            }
        }

        private void ConfigureServices(object? sender, IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddTransient<WasteWaterStorageTankDataComponentControl>();
        }

        public void Dispose()
        {
            this._thread?.Dispose();
            this._tokenSource?.Dispose();
        }
    }
}