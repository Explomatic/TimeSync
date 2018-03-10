using System;
using System.Collections.Generic;
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
        private readonly IRepository<Timeregistration> _repo = new Repository<Timeregistration>();
        private readonly Timeregistration _timeregistration = new Timeregistration();
        private string _customerName;
        private int _caseId;
        private string _hours;
        private DateTime _doneDate;
        private bool _synchronised;

        public TimeregistrationViewModel()
        {
            _timeregistration = _repo.GetData();
        }

        //public 

    }
}
