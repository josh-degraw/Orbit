using System;
using System.Windows.Controls;

using NLog;

using Orbit.Models;
using Orbit.Util;

namespace Orbit.Desktop.Components
{
    /// <summary>
    /// Interaction logic for WaterProcessorStatusComponentControl.xaml
    /// </summary>
    public partial class WaterProcessorStatusComponentControl : UserControl
    {
        private static readonly Lazy<ILogger> _logger = new Lazy<ILogger>(LogManager.GetCurrentClassLogger);

        private static ILogger Logger => _logger.Value;

        public AlertViewModel WaterProcessorStatusAlert {
            get => (AlertViewModel)DataContext;
            set => DataContext = value;
        }

        public WaterProcessorStatusComponentControl()
        {
            InitializeComponent();

            EventMonitor.Instance.AlertReported += this.Instance_AlertReported;
        }

        private void Instance_AlertReported(object? sender, AlertEventArgs e)
        {
            if (e.Alert.PropertyName == nameof(WaterProcessorData.SystemStatus))
            {
                // Handle level alert
                if (e.Report is WaterProcessorData data)
                {
                    Logger.Info("Alert reported: {data}", new { data.SystemStatus, e.Alert.Message });
                    if (this.WaterProcessorStatusAlert == null)
                    {
                        this.WaterProcessorStatusAlert = new AlertViewModel(data, e.Alert);
                    }
                    else
                    {
                        this.WaterProcessorStatusAlert.Model = data;
                        this.WaterProcessorStatusAlert.Alert = e.Alert;
                    }
                }
            }
        }
    }
}
