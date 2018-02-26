using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSync.Model
{

    public class UserInfo : INotifyPropertyChanged
    {
        private string _userInitials;
        private string _userPassword;

        #region Properties setters and getters
        public string UserInitials
        {
            get { return _userInitials; }
            set
            {
                _userInitials = value;
                OnPropertyChanged("UserInitials");
            }
        }

        public string UserPassword
        {
            get { return _userPassword; }
            set
            {
                _userPassword = value;
                OnPropertyChanged("UserPassword");
            }
        }
        #endregion

        public List<ToolkitInfo> ToolkitInfos;
        public readonly string Domain;

        public event PropertyChangedEventHandler PropertyChanged;

        public UserInfo()
        {
            ToolkitInfos = new List<ToolkitInfo>();
            Domain = "NCDMZ";
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
