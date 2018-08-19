using Castle.Windsor;
using TimeSync.DataAccess;

namespace TimeSync.UI.ViewModel
{
    public class WelcomePageViewModel : BaseViewModel
    {
        private string _defaultMessage;
        private string _welcomeMessage;
        private TimeManager _timeManager;
        
        public WelcomePageViewModel(TimeManager timeManager)
        {
            TimeManager = timeManager;
        }

        public string WelcomeMessage
        {
            get
            {
                if (TimeManager.UserInfo != null && TimeManager.UserInfo.SecurePassword.Length > 0)
                {
                    _welcomeMessage = $"Hi {TimeManager.UserInfo.Name}, welcome back to TimeSync";
                }
                else
                {
                    _welcomeMessage = "Welcome to TimeSync";
                }

                return _welcomeMessage;
            }
            set
            {
                _welcomeMessage = value;
                RaisePropertyChangedEvent("WelcomeMessage");
            }
        }

        public TimeManager TimeManager
        {
            get => _timeManager;
            set => _timeManager = value;
        }
    }
}