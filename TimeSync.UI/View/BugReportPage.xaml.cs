﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
using Castle.Windsor;
using TimeSync.DataAccess;
using TimeSync.UI.ViewModel;

namespace TimeSync.UI.View
{
    public partial class BugReportPage : Page
    {
        private TimeManager _timeManager;
        private IWindsorContainer _container;
        public BugReportPage()
        {
            InitializeComponent();
        }

        public BugReportPage(TimeManager timeManager, IWindsorContainer container)
        {
            InitializeComponent();
            _timeManager = timeManager;
            _container = container;
            DataContext = _container.Resolve<ToolkitUserViewModel>();
            var viewModel = (ToolkitUserViewModel) DataContext;

            viewModel.TimeManager = _timeManager;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
