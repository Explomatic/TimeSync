using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using TimeSync.DataAccess;
using TimeSync.Model;

namespace TimeSync.UI.View
{
    public partial class UserPage : Page
    {
        private bool _usernameTextBoxInitialFocus = false;
        private bool _passwordTextBoxInitialFocus = false;
        private Repository<ToolkitUser> _repo = new Repository<ToolkitUser>();
        public TimeManager TimeManager;

        public UserPage()
        {
            InitializeComponent();
        }

        public UserPage(TimeManager timeManager)
        {
            InitializeComponent();
            TimeManager = timeManager;
        }

        public void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((dynamic)this.DataContext).Password = ((PasswordBox)sender).SecurePassword; }
            //PasswordBox password = sender as PasswordBox;
            //DataAccess.TimeManager.UserInfo.Password = password.SecurePassword;
        }

        public void ClickSaveData(object sender, RoutedEventArgs e)
        {
            bool dataIsSaved = _repo.SaveData(TimeManager.UserInfo);
            if (dataIsSaved)
            {
                MessageBox.Show("Data has been saved!");
            }

        }

        private void ClickUserAccount(object sender, RoutedEventArgs e)
        {
            UserPage userPage = new UserPage();
            NavigationService.Navigate(userPage);
        }

        private void ClickToolkits(object sender, RoutedEventArgs e)
        {
            ToolkitsPage toolkitsPage = new ToolkitsPage();
            NavigationService.Navigate(toolkitsPage, this);
        }

        private void ClickTimeregistrations(object sender, RoutedEventArgs e)
        {
            TimeregistrationsPage timeregistrationsPage = new TimeregistrationsPage();
            NavigationService.Navigate(timeregistrationsPage);
        }

        public void SaveUserInfo(object sender, RoutedEventArgs e)
        {
            ToolkitUser userInfo = sender as ToolkitUser;
            _repo.SaveData(userInfo);
        }

        //private void UsernameTBox_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    if (!_usernameTextBoxInitialFocus)
        //    {
        //        TextBox TBox = sender as TextBox;
        //        TBox.Text = "";
        //        _usernameTextBoxInitialFocus = true;
        //    }
        //}

        //private void UsernameTBox_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    TextBox TBox = sender as TextBox;
        //    var text = TBox.Text;
        //    if (String.IsNullOrEmpty(text))
        //    {
        //        TBox.Text = "Initialis";
        //        _usernameTextBoxInitialFocus = true;
        //    }
        //}

        //private void UsernameRichTextBox_GotFocus(object sender, RoutedEventArgs e)
        //{

        //    if (!_usernameTextBoxInitialFocus)
        //    {
        //        RichTextBox rbox = sender as RichTextBox;
        //        rbox.Document.Blocks.Clear();
        //        _usernameTextBoxInitialFocus = true;
        //    }
        //}

        //private void UsernameRichTextBox_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    RichTextBox rbox = sender as RichTextBox;
        //    string richText = new TextRange(rbox.Document.ContentStart, rbox.Document.ContentEnd).Text;
        //    if (string.IsNullOrEmpty(richText))
        //    {
        //        Paragraph richParagraph = new Paragraph();
        //        richParagraph.Inlines.Add(new Run("Initials"));
        //        richParagraph.FontSize = 12;
        //        richParagraph.FontStyle = FontStyles.Italic;
        //        richParagraph.Foreground = Brushes.Gray;
        //        richParagraph.FontWeight = FontWeights.Light;
        //        rbox.Document.Blocks.Add(richParagraph);
        //        _usernameTextBoxInitialFocus = false;
        //    }
        //}

        private void PasswordRichTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!_passwordTextBoxInitialFocus)
            {
                RichTextBox rbox = sender as RichTextBox;
                rbox.Document.Blocks.Clear();
                _passwordTextBoxInitialFocus = true;
            }
        }

        

        private void PasswordRichTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            RichTextBox rbox = sender as RichTextBox;
            string richText = new TextRange(rbox.Document.ContentStart, rbox.Document.ContentEnd).Text;
            if (string.IsNullOrEmpty(richText))
            {
                Paragraph richParagraph = new Paragraph();
                richParagraph.Inlines.Add(new Run("Password"));
                richParagraph.FontSize = 12;
                richParagraph.FontStyle = FontStyles.Italic;
                richParagraph.Foreground = Brushes.Gray;
                richParagraph.FontWeight = FontWeights.Light;
                rbox.Document.Blocks.Add(richParagraph);
                _passwordTextBoxInitialFocus = false;
            }
        }
    }
}
