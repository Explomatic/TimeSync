using System;
using System.Windows;
using System.Windows.Controls;
using Castle.Windsor;
using TimeSync.DataAccess;
using TimeSync.UI.ViewModel;

namespace TimeSync.UI.View
{
    /// <summary>
    /// Interaction logic for Toolkits.xaml
    /// </summary>
    public partial class ToolkitsPage : Page
    {
        public ToolkitsPage()
        {
            InitializeComponent();
        }

        public ToolkitsPage(ToolkitViewModel vm) : this()
        {
            DataContext = vm;
        }
    }
}
