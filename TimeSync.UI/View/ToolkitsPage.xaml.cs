using System.Windows;
using System.Windows.Controls;
using TimeSync.DataAccess;
using TimeSync.UI.ViewModel;

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

        public ToolkitsPage(TimeManager timeManager)
        {
            InitializeComponent();
            _timeManager = timeManager;

            var viewModel = this.DataContext as ToolkitInfoViewModel;
            viewModel.TimeManager = _timeManager;
        }

        private void ButtonBase_OnClickUserAccount(object sender, RoutedEventArgs e)
        {
            UserPage userPage = new UserPage(_timeManager);
            NavigationService.Navigate(userPage);
        }

        private void ButtonBase_OnClickToolkits(object sender, RoutedEventArgs e)
        {
            ToolkitsPage toolkitsPage = new ToolkitsPage(_timeManager);
            NavigationService.Navigate(toolkitsPage);
        }

        private void ButtonBase_OnClickTimeregistrations(object sender, RoutedEventArgs e)
        {
            TimeregistrationsPage timeregistrationsPage = new TimeregistrationsPage(_timeManager);
            NavigationService.Navigate(timeregistrationsPage);
        }
    }
}
