using System;
using System.Collections.Generic;
using System.Configuration;
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
        private string _versionNumber = TimeSync.UI.Properties.Resources.versionNumber;
        private string _basePageTitle = "TimeSync (ALPHA)";
        private string _currentPageTitle = "";

        public string VersionNumber
        {
            get { return _versionNumber; }
            set
            {
                _versionNumber = value;
                RaisePropertyChangedEvent("VersionNumber");
            }
        }

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
            get { return pop; }
            set
            {
                pop = value;
                RaisePropertyChangedEvent("PopupIsOpen");
            }
        }

        protected string popText = "Popup test bro";
        public string PopupText
        {
            get { return popText; }
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
