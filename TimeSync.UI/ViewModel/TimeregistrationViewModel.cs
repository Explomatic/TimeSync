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

        public ObservableCollection<string> listOfToolkits { get; set; }

        public ObservableCollection<Timeregistration> listOfTimeregs { get; set; }

        

        public TimeregistrationViewModel()
        {
            listOfTimeregs = new ObservableCollection<Timeregistration>();
            listOfTimeregs.Add(new Timeregistration());
            //strings.Add(new Timeregistration()
            //{
            //    CaseId = 10,
            //    Customer = "NCMOD",
            //});
            //strings.Add(new Timeregistration()
            //{
            //    CaseId = 12,
            //    Customer = "Også morten",
            //});
            //_timeregistration = _repo.GetData();
            //_timeregistration = _timeManager.GetTimeregData();
        }

        public ICommand SynchroniseCommand
        {
            get { return new DelegateCommand(Synchronise);}
        }

        public void Synchronise()
        {
            _repo.SaveData(listOfTimeregs);
        }

        public ICommand AddTest
        {
            get { return new DelegateCommand(Add); }
        }

        public void Add()
        {
            listOfTimeregs.Add(new Timeregistration());

        }
    }
}
