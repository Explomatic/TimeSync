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
    /// Interaction logic for LogoPage.xaml
    /// </summary>
    public partial class LogoPage : Page
    {
        public LogoPage()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClickUserAccount(object sender, RoutedEventArgs e)
        {
            UserAccount userAccount = new UserAccount();
            this.NavigationService.Navigate(userAccount);
        }

        private void ButtonBase_OnClickToolkits(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ButtonBase_OnClickTimeregistration(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
