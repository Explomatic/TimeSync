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
        public ObservableCollection<Toolkit> ListOfToolkits { get; set; }
        public TimeManager TimeManager { get; set; }

        public ToolkitViewModel()
        {
            IRepository<ObservableCollection<Toolkit>> repo = new Repository<ObservableCollection<Toolkit>>("ToolkitSaveLocation");
            ListOfToolkits = repo.GetData();
        }

        public ICommand AddNewToolkitCommand => new DelegateCommand(AddNewToolkit);

        public virtual void AddNewToolkit()
        {
            ListOfToolkits.Add(new Toolkit());
        }

        public ICommand SaveToolkitsCommand => new DelegateCommand(SaveToolkits);

        public virtual void SaveToolkits()
        {
            TimeManager.SaveToolkitInfo(TimeManager.UserInfo, ListOfToolkits.ToList());
        }
    }
}
