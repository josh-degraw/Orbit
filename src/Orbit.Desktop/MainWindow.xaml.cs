using System;
using System.ComponentModel;
using System.Windows;

using Microsoft.Extensions.DependencyInjection;
using Orbit.Data;
using Orbit.Models;

namespace Orbit.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private IServiceProvider ServiceProvider { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private WasteWaterStorageTankData? _wasteWater;


        public WasteWaterStorageTankData? WasteWater {
            get => this._wasteWater;
            set 
            {
                // This stuff here is required to tell the UI to update anything that references this property
                this._wasteWater = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.WasteWater)));
            }
        }

        public MainWindow(IServiceProvider serviceProvider)
        {
            this.InitializeComponent();
            this.ServiceProvider = serviceProvider ?? App.ServiceProvider;
            this.Loaded += this.MainWindow_Loaded;
            
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Try to load 
            using var scope = ServiceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrbitDbContext>();
            db.InsertSeedData();
            var comp = scope.ServiceProvider.GetService<IMonitoredComponent<WasteWaterStorageTankData>>();
            var rep = await comp.GetLatestReportAsync().ConfigureAwait(true);
            this.WasteWater = rep;
        }
    }
}