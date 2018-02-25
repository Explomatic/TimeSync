using System;
using System.Collections.Generic;
using System.Linq;
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
using TimeSync.DataAccess;
using TimeSync.Model;

namespace TimeSync.UI
{
    /// <summary>
    /// Interaction logic for UserAccount.xaml
    /// </summary>
    public partial class UserAccount : Page
    {
        private bool _usernameTextBoxInitialFocus = false;
        private bool _passwordTextBoxInitialFocus = false;
        private Repository<UserInfo> _repo = new Repository<UserInfo>();

        public UserAccount()
        {
            InitializeComponent();
        }

        private void ClickUserAccount(object sender, RoutedEventArgs e)
        {
            UserAccount userAccount = new UserAccount();
            NavigationService.Navigate(userAccount);
        }

        private void ClickToolkits(object sender, RoutedEventArgs e)
        {
            Toolkits toolkits = new Toolkits();
            NavigationService.Navigate(toolkits);
        }

        private void ClickTimeregistrations(object sender, RoutedEventArgs e)
        {
            Timeregistrations timeregistrations = new Timeregistrations();
            NavigationService.Navigate(timeregistrations);
        }

        public void SaveUserInfo(object sender, RoutedEventArgs e)
        {
            UserInfo userInfo = sender as UserInfo;
            _repo.SaveUserInfo(userInfo);
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
