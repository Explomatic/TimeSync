using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Castle.Core;
using TimeSync.DataAccess;
using TimeSync.IoC;
using TimeSync.Model;
using TimeSync.UI.View;

namespace TimeSync.UI.ViewModel
{
    [Interceptor(typeof(ExceptionInterceptor))]
    public class ToolkitUserViewModel : BaseViewModel
    {
        private readonly IRepository<ToolkitUser> _repo;
        private readonly ToolkitUser _toolkitUser;
        private string _name;
        private string _password;
        private bool _toSavePassword;
        private bool _clearPasswordBox;
        private bool _isDataSaved;
        private IEncrypt _encryptionManager = new Encrypt();

        public bool ClearPasswordBox
        {
            get { return _clearPasswordBox; }
            set
            {
                _clearPasswordBox = value;
                if (_clearPasswordBox) RaisePropertyChangedEvent("ClearPasswordBox");
            }
        }

        public bool ToSavePassword
        {
            get
            {
                return _toSavePassword;
            }
            set
            {
                _toSavePassword = value;
                RaisePropertyChangedEvent("ToSavePassword");
            }
        }

        public TimeManager TimeManager { get; set; }

        public bool IsDataSaved
        {
            get { return _isDataSaved; }
            set
            {
                _isDataSaved = value;
                RaisePropertyChangedEvent("IsDataSaved");
            }
        }

        public ToolkitUserViewModel()
        {
            _repo = new Repository<ToolkitUser>("ToolkitUserSaveLocation");
            _toolkitUser = _repo.GetData();
            if (!string.IsNullOrEmpty(_toolkitUser.Password))
            {
                try
                {
                    _toolkitUser.SecurePassword = new NetworkCredential("", _encryptionManager.DecryptText(_toolkitUser.Password)).SecurePassword;
                }
                catch (CryptographicException)
                {
                    popText = "Could not decrypt the saved password, please retype it.";
                    pop = true;
                    PopupIsOpen = true;
                }
            }
            
            Username = _toolkitUser.Name;
        }

        public string Username
        {
            get => _name;
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

            _toolkitUser.Password = ToSavePassword ? Password : "";
            _toolkitUser.SecurePassword = new NetworkCredential("", Password).SecurePassword;
            Password = ToSavePassword ? Password : "";

            //TODO Fix these lines. Roll into one function on TimeManager. ViewModel shouldn't think/know about how TimeManager's data/fields look
            TimeManager.UserInfo = _toolkitUser; 
            var success = _repo.SaveData(_toolkitUser);

            if (!success) return;
            ClearPasswordBox = !ToSavePassword;
            IsDataSaved = true;
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public BoolToVisibilityConverter()
        {
            // set defaults
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return null;
            return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (Equals(value, TrueValue))
                return true;
            if (Equals(value, FalseValue))
                return false;
            return null;
        }
    }
}

