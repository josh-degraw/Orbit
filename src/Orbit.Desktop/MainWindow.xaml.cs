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
        private UrineSystemData? _urineSystemData;
        private WaterProcessorData? _waterProcessorData;


        public WasteWaterStorageTankData? WasteWater {
            get => this._wasteWater;
            set 
            {
                // This stuff here is required to tell the UI to update anything that references this property
                this._wasteWater = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.WasteWater)));
            }
        }

        public UrineSystemData? UrineTank {
            get => this._urineSystemData;
            set {
                this._urineSystemData = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.UrineTank)));
            }
        }

        public WaterProcessorData? PotableWaterTank {
            get => this._waterProcessorData;
            set {
                this._waterProcessorData = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.PotableWaterTank)));
            }
        }

        public UrineSystemData? UrineSystemStatus {
            get => this._urineSystemData;
            set {
                this._urineSystemData = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.UrineSystemStatus)));
            }
        }

        public WaterProcessorData? WaterProcessorStatus {
            get => this._waterProcessorData;
            set {
                this._waterProcessorData = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.WaterProcessorStatus)));
            }
        }

        public MainWindow(IServiceProvider serviceProvider)
        {
            this.InitializeComponent();
            this.ServiceProvider = serviceProvider;
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

            var urineComp = scope.ServiceProvider.GetService<IMonitoredComponent<UrineSystemData>>();
            var urineRep = await urineComp.GetLatestReportAsync().ConfigureAwait(true);
            this.UrineTank = urineRep;

            var potableWaterComp = scope.ServiceProvider.GetService<IMonitoredComponent<WaterProcessorData>>();
            var potableWaterRep = await potableWaterComp.GetLatestReportAsync().ConfigureAwait(true);
            this.PotableWaterTank = potableWaterRep;

            var urineStatusComp = scope.ServiceProvider.GetService<IMonitoredComponent<UrineSystemData>>();
            var urineStatusRep = await urineStatusComp.GetLatestReportAsync().ConfigureAwait(true);
            this.UrineSystemStatus = urineStatusRep;

            var waterProcessorComp = scope.ServiceProvider.GetService<IMonitoredComponent<WaterProcessorData>>();
            var waterProcessorRep = await waterProcessorComp.GetLatestReportAsync().ConfigureAwait(true);
            this.WaterProcessorStatus = waterProcessorRep;

        }
    }
}