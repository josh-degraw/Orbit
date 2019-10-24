using Orbit.Models;

using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;

namespace Orbit.Desktop.Components
{
    /// <summary>
    /// Interaction logic for BatteryComponentControl.xaml
    /// </summary>
    public partial class BatteryComponentControl : UserControl
    {
        [DisallowNull]
        public IMonitoredComponent<BatteryReport> Battery
        {
            get => (IMonitoredComponent<BatteryReport>)DataContext;
            set => DataContext = value;
        }

        public BatteryComponentControl()
        {
            InitializeComponent();
        }
    }
}