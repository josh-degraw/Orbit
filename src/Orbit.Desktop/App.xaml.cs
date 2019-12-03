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

        private IServiceProvider ServiceProvider => OrbitServiceProvider.Instance;

        //public IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            //var scope = OrbitServiceProvider.Instance.CreateScope();
            //IConfigurationBuilder builder = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            //this.Configuration = builder.Build();
            Logger.Info("Starting program");
            OrbitServiceProvider.OnRegisteringServices += this.ConfigureServices;

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            ServiceProvider.GetRequiredService<IDataGenerator>().Start();
        }

        private void ConfigureServices(object? sender, IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddTransient<WasteWaterStorageTankDataComponentControl>();
            services.AddSingleton(SynchronizationContext.Current);
        }
        protected void Dispose (bool disposing)
        {
            if (disposing)
            {
                (ServiceProvider as IDisposable)?.Dispose();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}