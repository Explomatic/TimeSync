using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TimeSync.UI.ViewModel
{
    public class BaseViewModel : ObservableObject
    {
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
            PopupIsOpen = !PopupIsOpen;
        }

        public ICommand ClosePopupCommand => new DelegateCommand(ClosePopup);

        public void ClosePopup()
        {
            PopupIsOpen = false;
        }
    }
}
