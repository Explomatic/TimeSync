using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Castle.Core.Internal;
using TimeSync.UI.View;

namespace TimeSync.UI.ViewModel
{
    public class BaseViewModel : ObservableObject
    {
        private readonly string _basePageTitle = $"TimeSync {Properties.Resources.versionNumber}";
        private string _currentPageTitle = "";

        public string PageTitle
        {
            get => _currentPageTitle.IsNullOrEmpty() ? _basePageTitle : $"{_basePageTitle} - {_currentPageTitle}";
            set
            {
                _currentPageTitle = value;
                RaisePropertyChangedEvent("PageTitle");
            }
        }

        protected bool pop;
        public bool PopupIsOpen
        {
            get => pop;
            set
            {
                pop = value;
                RaisePropertyChangedEvent("PopupIsOpen");
            }
        }

        protected string popText = "Popup test bro";
        public string PopupText
        {
            get => popText;
            set
            {
                popText = value;
                RaisePropertyChangedEvent("PopupText");
            }
        }

        public ICommand HideShowPopupCommand => new DelegateCommand(HideShowPopup);

        public void HideShowPopup()
        {
            PopupText = "Popup test bro";
            PopupIsOpen = !PopupIsOpen;
        }

        public ICommand ClosePopupCommand => new DelegateCommand(ClosePopup);

        public void ClosePopup()
        {
            PopupIsOpen = false;
        }

        public void OpenExceptionViewer(string message, Exception e)
        {
            var ev = new ExceptionViewer(message, e);
            ev.ShowDialog();
        }
    }
}
