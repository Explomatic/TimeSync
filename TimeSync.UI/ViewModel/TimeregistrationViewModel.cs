using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Windows.Input;
using TimeSync.DataAccess;
using TimeSync.Model;

namespace TimeSync.UI.ViewModel
{
    class TimeregistrationViewModel : BaseViewModel
    {
        private readonly IRepository<ObservableCollection<Timeregistration>> _timeregRepo;
        private List<Timeregistration> ListOfTimeregs;
        private string _customerName;
        private int _caseId;
        private string _hours;
        private DateTime _doneDate;
        private bool _synchronised;

        public TimeManager TimeManager { get; set; }

        public ObservableCollection<Timeregistration> Timeregistrations { get; set; }

        public List<Toolkit> ListOfToolkits { get; set; }
        

        public TimeregistrationViewModel()
        {
            _timeregRepo = new Repository<ObservableCollection<Timeregistration>>("TimeregistrationSaveLocation");
            Timeregistrations = new ObservableCollection<Timeregistration>();
            Timeregistrations = _timeregRepo.GetData();

            IRepository<List<Toolkit>> toolkitRepo = new Repository<List<Toolkit>>("ToolkitSaveLocation");
            ListOfToolkits = toolkitRepo.GetData();

            //ListOfTimeregs = new List<Timeregistration>();
        }

        public ICommand SynchroniseCommand => new DelegateCommand(Synchronise);

        public void Synchronise()
        {
            _timeregRepo.SaveData(Timeregistrations);
            PopulateListOfTimeregs();
            TimeManager.Sync(ListOfTimeregs);
        }

        private void PopulateListOfTimeregs()
        {
            ListOfTimeregs = new List<Timeregistration>();
            foreach (var timereg in Timeregistrations)
            {
                ListOfTimeregs.Add(timereg);
            }
        }

        public ICommand AddNewTimeregistrationCommand => new DelegateCommand(AddNewTimeregistration);

        public void AddNewTimeregistration()
        {
            var timereg = new Timeregistration();
            timereg.ListOfToolkits = ListOfToolkits;
            Timeregistrations.Add(timereg);

        }
    }
}
