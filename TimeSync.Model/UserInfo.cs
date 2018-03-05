using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace TimeSync.Model
{

    public class UserInfo : INotifyPropertyChanged
    {
        private string _initials;
        private SecureString _password;

        #region Properties setters and getters
        public string Initials
        {
            get { return _initials; }
            set
            {
                _initials = value;
                OnPropertyChanged("Initials");
            }
        }

        public SecureString Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
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
