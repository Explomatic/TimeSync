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
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {

        private SharepointClient _sharepointClient;

        public HomePage()
        {
            InitializeComponent();
            _sharepointClient = new SharepointClient();
        }

        private void ButtonBase_OnClickUserAccount(object sender, RoutedEventArgs e)
        {
            UserAccount userAccount = new UserAccount();
            this.NavigationService.Navigate(userAccount);
        }

        private void ButtonBase_OnClickToolkits(object sender, RoutedEventArgs e)
        {
            Toolkits toolkits = new Toolkits();
            this.NavigationService.Navigate(toolkits);
        }

        private void ButtonBase_OnClickTimeregistrations(object sender, RoutedEventArgs e)
        {
            Timeregistrations timeregistrations = new Timeregistrations();
            this.NavigationService.Navigate(timeregistrations);
        }
    }
}
