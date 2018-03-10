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
        }

        /// <summary>
        /// Call Sharepoint and do clean up for stored timeregs
        /// </summary>
        public void Sync()
        {
            //Logging?
            //Get stored timeregs //TODO Maybe get from ViewModel instead?
            var timeregs = _timeregistrationRepository.GetData();
            //Send to Sharepoint
            SharepointClient.MakeTimeregistrations(timeregs);
        }
    }
}
