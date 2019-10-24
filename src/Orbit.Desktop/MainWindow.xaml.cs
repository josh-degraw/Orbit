using System;
using System.Windows;

namespace Orbit.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IServiceProvider ServiceProvider { get; }
        public MainWindow(IServiceProvider serviceProvider)
        {
            this.InitializeComponent();
            this.ServiceProvider = serviceProvider;
        }
    }
}
