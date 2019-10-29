using System;
using System.ComponentModel;
using System.Windows;

using Microsoft.Extensions.DependencyInjection;

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

        private BatteryReport? _battery;


        public BatteryReport? Battery {
            get => _battery;
            set 
            {
                // This stuff here is required to tell the UI to update anything that references this property
                _battery = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Battery)));
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
            using var scope = ServiceProvider.CreateScope();
            var comp = scope.ServiceProvider.GetService<IMonitoredComponent<BatteryReport>>();
            var rep = await comp.GetLatestReportAsync().ConfigureAwait(true);
            this.Battery = rep;
        }
    }
}