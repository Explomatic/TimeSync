using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
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
        private RelayCommand _addCommand;
        private RelayCommand _syncCommand;
        private RelayCommand _saveCommand;
        private RelayCommand _removeSelectedCommand;
        private RelayCommand _selectAllCommand;
        private RelayCommand _selectNoneCommand;
        private RelayCommand _invertSelectionCommand;

        private void InitializeToolkitList()
        {
            for (var i = 0; i < 18; i++)
            {
                _toolkits.Add(new Toolkit()
                {
                    CustomerName = "HEST",
                    DisplayName = "HEST TK",
                    GetTeamsWithoutSLA = false,
                    Teams = new List<Team>()
                    {
                        new Team()
                        {
                            Name = "Operations",
                            UsesTimeslots = true
                        },
                        new Team()
                        {
                            Name = "Infrastruktur",
                            UsesTimeslots = false
                        }
                    },
                    TimeslotFieldName = "hesthest",
                    TimeslotIsFieldLookup = false,
                    Timeslots = null,
                    ToBeDeleted = false,
                    Url = "HTTPS://HEST.DK"
                });
            }
        }

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
            InitializeToolkitList();
        }

        public ICommand AddCommand
        {
            get
            {
                _addCommand = new RelayCommand(param => Add(), param => true);
                return _addCommand;
            }
        }

        protected virtual void Add()
        {
            Toolkits.Add(new Toolkit());
        }

        public ICommand SyncCommand
        {
            get
            {
                _syncCommand = new RelayCommand(param => Sync(), param => CanSyncOrSaveToolkits());
                return _syncCommand;
            }
        }

        private bool CanSyncOrSaveToolkits()
        {
            return Toolkits.Count > 0;
        }

        protected virtual void Sync()
        {
            TimeManager.SyncToolkits(TimeManager.UserInfo, Toolkits.ToList());
        }

        public ICommand RemoveSelectedCommand 
        {
            get
            {
                _removeSelectedCommand = new RelayCommand(param => DeleteSelectedToolkits(), param => CanRemoveToolkits());
                return _removeSelectedCommand;
            }    
        }

        private bool CanRemoveToolkits()
        {
            return Toolkits.Any(tk => tk.ToBeDeleted);
        }

        protected virtual void DeleteSelectedToolkits()
        {
            Toolkits = new ObservableCollection<Toolkit>(Toolkits.Where(x => !x.ToBeDeleted));
        }

        private bool CanSelectToolkits()
        {
            return Toolkits.Count > 0;
        }
        
        public ICommand SelectAllCommand
        {
            get
            {
                _selectAllCommand = new RelayCommand(param => SelectAll(), param => CanSelectToolkits());
                return _selectAllCommand;
            }   
        }

        protected virtual void SelectAll()
        {
            foreach (var toolkit in Toolkits)
            {
                toolkit.ToBeDeleted = true;
            }
            
        }

        public ICommand SelectNoneCommand
        {
            get
            {
                _selectNoneCommand = new RelayCommand(param => SelectNone(), param => CanSelectToolkits());
                return _selectNoneCommand;
            }   
        }

        protected virtual void SelectNone()
        {
            foreach (var tk in Toolkits)
            {
                tk.ToBeDeleted = false;
            }
        }

        public ICommand InvertSelectionCommand
        {
            get
            {
                _invertSelectionCommand = new RelayCommand(param => InvertSelection(), param => CanSelectToolkits());
                return _invertSelectionCommand;
            }   
        }

        protected virtual void InvertSelection()
        {
            foreach (var tk in Toolkits)
            {
                tk.ToBeDeleted = !tk.ToBeDeleted;
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                _saveCommand = new RelayCommand(param => Save(), param => CanSyncOrSaveToolkits());
                return _saveCommand;
            }
        }

        protected virtual void Save()
        {
            TimeManager.SaveToolkits(Toolkits.ToList());
        }
    }
}
