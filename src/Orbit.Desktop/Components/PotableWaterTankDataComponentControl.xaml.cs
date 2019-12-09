using System;
using System.Windows.Controls;

using NLog;

using Orbit.Models;
using Orbit.Util;

namespace Orbit.Desktop.Components
{
    /// <summary>
    /// Interaction logic for PotableWaterTankDataComponentControl.xaml
    /// </summary>
    public partial class PotableWaterTankDataComponentControl : UserControl
    {
        private static readonly Lazy<ILogger> _logger = new Lazy<ILogger>(LogManager.GetCurrentClassLogger);

        private static ILogger Logger => _logger.Value;

        public AlertViewModel PotableTankLevelAlert {
            get => (AlertViewModel)DataContext;
            set => DataContext = value;
        }
        public PotableWaterTankDataComponentControl()
        {
            InitializeComponent();

            // Subscribe to updates here
            EventMonitor.Instance.AlertReported += this.Instance_AlertReported;
        }

        private void Instance_AlertReported(object? sender, AlertEventArgs e)
        {
            if (e.Alert.PropertyName == nameof(WaterProcessorData.ProductTankLevel))
            {
                // Handle level alert
                if (e.Report is WaterProcessorData data)
                {
                    Logger.Info("Alert reported: {data}", new { data.ProductTankLevel, e.Alert.Message });
                    if (this.PotableTankLevelAlert == null)
                    {
                        this.PotableTankLevelAlert = new AlertViewModel(data, e.Alert);
                    }
                    else
                    {
                        this.PotableTankLevelAlert.Model = data;
                        this.PotableTankLevelAlert.Alert = e.Alert;
                    }
                }
            }
        }
    }
}
