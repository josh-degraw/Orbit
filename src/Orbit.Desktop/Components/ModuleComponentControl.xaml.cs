using Orbit.Models;

using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using Orbit.Util;

namespace Orbit.Desktop.Components
{
    /// <summary>
    /// Interaction logic for ModuleComponentControl.xaml
    /// </summary>
    public partial class ModuleComponentControl : UserControl
    {
        [DisallowNull]
        public IMonitoredComponent<BatteryReport> MonitoredComponent
        {
            get => (IMonitoredComponent<BatteryReport>)DataContext;
            set => DataContext = value;
        }

        public ReportViewModel ViewModel {
            get => (ReportViewModel)DataContext;
            set => DataContext = value;
        }

        public ModuleComponentControl()
        {
            InitializeComponent();

            EventMonitor.Instance.NewValueRead += this.Instance_NewValueRead;
        }

        private void Instance_NewValueRead(object? sender, CurrentValueReport e)
        {
            if(e.ComponentName == ViewModel.ComponentName)
            {
                ViewModel.CurrentValue = e.Value;
                ViewModel.ReportDate = e.Report.ReportDateTime;
            }
        }
    }
}