using System;
using System.Windows.Controls;

using NLog;

using Orbit.Models;
using Orbit.Util;

namespace Orbit.Desktop.Components
{
    /// <summary>
    /// Interaction logic for WasteWaterStorageTankDataComponentControl.xaml
    /// </summary>
    public partial class WasteWaterStorageTankDataComponentControl : UserControl
    {
        private static readonly Lazy<ILogger> _logger = new Lazy<ILogger>(LogManager.GetCurrentClassLogger);

        private static ILogger Logger => _logger.Value;

        public AlertViewModel WaterLevelAlert {
            get => (AlertViewModel)DataContext;
            set => DataContext = value;
        }

        public WasteWaterStorageTankDataComponentControl()
        {
            InitializeComponent();

            // Subscribe to updates here
            EventMonitor.Instance.AlertReported += this.Instance_AlertReported;
        }

        private void Instance_AlertReported(object? sender, AlertEventArgs e)
        {
            if (e.Alert.PropertyName == nameof(WasteWaterStorageTankData.Level))
            {
                // Handle level alert
                if (e.Report is WasteWaterStorageTankData data)
                {
                    Logger.Info("Alert reported: {data}", new { data.Level, data.TankId, e.Alert.Message });
                    if (this.WaterLevelAlert == null)
                    {
                        this.WaterLevelAlert = new AlertViewModel(data, e.Alert);
                    }
                    else
                    {
                        this.WaterLevelAlert.Model = data;
                        this.WaterLevelAlert.Alert = e.Alert;
                    }
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

        protected IModel _model;
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