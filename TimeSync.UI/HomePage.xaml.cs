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
using TimeSync.Model;

namespace TimeSync.UI
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private TimeManager _timeManager;
        private UserAccountPage _userAccountPage;
        private ToolkitsPage _toolkitsPage;
        private TimeregistrationsPage _timeregistrationsPage;

        public HomePage()
        {
            InitializeComponent();
            _timeManager = new TimeManager();
            _userAccountPage = new UserAccountPage(_timeManager);
            _toolkitsPage = new ToolkitsPage(_timeManager);
            _timeregistrationsPage = new TimeregistrationsPage(_timeManager);
        }

        private void ButtonBase_OnClickUserAccount(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(_userAccountPage);
        }

        private void ButtonBase_OnClickToolkits(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(_toolkitsPage);
        }

        private void ButtonBase_OnClickTimeregistrations(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(_timeregistrationsPage);
        }
    }
}
