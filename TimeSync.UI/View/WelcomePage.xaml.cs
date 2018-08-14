using System.Windows.Controls;
using Castle.Windsor;
using TimeSync.DataAccess;
using TimeSync.UI.ViewModel;

namespace TimeSync.UI.View
{
    /// <summary>
    /// Interaction logic for WelcomePage.xaml
    /// </summary>
    public partial class WelcomePage : Page
    {
        private TimeManager _timeManager;
        private IWindsorContainer _container;

        public WelcomePage()
        {
        }

        public WelcomePage(TimeManager timeManager, IWindsorContainer container)
        {
            InitializeComponent();
            _timeManager = timeManager;
            _container = container;
            DataContext = _container.Resolve<WelcomePageViewModel>();
            var viewModel = (WelcomePageViewModel) DataContext;

            viewModel.TimeManager = _timeManager;
        }
    }
}
