using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Castle.Core;
using Castle.Core.Internal;
using Castle.Windsor;
using log4net.Config;
using TimeSync.DataAccess;
using TimeSync.IoC;
using TimeSync.UI.IoC;
using TimeSync.UI.View;

namespace TimeSync.UI.ViewModel
{
    [Interceptor(typeof(ExceptionInterceptor)),Interceptor(typeof(LoggingInterceptor))]
    public class HomePageViewModel : BaseViewModel
    {
        private TimeManager _timeManager;
        private readonly UserPage _userPage;
        private readonly ToolkitsPage _toolkitsPage;
        private readonly TimeregistrationsPage _timeregistrationsPage;
        private readonly BugReportPage _bugReportPage;
        private readonly WelcomePage _welcomePage;
        private readonly SettingsPage _settingsPage;
        private Page _currentPage;
        private IWindsorContainer _container;

        private RelayCommand _accountInfoCommand;
        private RelayCommand _toolkitsCommand;
        private RelayCommand _timeregistrationCommand;
        private RelayCommand _bugReportCommand;
        private RelayCommand _settingsCommand;
        
        

        public HomePageViewModel()
        {
            InitWindsor();
            
            //TODO Maybe set up our own LoggingManager class to wrap configuration and instantiation of loggers. Encapsulation principle!
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/Log4Net.config"));
            var logger = log4net.LogManager.GetLogger("Test");
            logger.Debug("It works!");

            // TODO: Move these container resolves from HomePageViewModel to the App class.
            TimeManager = _container.Resolve<TimeManager>();
            _userPage = _container.Resolve<UserPage>();
            _toolkitsPage = _container.Resolve<ToolkitsPage>();
            _timeregistrationsPage = _container.Resolve<TimeregistrationsPage>();
            _bugReportPage = _container.Resolve<BugReportPage>();
            _welcomePage = _container.Resolve<WelcomePage>();
            //_settingsPage = _container.Resolve<SettingsPage>();
            _settingsPage = null;

            _currentPage = _welcomePage;

            PageTitle = TimeManager.UserInfo.Name;
        }

        private void InitWindsor()
        {
            _container = new WindsorContainer();
            WindsorInitializer.InitializeContainer(_container);
        }

        public Page CurrentPage
        {
            get => _currentPage;

            set
            {
                _currentPage = value;
                RaisePropertyChangedEvent("CurrentPage");
            }
        }

        public TimeManager TimeManager
        {
            get => _timeManager;

            set => _timeManager = value;
        }

        public ICommand AccountInfoCommand
        {
            get
            {
                _accountInfoCommand = new RelayCommand(param => AccountInfo(), param => true);
                return _accountInfoCommand;
            }
        }

        public virtual void AccountInfo()
        {
            CurrentPage = _userPage;
        }

        public virtual bool CanNavigateFromAccountInfo()
        {
            return TimeManager.UserInfo != null && TimeManager.UserInfo.Name?.Length > 0 &&
                   TimeManager.UserInfo.SecurePassword?.Length > 0;
        }

        public ICommand ToolkitsCommand
        {
            get
            {
                _toolkitsCommand = new RelayCommand(param => Toolkits(), param => CanNavigateFromAccountInfo());
                return _toolkitsCommand;
            }
        }

        public virtual void Toolkits()
        {
            CurrentPage = _toolkitsPage;
        }

        public ICommand TimeregistrationCommand
        {
            get
            {
                _timeregistrationCommand = new RelayCommand(param => Timeregistration(), param => CanNavigateFromAccountInfo());
                return _timeregistrationCommand;
            }
        }

        public virtual void Timeregistration()
        {
            CurrentPage = _timeregistrationsPage;
        }

        public ICommand BugReportCommand
        {
            get
            {
                _bugReportCommand = new RelayCommand(param => BugReport(), param => true);
                return _bugReportCommand;
            }
        }

        public virtual void BugReport()
        {
            CurrentPage = _bugReportPage;
        }

        public ICommand SettingsCommand
        {
            get
            {
                _settingsCommand = new RelayCommand(param => Settings(), param => SettingsActivated());
                return _settingsCommand;
            }
        }

        public virtual void Settings()
        {
            CurrentPage = _settingsPage;
        }

        public virtual bool SettingsActivated()
        {
            return _settingsPage != null;
        }
    }
    
    [ValueConversion(typeof(string), typeof(SolidColorBrush))]
    public sealed class StringToSolidColorBrushConverter : IMultiValueConverter
    {
        public SolidColorBrush TrueValue { get; set; }
        public SolidColorBrush FalseValue { get; set; }

        public StringToSolidColorBrushConverter()
        {
            // set defaults
            TrueValue = (SolidColorBrush) Application.Current.FindResource("ButtonHoverBrush");
            FalseValue = (SolidColorBrush) Application.Current.FindResource("WindowButtonBrush");
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var button = (Button) values[0];
            var currentPageTitle = (string) values[1];
            return (string) button.Content == currentPageTitle ? TrueValue : FalseValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}