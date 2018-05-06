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
            //foreach (var tk in toolkits)

            for (var i=0; i < toolkits.Count(); i++)
            {
                var tk = toolkits[i];

                if (tk.UserId == 0)
                    tk.UserId = _sharepointClient.GetUserIdFromToolkit(toolkitUser, tk);

                tk.Teams = _sharepointClient.GetTeamsFromToolkit(toolkitUser, tk);
                tk = _sharepointClient.GetTimeslotInformationFromToolkit(toolkitUser, tk);

                toolkits[i] = tk;
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
            //TODO: Add logging + give user feedback when timeregs are synchronised.
            //Logging?
            _toolkitUser = _toolkitUser ?? _toolkitUserRepository.GetData();
            //var toolkitInfo = _toolkitInfo ?? _toolkitInfoRepository.GetData();
            _toolkits = _toolkits ?? _toolkitRepository.GetData();

            var uniqueToolkitNames = new List<string>();
            foreach (var timereg in timeregs)
            {
                if (uniqueToolkitNames.Contains(timereg.Customer)) continue;
                uniqueToolkitNames.Add(timereg.Customer);
            }

            foreach (var toolkitName in uniqueToolkitNames)
            {
                var tkTimeregs = (from timereg in timeregs where timereg.Customer == toolkitName select timereg).ToList();
                _sharepointClient.MakeTimeregistrations(tkTimeregs, _toolkitUser, _toolkits.Single(tk => tk.CustomerName == toolkitName));
            }

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
