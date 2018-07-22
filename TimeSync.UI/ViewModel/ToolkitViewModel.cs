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
            get => _toolkits;
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

        public ICommand AddCommand => new DelegateCommand(AddNewToolkit);

        public virtual void AddNewToolkit()
        {
            Toolkits.Add(new Toolkit());
        }

        public ICommand SyncCommand => new DelegateCommand(Sync);

        public virtual void Sync()
        {
            TimeManager.SyncToolkits(TimeManager.UserInfo, Toolkits.ToList());
        }

        public ICommand RemoveSelectedCommand => new DelegateCommand(DeleteSelectedToolkits);

        public virtual void DeleteSelectedToolkits()
        {
            Toolkits = new ObservableCollection<Toolkit>(Toolkits.Where(x => !x.ToBeDeleted));
        }

        public ICommand SelectAllCommand => new DelegateCommand(SelectAll);

        public virtual void SelectAll()
        {
            foreach (var toolkit in Toolkits)
            {
                toolkit.ToBeDeleted = true;
            }
            
        }

        public ICommand SelectNoneCommand => new DelegateCommand(SelectNone);

        public virtual void SelectNone()
        {
            foreach (var tk in Toolkits)
            {
                tk.ToBeDeleted = false;
            }
        }

        public ICommand InvertSelectionCommand => new DelegateCommand(InvertSelection);

        public virtual void InvertSelection()
        {
            foreach (var tk in Toolkits)
            {
                tk.ToBeDeleted = !tk.ToBeDeleted;
            }
        }

        public ICommand SaveCommand => new DelegateCommand(Save);

        public virtual void Save()
        {
            TimeManager.StoreToolkits(Toolkits.ToList());
        }
    }
}
