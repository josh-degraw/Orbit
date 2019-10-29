﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orbit.Desktop.Components;
using Orbit.Util;

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Orbit.Components;
using System.Windows.Navigation;
using Orbit.Models;
using Orbit.Data;
using AutoFixture;

namespace Orbit.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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

            OrbitServiceProvider.OnRegisteringServices += this.ConfigureServices;


            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            _thread = Task.Run(InsertNewBatteryReports, _tokenSource.Token);
        }

        private async Task InsertNewBatteryReports()
        {
            Limit limit;
            using (var db = ServiceProvider.GetRequiredService<OrbitDbContext>())
            {
                db.InsertSeedData();
                limit = db.Limits.First();
            }

            var rand = new Random();
            while (true)
            {
                using (var scope = ServiceProvider.CreateScope())
                using (var db = scope.ServiceProvider.GetRequiredService<OrbitDbContext>())
                {
                    var next = new BatteryReport(DateTimeOffset.UtcNow, rand.Next(300, 400)) {
                        LimitId = limit.Id,
                    };

                    db.BatteryReports.Add(next);
                    db.SaveChanges();
                }

                await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(true);
            }
        }


        private void ConfigureServices(object? sender, IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddTransient<ModuleComponentControl>();
        }
    }
}