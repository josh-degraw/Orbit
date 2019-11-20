using Orbit.Models;

using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Orbit.Util;

namespace Orbit.Desktop.Components
{
    /// <summary>
    /// Interaction logic for WasteWaterStorageTankDataComponentControl.xaml
    /// </summary>
    public partial class WasteWaterStorageTankDataComponentControl : UserControl
    {
        public ReportViewModel<WasteWaterStorageTankData> ViewModel {
            get => (ReportViewModel<WasteWaterStorageTankData>)DataContext;
            set => DataContext = value;
        }

        public string? ComponentName { get; set; }

        public WasteWaterStorageTankDataComponentControl()
        {
            InitializeComponent();

            // Subscribe to updates
            EventMonitor.Instance.NewValueRead += this.Instance_NewValueRead;
            EventMonitor.Instance.AlertReported += this.Instance_AlertReported;
        }

        private void Instance_AlertReported(object? sender, AlertEventArgs e)
        {

        }

        private void Instance_NewValueRead(object? sender, ValueReadEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => Instance_NewValueRead(sender, e));
                return;
            }
            
            //else 
            if (e.Report is WasteWaterStorageTankData data)
            {
                if (ViewModel == null)
                {
                    ViewModel = ReportViewModel.Create(data);
                }
                else
                {
                    ViewModel.CurrentReport = data;
                    ViewModel.ReportDate = e.Report.ReportDateTime;
                }
            }
        }
    }
}