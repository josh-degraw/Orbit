using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Orbit.Components;

namespace Orbit.Desktop.Components
{
    /// <summary>
    /// Interaction logic for BatteryComponentControl.xaml
    /// </summary>
    public partial class BatteryComponentControl : UserControl
    {
        [DisallowNull]
        public BatteryComponent Battery
        {
            get => (BatteryComponent)DataContext;
            set => DataContext = value;
        }

        public BatteryComponentControl()
        {
            InitializeComponent();
        }
    }
}
