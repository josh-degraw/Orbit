using System;
using Orbit.Models;

namespace Orbit.Desktop
{
    public static class ReportViewModel
    {
        public static ReportViewModel<T> Create<T>(T report) where T : IModel
        {
            return new ReportViewModel<T>(report.ComponentName, report);
        }
    }

    public class ReportViewModel<T> : ViewModelBase where T: IModel
    {
        public ReportViewModel(string componentName, T report)
        {
            this.ComponentName = componentName;
            this.CurrentReport = report;
            this.ReportDate = report.ReportDateTime;
        }
        
        private DateTimeOffset _reportDate;
        private T _currentReport;

        public string ComponentName { get; }

        public DateTimeOffset ReportDate {
            get => _reportDate;
            set => OnPropertyChanged(ref _reportDate, value);
        }

        public T CurrentReport {
            get => this._currentReport;
            set => OnPropertyChanged(ref this._currentReport, value);
        }
    }
}