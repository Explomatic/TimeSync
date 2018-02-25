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

namespace TimeSync.UI
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private UserAccount _userAccount;
        private Toolkits _toolkits;
        private Timeregistrations _timeregistrations;

        public HomePage()
        {
            InitializeComponent();
            _userAccount = new UserAccount();
            _toolkits = new Toolkits();
            _timeregistrations = new Timeregistrations();
        }

        private void ButtonBase_OnClickUserAccount(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(_userAccount);
        }

        private void ButtonBase_OnClickToolkits(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(_toolkits);
        }

        private void ButtonBase_OnClickTimeregistrations(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(_timeregistrations);
        }
    }
}
