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
        private TimeManager _timeManager;
        private IWindsorContainer _container;
        public UserPage()
        {
            InitializeComponent();
        }

        public UserPage(TimeManager timeManager, IWindsorContainer container)
        {
            InitializeComponent();
            _timeManager = timeManager;
            _container = container;
            this.DataContext = _container.Resolve<ToolkitUserViewModel>();
            var viewModel = this.DataContext as ToolkitUserViewModel;
            
            viewModel.TimeManager = _timeManager;
        }

        public void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((dynamic)this.DataContext).Password = ((PasswordBox)sender).Password; }
        }

        //
        private void ClickTestpage(object sender, RoutedEventArgs e)
        {
            var testPage = new Page1();
            NavigationService.Navigate(testPage);
        }
    }
}
