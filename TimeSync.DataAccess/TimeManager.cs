using Castle.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TimeSync.IoC;
using TimeSync.Model;
using System;

namespace TimeSync.DataAccess
{
    [Interceptor(typeof(LoggingInterceptor))]
    public class TimeManager : INotifyPropertyChanged
    {
        public ToolkitUser UserInfo;
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly IRepository<ToolkitUser> _toolkitUserRepository;
        private readonly IRepository<List<Toolkit>> _toolkitRepository;
        private readonly IRepository<List<Timeregistration>> _timeregistrationRepository;
        private readonly ISharepointClient _sharepointClient;
        private ToolkitUser _toolkitUser;
        private List<Toolkit> _toolkits;

        public TimeManager()
        {
            UserInfo = new ToolkitUser();
            _toolkitUserRepository = new Repository<ToolkitUser>("ToolkitUserSaveLocation");
            _toolkitRepository = new Repository<List<Toolkit>>("ToolkitSaveLocation");
            _timeregistrationRepository = new Repository<List<Timeregistration>>("TimeregistrationSaveLocation");
            _sharepointClient = new SharepointClient();

            UserInfo = _toolkitUserRepository.GetData();
        }

        public TimeManager(IRepository<ToolkitUser> userRepo, IRepository<List<Toolkit>> toolkitRepo,
            IRepository<List<Timeregistration>> timeregRepo
            , ISharepointClient spClient)
        {
            _toolkitUserRepository = userRepo;
            _toolkitRepository = toolkitRepo;
            _timeregistrationRepository = timeregRepo;
            _sharepointClient = spClient;
        }
        /// <summary>
        /// Stores timeregs that have been input by user in "DB"
        /// </summary>
        public virtual void StoreTimereg(List<Timeregistration> timeRegs)
        {
            //Logging?
            //Validation?
            //Call Repo
            _timeregistrationRepository.SaveData(timeRegs);
        }

        public virtual void SaveToolkitInfo(ToolkitUser toolkitUser, List<Toolkit> toolkits)
        {
            //Get user ids
            foreach (var tk in toolkits)
            {
                if (tk.UserId == 0)
                    tk.UserId = _sharepointClient.GetUserIdFromToolkit(toolkitUser, tk);

                tk.Teams = _sharepointClient.GetTeamsFromToolkit(toolkitUser, tk);
                tk.Teams = _sharepointClient.CheckForTimeSlots(toolkitUser, tk);
            }

            

            //call repo to save
            _toolkitRepository.SaveData(toolkits);

            _toolkits = toolkits;
            _toolkitUser = toolkitUser;
        }

        /// <summary>
        /// Call Sharepoint and do clean up for stored timeregs
        /// </summary>
        public virtual void Sync(List<Timeregistration> timeregs)
        {
            //Logging?
            //Get stored timeregs //TODO Maybe get from ViewModel instead?
            //var timeregs = _timeregistrationRepository.GetData();
            _toolkitUser = _toolkitUser ?? _toolkitUserRepository.GetData();
            //var toolkitInfo = _toolkitInfo ?? _toolkitInfoRepository.GetData();
            _toolkits = _toolkits ?? _toolkitRepository.GetData();


            //Send to Sharepoint
            foreach (var timereg in timeregs.Where(tr => !tr.IsSynchronized))
            {
                var toolkit = GetRelevantToolkit(timereg);
                var id = _sharepointClient.MakeTimeregistration(timereg, _toolkitUser, toolkit);

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

        private Toolkit GetRelevantToolkit(Timeregistration timereg)
        {
            var toolkit = _toolkits.SingleOrDefault(tk => tk.CustomerName == timereg.Customer);
            if (toolkit == null)
                throw new Exception("Customer not found");

            return toolkit;
        }
    }
}
