using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeSync.DataAccess;
using TimeSync.Model;

namespace TimeSync.UI.ViewModel
{
    class ToolkitUserViewModel : ObservableObject
    {
        private readonly IRepository<ToolkitUser> _repo = new Repository<ToolkitUser>();
        private readonly ToolkitUser _toolkitUser = new ToolkitUser();
        private string _name;
        private SecureString _password;

        public string Username
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChangedEvent("Username");
            }
        }

        public SecureString Password
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChangedEvent("Password");
            }
        }

        public ICommand UpdateToolkitUserCommand
        {
            get { return new DelegateCommand(UpdateToolkitUser); }
        }

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

            _repo.SaveData(_toolkitUser);

        }
    }
}
