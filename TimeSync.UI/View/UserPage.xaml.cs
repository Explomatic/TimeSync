using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using TimeSync.DataAccess;
using TimeSync.Model;
using TimeSync.UI.ViewModel;

namespace TimeSync.UI.View
{
    public partial class UserPage : Page
    {
        private TimeManager _timeManager;

        public UserPage()
        {
            InitializeComponent();
        }

        public UserPage(TimeManager timeManager)
        {
            InitializeComponent();
            _timeManager = timeManager;
            var viewModel = this.DataContext as ToolkitUserViewModel;
            viewModel.TimeManager = _timeManager;
        }

        public void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((dynamic)this.DataContext).Password = ((PasswordBox)sender).Password; }
        }

        private void ClickUserAccount(object sender, RoutedEventArgs e)
        {
            UserPage userPage = new UserPage(_timeManager);
            NavigationService.Navigate(userPage);
        }

        private void ClickToolkits(object sender, RoutedEventArgs e)
        {
            ToolkitsPage toolkitsPage = new ToolkitsPage(_timeManager);
            NavigationService.Navigate(toolkitsPage);
        }

        private void ClickTimeregistrations(object sender, RoutedEventArgs e)
        {
            TimeregistrationsPage timeregistrationsPage = new TimeregistrationsPage(_timeManager);
            NavigationService.Navigate(timeregistrationsPage);
        }
        //
        private void ClickTestpage(object sender, RoutedEventArgs e)
        {
            var testPage = new Page1();
            NavigationService.Navigate(testPage);
        }
    }
}
