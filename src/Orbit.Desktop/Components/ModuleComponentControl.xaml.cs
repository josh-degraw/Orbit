using Orbit.Models;

using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Orbit.Util;

namespace Orbit.Desktop.Components
{
    /// <summary>
    /// Interaction logic for ModuleComponentControl.xaml
    /// </summary>
    public partial class ModuleComponentControl : UserControl
    {
        public ReportViewModel ViewModel {
            get => (ReportViewModel)DataContext;
            set => DataContext = value;
        }

        public string? ComponentName { get; set; }

        public ModuleComponentControl()
        {
            InitializeComponent();

            EventMonitor.Instance.NewValueRead += this.Instance_NewValueRead;
        }


        private void Instance_NewValueRead(object? sender, CurrentValueReport e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => Instance_NewValueRead(sender, e));
                return;
            }
            
            //else 
            if (e.ComponentName == ComponentName)
            {
                if (ViewModel == null)
                {
                    ViewModel = new ReportViewModel(e.ComponentName, e.Value);
                }
                else
                {
                    ViewModel.CurrentValue = e.Value;
                    ViewModel.ReportDate = e.Report.ReportDateTime;
                }
            }
        }
    }
}