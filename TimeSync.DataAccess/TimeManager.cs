using Castle.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TimeSync.IoC;
using TimeSync.Model;
using System;
using System.Net;
using Castle.Core.Internal;
using Microsoft.SharePoint.Client;

namespace TimeSync.DataAccess
{
    [Interceptor(typeof(LoggingInterceptor))]
    public class TimeManager
    {
        private readonly IRepository<ToolkitUser> _toolkitUserRepository;
        private readonly IRepository<List<Toolkit>> _toolkitRepository;
        private readonly IRepository<List<Timeregistration>> _timeregistrationRepository;
        private readonly ISharepointClient _sharepointClient;
        private ToolkitUser _toolkitUser;
        private List<Toolkit> _toolkits;
        
        public ToolkitUser UserInfo;

        public TimeManager(IRepository<ToolkitUser> tkUserRepo, IRepository<List<Toolkit>> tkRepo, 
            IRepository<List<Timeregistration>> timeregRepo, ISharepointClient spClient, IEncryption encryptionManager)
        {
            _toolkitUserRepository = tkUserRepo;
            _toolkitRepository = tkRepo;
            _timeregistrationRepository = timeregRepo;
            _sharepointClient = spClient;

            UserInfo = _toolkitUserRepository.GetData();
            if (UserInfo.Password?.Length > 0)
            {
                UserInfo.SecurePassword = new NetworkCredential("", encryptionManager.DecryptText(UserInfo.Password))
                    .SecurePassword;
            }
            
            _toolkits = _toolkitRepository.GetData();
        }

        /// <summary>
        /// Stores timeregs that have been input by user in "DB"
        /// </summary>
        public virtual bool SaveTimeregistrations(List<Timeregistration> timeregs)
        {
            //Logging?
            //Validation?
            //Call Repo
            return _timeregistrationRepository.SaveData(timeregs);
        }

        /// <summary>
        /// Stores toolkits that have been input by user in "DB"
        /// </summary>
        public virtual bool SaveToolkits(List<Toolkit> toolkits)
        {
            //Logging?
            //Validation?
            //Call Repo
            return _toolkitRepository.SaveData(toolkits);
        }
        
        /// <summary>
        /// Stores toolkit user information that have been input by user in "DB"
        /// </summary>
        public virtual bool SaveToolkitUser()
        {
            //Logging?
            //Validation?
            //Call Repo
            return _toolkitUserRepository.SaveData(UserInfo);
        }
        
        public virtual IEnumerable<Toolkit> GetToolkits()
        {
            return _toolkitRepository.GetData();
        }

        public virtual void SyncToolkits(ToolkitUser toolkitUser, List<Toolkit> toolkits)
        {
            //Get user ids
            //foreach (var tk in toolkits)

            for (var i=0; i < toolkits.Count; i++)
            {
                var tk = toolkits[i];

                tk.CustomerName = GetCustomerNameFromUrl(tk);

                if (tk.UserId == 0)
                    tk.UserId = _sharepointClient.GetUserIdFromToolkit(toolkitUser, tk);

                if (_toolkits.IsNullOrEmpty() || ToolkitInfoChanged(tk,_toolkits))
                {
                    tk.Teams = _sharepointClient.GetTeamsFromToolkit(toolkitUser, tk);
                    tk = _sharepointClient.GetTimeslotInformationFromToolkit(toolkitUser, tk);
                }

                toolkits[i] = tk;
            }

            //call repo to save
            _toolkitRepository.SaveData(toolkits);

            _toolkits = toolkits;
            _toolkitUser = toolkitUser;
        }

        private static string GetCustomerNameFromUrl(Toolkit tk)
        {
            var customerName = tk.Url.Split('/');
            return customerName.Last().IsNullOrEmpty() ? customerName[customerName.Length - 2] : customerName.Last();
        }

        private static bool ToolkitInfoChanged(Toolkit newTk, IEnumerable<Toolkit> tks)
        {
            var currTk = tks?.SingleOrDefault(tk => tk.Url == newTk.Url);
            return currTk?.GetTeamsWithoutSLA != newTk.GetTeamsWithoutSLA;

        }

        /// <summary>
        /// Call Sharepoint and do clean up for stored timeregs
        /// </summary>
        public virtual void SyncTimeregs(List<Timeregistration> timeregs)
        {
            //TODO: Add logging + give user feedback when timeregs are synchronised.
            _toolkitUser = _toolkitUser ?? _toolkitUserRepository.GetData();
            _toolkits = _toolkits ?? _toolkitRepository.GetData();

            var distinctTkDisplayNames = timeregs.Select(x => x.ToolkitDisplayName).Distinct();
            foreach (var tkDisplayName in distinctTkDisplayNames)
            {
                var timeregsInTk = timeregs.Where(x => x.ToolkitDisplayName == tkDisplayName).ToList();
                var tk = _toolkits.Single(x => x.DisplayName == tkDisplayName);
                _sharepointClient.MakeTimeregistrations(timeregsInTk, _toolkitUser, tk);
            }

            //Send to Sharepoint
            foreach (var timereg in timeregs.Where(tr => !tr.IsSynchronized))
            {
                var toolkit = GetRelevantToolkit(timereg);
                var id = _sharepointClient.MakeTimeregistration(timereg, _toolkitUser, toolkit);

                timereg.IsSynchronized = id != -1;
            }

            _timeregistrationRepository.SaveData(timeregs);
        }

        private Toolkit GetRelevantToolkit(Timeregistration timereg)
        {
            var toolkit = _toolkits.SingleOrDefault(tk => tk.DisplayName == timereg.ToolkitDisplayName);
            if (toolkit == null)
                throw new Exception("Toolkit not found");

            return toolkit;
        }

        public IEnumerable<Timeregistration> GetTimeregistrations()
        {
            return _timeregistrationRepository.GetData().Where(t => !t.IsSynchronized);
        }
    }
}
