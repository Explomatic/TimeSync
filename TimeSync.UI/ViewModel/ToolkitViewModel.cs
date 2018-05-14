using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Castle.Core;
using TimeSync.DataAccess;
using TimeSync.IoC;
using TimeSync.Model;

namespace TimeSync.UI.ViewModel
{
    [Interceptor(typeof(ExceptionInterceptor))]
    public class ToolkitViewModel : BaseViewModel
    {
        private ObservableCollection<Toolkit> _toolkits;

        public ObservableCollection<Toolkit> Toolkits
        {
            get { return _toolkits; }
            set
            {
                _toolkits = value;
                RaisePropertyChangedEvent("Toolkits");
            }
        }

        public TimeManager TimeManager { get; set; }

        public ToolkitViewModel()
        {
            IRepository<ObservableCollection<Toolkit>> repo = new Repository<ObservableCollection<Toolkit>>("ToolkitSaveLocation");
            Toolkits = repo.GetData();
        }

        public ICommand AddNewToolkitCommand => new DelegateCommand(AddNewToolkit);

        public virtual void AddNewToolkit()
        {
            Toolkits.Add(new Toolkit());
        }

        public ICommand SaveToolkitsCommand => new DelegateCommand(SaveToolkits);

        public virtual void SaveToolkits()
        {
            TimeManager.SaveToolkitInfo(TimeManager.UserInfo, Toolkits.ToList());
        }

        public ICommand DeleteSelectedToolkitsCommand => new DelegateCommand(DeleteSelectedToolkits);

        public virtual void DeleteSelectedToolkits()
        {
            Toolkits = new ObservableCollection<Toolkit>(Toolkits.Where(x => !x.ToBeDeleted));
        }
    }
}
