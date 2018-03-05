using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TimeSync.DataAccess;

namespace TimeSync.UI
{
    /// <summary>
    /// Interaction logic for Toolkits.xaml
    /// </summary>
    public partial class ToolkitsPage : Page
    {
        private TimeManager _timeManager;

        public ToolkitsPage()
        {
            InitializeComponent();
        }

        public ToolkitsPage(TimeManager _timeManager)
        {
            _timeManager = _timeManager;
        }

        private void ButtonBase_OnClickUserAccount(object sender, RoutedEventArgs e)
        {
            UserAccountPage userAccountPage = new UserAccountPage();
            NavigationService.Navigate(userAccountPage);
        }

        private void ButtonBase_OnClickToolkits(object sender, RoutedEventArgs e)
        {
            ToolkitsPage toolkitsPage = new ToolkitsPage();
            NavigationService.Navigate(toolkitsPage);
        }

        private void ButtonBase_OnClickTimeregistrations(object sender, RoutedEventArgs e)
        {
            TimeregistrationsPage timeregistrationsPage = new TimeregistrationsPage();
            NavigationService.Navigate(timeregistrationsPage);
        }
    }
}
