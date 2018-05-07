using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Castle.Core;
using TimeSync.DataAccess;
using TimeSync.IoC;
using TimeSync.Model;

namespace TimeSync.UI.ViewModel
{
    [Interceptor(typeof(ExceptionInterceptor))]
    public class ToolkitUserViewModel : BaseViewModel
    {
        private readonly IRepository<ToolkitUser> _repo;
        private readonly ToolkitUser _toolkitUser;
        private string _name;
        private string _password;
        private SecureString _securePassword;
        private bool _clearPasswordBox = false;


        public bool ClearPasswordBox
        {
            get { return _clearPasswordBox; }
            set
            {
                _clearPasswordBox = value;
                if (_clearPasswordBox) RaisePropertyChangedEvent("ClearPasswordBox");
            }
        }

        public bool ToSavePassword { get; set; } = false;

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
                _password = $@"{value}";
                RaisePropertyChangedEvent("Password");
            }
        }

        public ICommand UpdateToolkitUserCommand => new DelegateCommand(UpdateToolkitUser);

        public virtual void UpdateToolkitUser()
        {
            if (!string.IsNullOrWhiteSpace(Username))
            {
                _toolkitUser.Name = Username;
            }

            _toolkitUser.ToSavePassword = ToSavePassword;

            _toolkitUser.Password = _toolkitUser.ToSavePassword ? Password : "";
            _toolkitUser.SecurePassword = new NetworkCredential("", Password).SecurePassword;
            Password = _toolkitUser.ToSavePassword ? Password : "";

            //TODO Fix these lines. Roll into one function on TimeManager. ViewModel shouldn't think/know about how TimeManager's data/fields look
            TimeManager.UserInfo = _toolkitUser; 
            _repo.SaveData(_toolkitUser);

            ClearPasswordBox = !ToSavePassword;
        }
    }
}
