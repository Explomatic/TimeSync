using System;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Input;
using Castle.Core;
using TimeSync.IoC;

namespace TimeSync.UI.ViewModel
{
    [Interceptor(typeof(ExceptionInterceptor))]
    public class BugReportViewModel : BaseViewModel
    {
        private Uri _issuesUri;
        private string _message;
        private Hyperlink _issuesLink;
        private RelayCommand _navigateCommand;

        public BugReportViewModel()
        {
            _issuesUri = new Uri("https://github.com/NCTimeSync/TimeSync/issues");
            _message =
                "Hi! We're sorry to hear you have experienced issues with TimeSync.\nPlease visit us at our GitHub in order to submit an issue.\nGitHub:";
//            _issuesLink = new Hyperlink {NavigateUri = _issuesUri, Command = NavigateCommand, Inlines = { _issuesUri.ToString() }};
        }

        public Uri IssuesUri
        {
            get => _issuesUri; 
            set => _issuesUri = value; 
        }

        public string Message
        {
            get => _message;
            set => _message = value;
        }

        public virtual ICommand NavigateCommand
        {
            get
            {
                _navigateCommand = new RelayCommand(param => Navigate(), param => true);
                return _navigateCommand;    
            }
        }

        public virtual void Navigate()
        {
            Process.Start(new ProcessStartInfo(IssuesUri.AbsoluteUri));
        }
    }
}