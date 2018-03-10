using System.Windows;
using System.Windows.Controls;
using TimeSync.DataAccess;

namespace TimeSync.UI.View
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private TimeManager _timeManager;
        private UserPage _userPage;
        private ToolkitsPage _toolkitsPage;
        private TimeregistrationsPage _timeregistrationsPage;

        public HomePage()
        {
            InitializeComponent();
            _timeManager = new TimeManager();
            _userPage = new UserPage(_timeManager);
            _toolkitsPage = new ToolkitsPage(_timeManager);
            _timeregistrationsPage = new TimeregistrationsPage(_timeManager);
        }

        private void ButtonBase_OnClickUserAccount(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(_userPage);
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
