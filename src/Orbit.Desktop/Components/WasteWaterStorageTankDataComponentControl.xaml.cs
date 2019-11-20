using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Orbit.Models;
using Orbit.Util;

namespace Orbit.Desktop.Components
{
    /// <summary>
    /// Interaction logic for WasteWaterStorageTankDataComponentControl.xaml
    /// </summary>
    public partial class WasteWaterStorageTankDataComponentControl : UserControl
    {
        public AlertViewModel WaterLevelAlert {
            get => (AlertViewModel)DataContext;
            set => DataContext = value;
        }
        
        public WasteWaterStorageTankDataComponentControl()
        {
            InitializeComponent();

            // Subscribe to updates
            EventMonitor.Instance.NewValueRead += this.Instance_NewValueRead;
            EventMonitor.Instance.AlertReported += this.Instance_AlertReported;
        }

        private void Instance_AlertReported(object? sender, AlertEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => Instance_AlertReported(sender, e));
                return;
            }

            Trace.WriteLine($"Alert triggered: {e.Alert}", "Trace");
            if (e.Alert.PropertyName == nameof(WasteWaterStorageTankData.Level))
            {
                // Handle level alert
                if (e.Report is WasteWaterStorageTankData data)
                {
                    if (this.WaterLevelAlert == null)
                    {
                        this.WaterLevelAlert = new AlertViewModel(data, e.Alert);
                    }
                    else
                    {
                        this.WaterLevelAlert.Alert = e.Alert;
                    }
                }
            }
        }

        private void Instance_NewValueRead(object? sender, ValueReadEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => Instance_NewValueRead(sender, e));
                return;
            }

            Trace.WriteLine($"New Value Read for {e.Report.ComponentName}", "Trace");
            if (e.Report is WasteWaterStorageTankData data)
            {
                if (this.WaterLevelAlert == null)
                {
                    this.WaterLevelAlert = new AlertViewModel(data, Alert.Safe(nameof(WasteWaterStorageTankData.Level)));
                }
                else
                {
                    this.WaterLevelAlert.Model = data;
                }
            }
        }
    }


    public class AlertViewModel : ViewModelBase
    {
        public AlertViewModel(IModel model, Alert alert)
        {
            _model = model;
            _alert = alert;
        }

        private IModel _model;
        private Alert _alert;

        public IModel Model {
            get => _model;
            set => OnPropertyChanged(ref _model, value);
        }

        public Alert Alert {
            get => _alert;
            set => OnPropertyChanged(ref _alert, value);
        }
    }
}