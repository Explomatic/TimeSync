using System.Windows;
using System.Windows.Controls;
using TimeSync.DataAccess;

namespace TimeSync.UI.View
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
            UserPage userPage = new UserPage();
            NavigationService.Navigate(userPage);
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
