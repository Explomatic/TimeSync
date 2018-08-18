using System.Windows.Controls;
using TimeSync.UI.ViewModel;

namespace TimeSync.UI.View
{
    /// <summary>
    /// Interaction logic for WelcomePage.xaml
    /// </summary>
    public partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        public WelcomePage(WelcomePageViewModel vm) : this()
        {
            this.DataContext = vm;
        }
    }
}
