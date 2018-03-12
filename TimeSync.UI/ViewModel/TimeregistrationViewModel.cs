using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Windows.Input;
using TimeSync.DataAccess;
using TimeSync.Model;

namespace TimeSync.UI.ViewModel
{
    class TimeregistrationViewModel : ObservableObject
    {
        private readonly IRepository<ObservableCollection<Timeregistration>> _timeregRepo;
        private readonly TimeManager _timeManager = new TimeManager();
        private string _customerName;
        private int _caseId;
        private string _hours;
        private DateTime _doneDate;
        private bool _synchronised;

        public ObservableCollection<Timeregistration> Timeregistrations { get; set; }

        public ObservableCollection<string> ListOfToolkitNames { get; set; }
        

        public TimeregistrationViewModel()
        {
            _timeregRepo = new Repository<ObservableCollection<Timeregistration>>("TimeregistrationSaveLocation");
            Timeregistrations = new ObservableCollection<Timeregistration>();
            Timeregistrations = _timeregRepo.GetData();

            ListOfToolkitNames = new ObservableCollection<string>();
            IRepository<ToolkitInfo> toolkitInfoRepo = new Repository<ToolkitInfo>("ToolkitInfoSaveLocation");
            PopulateListOfToolkits(toolkitInfoRepo.GetData());
        }

        private void PopulateListOfToolkits(ToolkitInfo toolkitInfo)
        {
            foreach(KeyValuePair<string, Toolkit> entry in toolkitInfo.Toolkits)
            {
                ListOfToolkitNames.Add(entry.Key);
            }
        }

        public ICommand SynchroniseCommand => new DelegateCommand(Synchronise);

        public void Synchronise()
        {
            _timeregRepo.SaveData(Timeregistrations);
        }

        public ICommand AddNewTimeregistrationCommand => new DelegateCommand(AddNewTimeregistration);

        public void AddNewTimeregistration()
        {
            var timereg = new Timeregistration();
            timereg.ListOfToolkitNames = ListOfToolkitNames;
            Timeregistrations.Add(timereg);

        }
    }
}
