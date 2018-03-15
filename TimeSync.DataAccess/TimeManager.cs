using Castle.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TimeSync.IoC;
using TimeSync.Model;

namespace TimeSync.DataAccess
{
    [Interceptor(typeof(LoggingInterceptor))]
    public class TimeManager : INotifyPropertyChanged
    {
        public ToolkitUser UserInfo;
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly IRepository<ToolkitUser> _toolkitUserRepository;
        private readonly IRepository<ToolkitInfo> _toolkitInfoRepository;
        private readonly IRepository<List<Timeregistration>> _timeregistrationRepository;
        private readonly ISharepointClient _sharepointClient;
        private ToolkitInfo _toolkitInfo;
        private ToolkitUser _toolkitUser;

        public TimeManager()
        {
            UserInfo = new ToolkitUser();
            _toolkitUserRepository = new Repository<ToolkitUser>("ToolkitUserSaveLocation");
            _toolkitInfoRepository = new Repository<ToolkitInfo>("ToolkitInfoSaveLocation");
            _timeregistrationRepository = new Repository<List<Timeregistration>>("TimeregistrationSaveLocation");
            _sharepointClient = new SharepointClient();

            UserInfo = _toolkitUserRepository.GetData();
        }

        public TimeManager(IRepository<ToolkitUser> userRepo, IRepository<ToolkitInfo> infoRepo,
            IRepository<List<Timeregistration>> timeregRepo
            , ISharepointClient spClient)
        {
            _toolkitUserRepository = userRepo;
            _toolkitInfoRepository = infoRepo;
            _timeregistrationRepository = timeregRepo;
            _sharepointClient = spClient;
        }
        /// <summary>
        /// Stores timeregs that have been input by user in "DB"
        /// </summary>
        public void StoreTimereg(List<Timeregistration> timeRegs)
        {
            //Logging?
            //Validation?
            //Call Repo
            _timeregistrationRepository.SaveData(timeRegs);
        }

        public void SaveToolkitInfo(ToolkitUser toolkitUser, ToolkitInfo toolkitInfo)
        {
            //Get user ids
            foreach (var tk in toolkitInfo.Toolkits.Select(kvp => kvp.Value))
            {
                if (tk.UserId == 0)
                    tk.UserId = _sharepointClient.GetUserIdFromToolkit(toolkitUser, tk);
            }

            //call repo to save
            _toolkitInfoRepository.SaveData(toolkitInfo);

            _toolkitInfo = toolkitInfo;
            _toolkitUser = toolkitUser;
        }

        /// <summary>
        /// Call Sharepoint and do clean up for stored timeregs
        /// </summary>
        public void Sync(List<Timeregistration> timeregs)
        {
            //Logging?
            //Get stored timeregs //TODO Maybe get from ViewModel instead?
            //var timeregs = _timeregistrationRepository.GetData();
            var userInfo = _toolkitUser ?? _toolkitUserRepository.GetData();
            var toolkitInfo = _toolkitInfo ?? _toolkitInfoRepository.GetData();


            //Send to Sharepoint
            foreach (var timereg in timeregs.Where(tr => !tr.IsSynchronized))
            {
                var id = _sharepointClient.MakeTimeregistration(timereg, userInfo, toolkitInfo);

                if (id == -1)
                {
                    timereg.IsSynchronized = false;
                }
                else
                {
                    timereg.IsSynchronized = true;
                }
            }


            _timeregistrationRepository.SaveData(timeregs);
        }
    }
}
