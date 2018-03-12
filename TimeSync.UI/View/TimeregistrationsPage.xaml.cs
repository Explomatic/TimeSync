using System;
using System.Windows;
using System.Windows.Controls;
using TimeSync.DataAccess;
using TimeSync.Model;

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
