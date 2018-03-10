using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeSync.Model;

namespace TimeSync.DataAccess
{
    public class TimeManager : INotifyPropertyChanged
    {
        public ToolkitUser UserInfo;
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Repository<ToolkitUser> _toolkitUserRepository;
        private readonly Repository<ToolkitInfo> _toolkitInfoRepository;
        private readonly Repository<List<Timeregistration>> _timeregistrationRepository;
        private ToolkitInfo _toolkitInfo;
        private ToolkitUser _toolkitUser;

        public TimeManager()
        {
            UserInfo = new ToolkitUser();
            _toolkitUserRepository = new Repository<ToolkitUser>("ToolkitUserSaveLocation");
            _toolkitInfoRepository = new Repository<ToolkitInfo>("ToolkitInfoSaveLocation");
            _timeregistrationRepository = new Repository<List<Timeregistration>>("TimeregistrationSaveLocation");
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
                    tk.UserId = SharepointClient.GetUserIdFromToolkit(toolkitUser, tk);
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
                var id = SharepointClient.MakeTimeregistration(timereg, userInfo, toolkitInfo);

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
