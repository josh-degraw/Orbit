using System;
using System.Windows.Controls;

using NLog;

using Orbit.Models;
using Orbit.Util;

namespace Orbit.Desktop.Components
{
    /// <summary>
    /// Interaction logic for UrineTankDataComponentControl.xaml
    /// </summary>
    public partial class UrineTankDataComponentControl : UserControl
    {
        private static readonly Lazy<ILogger> _logger = new Lazy<ILogger>(LogManager.GetCurrentClassLogger);

        private static ILogger Logger => _logger.Value;

        public AlertViewModel UrineLevelAlert {
            get => (AlertViewModel)DataContext;
            set => DataContext = value;
        }

        public UrineTankDataComponentControl()
        {
            InitializeComponent();

            // Subscribe to updates here
            EventMonitor.Instance.AlertReported += this.Instance_AlertReported;
        }

        private void Instance_AlertReported(object? sender, AlertEventArgs e)
        {
            if (e.Alert.PropertyName == nameof(UrineSystemData.UrineTankLevel))
            {
                // Handle level alert
                if (e.Report is UrineSystemData data)
                {
                    Logger.Info("Alert reported: {data}", new { data.UrineTankLevel, e.Alert.Message });
                    if (this.UrineLevelAlert == null)
                    {
                        this.UrineLevelAlert = new AlertViewModel(data, e.Alert);
                    }
                    else
                    {
                        this.UrineLevelAlert.Model = data;
                        this.UrineLevelAlert.Alert = e.Alert;
                    }
                }
            }
        }
    }
}
