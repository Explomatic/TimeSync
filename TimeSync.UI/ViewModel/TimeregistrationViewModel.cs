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
        private readonly IRepository<ObservableCollection<Timeregistration>> _repo = new Repository<ObservableCollection<Timeregistration>>("TimeregistrationSaveLocation");
        public ObservableCollection<Timeregistration> Timeregistrations = new ObservableCollection<Timeregistration>();
        private readonly TimeManager _timeManager = new TimeManager();
        private string _customerName;
        private int _caseId;
        private string _hours;
        private DateTime _doneDate;
        private bool _synchronised;

        public ObservableCollection<Timeregistration> strings { get; set; }

        

        public TimeregistrationViewModel()
        {
            strings = new ObservableCollection<Timeregistration>();
            strings.Add(new Timeregistration()
            {
                CaseId = -1,
                Customer = "Morten",
            });
            strings.Add(new Timeregistration()
            {
                CaseId = -2,
                Customer = "Også morten",
            });
            //_timeregistration = _repo.GetData();
            //_timeregistration = _timeManager.GetTimeregData();
        }

        public ICommand SynchroniseCommand
        {
            get { return new DelegateCommand(Synchronise);}
        }

        public void Synchronise()
        {
            _repo.SaveData(strings);
        }
        public ICommand AddTest
        {
            get { return new DelegateCommand(Add); }
        }

        public void Add()
        {
            strings.Add(new Timeregistration());

        }
    }
}
