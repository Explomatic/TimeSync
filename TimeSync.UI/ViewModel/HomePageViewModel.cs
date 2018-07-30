using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
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
    [Interceptor(typeof(ExceptionInterceptor))]
    public class HomePageViewModel : BaseViewModel
    {
        private string _versionNumber = TimeSync.UI.Properties.Resources.versionNumber;
        private string _basePageTitle = "TimeSync (ALPHA)";
        private string _currentPageTitle = "";

        private TimeManager _timeManager;
        private UserPage _userPage;
        private ToolkitsPage _toolkitsPage;
        private TimeregistrationsPage _timeregistrationsPage;
        private BugReportPage _bugReportPage;
        private WelcomePage _welcomePage;
        private Page _settingsPage = null;
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

            _timeManager = _container.Resolve<TimeManager>();
            _userPage = new UserPage(_timeManager, _container);
            _toolkitsPage = new ToolkitsPage(_timeManager, _container);
            _timeregistrationsPage = new TimeregistrationsPage(_timeManager, _container);
            _bugReportPage = new BugReportPage(_timeManager, _container);
            _welcomePage = new WelcomePage();

            _currentPage = _welcomePage;
        }

        private void InitWindsor()
        {
            _container = new WindsorContainer();
            WindsorInitializer.InitializeContainer(_container);
        }

        public string VersionNumber
        {
            get { return _versionNumber; }
            set
            {
                _versionNumber = value;
                RaisePropertyChangedEvent("VersionNumber");
            }
        }

        public string PageTitle
        {
            get => _currentPageTitle.IsNullOrEmpty() ? _basePageTitle : $"{_basePageTitle} - {_currentPageTitle}";
            set
            {
                _currentPageTitle = value;
                RaisePropertyChangedEvent("PageTitle");
            }
        }

        private Page _currentPage;

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
            PageTitle = "Account information";
            CurrentPage = _userPage;
        }

        public virtual bool CanNavigateFromAccountInfo()
        {
            return _timeManager.UserInfo != null && _timeManager.UserInfo.Name?.Length > 0 &&
                   _timeManager.UserInfo.SecurePassword?.Length > 0;
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
            PageTitle = "Toolkit information";
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
            PageTitle = "Timeregistrations";
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
            PageTitle = "Bug report";
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
            PageTitle = "Settings";
            CurrentPage = _welcomePage;
        }

        public virtual bool SettingsActivated()
        {
            return _settingsPage != null;
        }
    }
}