﻿using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Castle.Windsor;
using log4net.Config;
using TimeSync.DataAccess;
using TimeSync.UI.IoC;

namespace TimeSync.UI.View
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private TimeManager _timeManager;
        private UserPage _userPage;
        private ToolkitsPage _toolkitsPage;
        private TimeregistrationsPage _timeregistrationsPage;
        private BugReportPage _bugReportPage;
        private IWindsorContainer _container;

        public HomePage()
        {
            InitializeComponent();

            InitWindsor();
            //TODO Maybe set up our own LoggingManager class to wrap configuration and instantiation of loggers. Encapsulation principle!
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/Log4Net.config"));
            //var logger = log4net.LogManager.GetLogger("Test");
            //logger.Debug("It works!");
            _timeManager = _container.Resolve<TimeManager>();
            _userPage = new UserPage(_timeManager, _container);
            _toolkitsPage = new ToolkitsPage(_timeManager, _container);
            _timeregistrationsPage = new TimeregistrationsPage(_timeManager, _container);
            _bugReportPage = new BugReportPage(_timeManager, _container);

            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
            Thread.CurrentThread.CurrentCulture = ci;
        }

        private void InitWindsor()
        {
            _container = new WindsorContainer();
            WindsorInitializer.InitializeContainer(_container);
        }

        private void ButtonBase_OnClickUserAccount(object sender, RoutedEventArgs e)
        {
            ContentGrid.Children.Clear();
            var frame = new Frame();
            frame.Navigate(_userPage);
            ContentGrid.Children.Add(frame);
        }

        private void ButtonBase_OnClickToolkits(object sender, RoutedEventArgs e)
        {
            ContentGrid.Children.Clear();
            var frame = new Frame();
            frame.Navigate(_toolkitsPage);
            ContentGrid.Children.Add(frame);
        }

        private void ButtonBase_OnClickTimeregistrations(object sender, RoutedEventArgs e)
        {
            ContentGrid.Children.Clear();
            var frame = new Frame();
            frame.Navigate(_timeregistrationsPage);
            ContentGrid.Children.Add(frame);
        }

        private void ButtonBase_OnClickBugReport(object sender, RoutedEventArgs e)
        {
            ContentGrid.Children.Clear();
            var frame = new Frame();
            frame.Navigate(_bugReportPage);
            ContentGrid.Children.Add(frame);
        }
    }
}
