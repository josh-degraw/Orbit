using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orbit.Desktop.Components;
using Orbit.Util;

using System;
using System.IO;
using System.Windows;
using Orbit.Components;

namespace Orbit.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider => OrbitServiceProvider.Instance;
        

        public IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            this.Configuration = builder.Build();

            OrbitServiceProvider.OnRegisteringServices += this.ConfigureServices;
            
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(object? sender, IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddTransient<BatteryComponentControl>();
        }
    }
}