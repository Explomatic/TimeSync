using System.Diagnostics;
using System.Net;
using System.Windows.Controls;
using System.Windows.Navigation;
using TimeSync.UI.ViewModel;

namespace TimeSync.UI.View
{
    
    public partial class BugReportPage : Page
    {
        public BugReportPage(BugReportViewModel vm)
        {
            InitializeComponent();
            
            //TODO: Fix opening links in view model using commands before using the viewmodel
//            DataContext = vm;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
