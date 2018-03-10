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
        private readonly IRepository<Timeregistration> _repo = new Repository<Timeregistration>("TimeregistrationSaveLocation");
        public List<Timeregistration> _timeregistration = new List<Timeregistration>();
        private readonly TimeManager _timeManager = new TimeManager();
        private string _customerName;
        private int _caseId;
        private string _hours;
        private DateTime _doneDate;
        private bool _synchronised;

        public ObservableCollection<string> strings { get; set; }

        

        public TimeregistrationViewModel()
        {
            strings = new ObservableCollection<string>() { "A", "B", "C" };
            //_timeregistration = _repo.GetData();
            //_timeregistration = _timeManager.GetTimeregData();
        }

        //public 
        public ICommand AddTest
        {
            get { return new DelegateCommand(Add); }
        }

        public void Add()
        {
            strings.Add("Morten");

        }
    }
}
