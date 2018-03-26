using System.Windows;
using System.Windows.Controls;
using Castle.Windsor;
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
        private IWindsorContainer _container;

        public ToolkitsPage()
        {
            InitializeComponent();
        }

        public ToolkitsPage(TimeManager timeManager, IWindsorContainer container)
        {
            InitializeComponent();
            _timeManager = timeManager;
            _container = container;
            this.DataContext = _container.Resolve<ToolkitViewModel>();
            var viewModel = this.DataContext as ToolkitViewModel;
            viewModel.TimeManager = _timeManager;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
