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

namespace TimeSync.UI
{
    /// <summary>
    /// Interaction logic for UserAccount.xaml
    /// </summary>
    public partial class UserAccount : Page
    {
        private bool _usernameTextBoxInitialFocus = false;
        private bool _passwordTextBoxInitialFocus = false;
        private SharepointClient _sharepointClient;

        public UserAccount()
        {
            InitializeComponent();
        }

        public UserAccount(SharepointClient sharepointClient)
        {
            InitializeComponent();
            _sharepointClient = sharepointClient;
        }

        private void ButtonBase_OnClickUserAccount(object sender, RoutedEventArgs e)
        {
            UserAccount userAccount = new UserAccount();
            NavigationService.Navigate(userAccount);
        }

        private void ButtonBase_OnClickToolkits(object sender, RoutedEventArgs e)
        {
            Toolkits toolkits = new Toolkits();
            NavigationService.Navigate(toolkits);
        }

        private void ButtonBase_OnClickTimeregistrations(object sender, RoutedEventArgs e)
        {
            Timeregistrations timeregistrations = new Timeregistrations();
            NavigationService.Navigate(timeregistrations);
        }

        private void UsernameRichTextBox_GotFocus(object sender, RoutedEventArgs e)
        {

            if (!_usernameTextBoxInitialFocus)
            {
                RichTextBox rbox = sender as RichTextBox;
                rbox.Document.Blocks.Clear();
                _usernameTextBoxInitialFocus = true;
            }
        }

        private void PasswordRichTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!_passwordTextBoxInitialFocus)
            {
                RichTextBox rbox = sender as RichTextBox;
                rbox.Document.Blocks.Clear();
                _passwordTextBoxInitialFocus = true;
            }
        }

        private void UsernameRichTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            RichTextBox rbox = sender as RichTextBox;
            string richText = new TextRange(rbox.Document.ContentStart, rbox.Document.ContentEnd).Text;
            if (string.IsNullOrEmpty(richText))
            {
                Paragraph richParagraph = new Paragraph();
                richParagraph.Inlines.Add(new Run("Initials"));
                richParagraph.FontSize = 12;
                richParagraph.FontStyle = FontStyles.Italic;
                richParagraph.Foreground = Brushes.Gray;
                richParagraph.FontWeight = FontWeights.Light;
                rbox.Document.Blocks.Add(richParagraph);
                _usernameTextBoxInitialFocus = false;
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
