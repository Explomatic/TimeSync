using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace TimeSync.UI.ViewModel
{
    public class SettingsViewModel : BaseViewModel
    {
        private Color _buttonHoverColor;

        public SettingsViewModel()
        {
            _buttonHoverColor = (Color) Application.Current.FindResource("ButtonHoverColor");
        }

        public Color ButtonHoverColor
        {
            get => _buttonHoverColor;
            set
            {
                _buttonHoverColor = value;
                Application.Current.Resources["ButtonHoverColor"] = value;
                RaisePropertyChangedEvent("ButtonHoverColor");
            }
        }
    }
}
