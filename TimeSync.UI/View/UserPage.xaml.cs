using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Castle.Windsor;
using TimeSync.DataAccess;
using TimeSync.Model;
using TimeSync.UI.ViewModel;

namespace TimeSync.UI.View
{

    public partial class UserPage : Page
    {       
        public UserPage(ToolkitUserViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        public void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            { ((dynamic)DataContext).Password = ((PasswordBox)sender).Password; }
        }

        private void ClickTestpage(object sender, RoutedEventArgs e)
        {
            var testPage = new Page1();
            NavigationService.Navigate(testPage);
        }
    }
}
