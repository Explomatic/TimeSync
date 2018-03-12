using System;
using System.Windows;
using System.Windows.Controls;
using TimeSync.DataAccess;
using TimeSync.Model;
using TimeSync.UI.ViewModel;

namespace TimeSync.UI.View
{
    public partial class TimeregistrationsPage : Page
    {
        private TimeManager _timeManager;
        public TimeregistrationsPage()
        {
            InitializeComponent();
        }

        public TimeregistrationsPage(TimeManager timeManager)
        {
            InitializeComponent();
            _timeManager = timeManager;
            var viewModel = this.DataContext as TimeregistrationViewModel;
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
