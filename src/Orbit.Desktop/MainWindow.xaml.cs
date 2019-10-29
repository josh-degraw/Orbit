using System;
using System.Windows;

using Microsoft.Extensions.DependencyInjection;
using Orbit.Models;
using Orbit.Util;

namespace Orbit.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IServiceProvider ServiceProvider { get; }

        public IMonitoredComponent<BatteryReport> Battery { get; }

        public MainWindow()
        {
            this.InitializeComponent();
            this.ServiceProvider = /*serviceProvider??*/ App.ServiceProvider;
            this.Battery = ServiceProvider.GetService<IMonitoredComponent<BatteryReport>>();
        }
    }
}