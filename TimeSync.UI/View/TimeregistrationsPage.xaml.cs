using System;
using System.Windows;
using System.Windows.Controls;
using Castle.Windsor;
using TimeSync.DataAccess;
using TimeSync.Model;
using TimeSync.UI.ViewModel;

namespace TimeSync.UI.View
{
    public partial class TimeregistrationsPage : Page
    {
        private TimeManager _timeManager;

        private IWindsorContainer _container;
        public TimeregistrationsPage()
        {
            InitializeComponent();
        }

        public TimeregistrationsPage(TimeManager timeManager, IWindsorContainer container)
        {
            InitializeComponent();
            _timeManager = timeManager;
            _container = container;
            this.DataContext = _container.Resolve<TimeregistrationViewModel>();
            var viewModel = this.DataContext as TimeregistrationViewModel;
            viewModel.TimeManager = _timeManager;
        }
    }
}
