using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeSync.DataAccess;
using TimeSync.Model;

namespace TimeSync.UI.ViewModel
{
    class ToolkitUserViewModel : BaseViewModel
    {
        private readonly IRepository<ToolkitUser> _repo;
        private readonly ToolkitUser _toolkitUser;
        private string _name;
        private string _password;

        public TimeManager TimeManager { get; set; }

        public ToolkitUserViewModel()
        {
            _repo = new Repository<ToolkitUser>("ToolkitUserSaveLocation");
            _toolkitUser = _repo.GetData();
            Username = _toolkitUser.Name;            
        }
        public string Username
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChangedEvent("Username");
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChangedEvent("Password");
            }
        }

        public ICommand UpdateToolkitUserCommand => new DelegateCommand(UpdateToolkitUser);

        public void UpdateToolkitUser()
        {
            if (!string.IsNullOrWhiteSpace(Username))
            {
                _toolkitUser.Name = _name;
            }

            if (Password.Length > 0)
            {
                _toolkitUser.Password = _password;
            }
            //TODO Fix these lines. Roll into one function on TimeManager. ViewModel shouldn't think/know about how TimeManager's data/fields look
            TimeManager.UserInfo = _toolkitUser; 
            _repo.SaveData(_toolkitUser);

        }
    }
}
