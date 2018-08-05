using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Castle.Core;
using TimeSync.DataAccess;
using TimeSync.IoC;
using TimeSync.Model;

namespace TimeSync.UI.ViewModel
{
    [Interceptor(typeof(ExceptionInterceptor))]
    public class TimeregistrationViewModel : BaseViewModel
    {
        private ObservableCollection<Timeregistration> _timeregistrations;
        private IRepository<List<Toolkit>> _toolkitRepo;
        private RelayCommand _addCommand;
        private RelayCommand _syncCommand;
        private RelayCommand _saveCommand;
        private RelayCommand _removeSelectedCommand;
        private RelayCommand _selectAllCommand;
        private RelayCommand _selectNoneCommand;
        private RelayCommand _invertSelectionCommand;

        private void InitializeTimeregList()
        {
            for (var i = 0; i < 18; i++)
            {
                _timeregistrations.Add(new Timeregistration()
                {
                    Team = "Operations",
                    CaseId = 0,
                    DoneDate = DateTime.Now,
                    Duration = "08:00-10:00",
                    Hours = 0,
                    IsSynchronized = true,
                    IsWorkPackage = false,
                    Teams = new ObservableCollection<string>()
                    {
                        "Operations","Infrastruktur"
                    },
                    Timeslots = new ObservableCollection<string>()
                    {
                        "08:00-16:30",
                        "16:30-06:00",
                        "06:00-08:00"
                    },
                    Timeslot = "08:00-16:30",
                    ToolkitDisplayName = "Hest 1",
                    ToolkitNames = new List<string>()
                    {
                        "Hest 1","Hest 2", "Hest 3"
                    },
                    Toolkits = InitializeToolkitList(),
                    ToBeDeleted = false
                });
            }
        }
        private List<Toolkit> InitializeToolkitList()
        {
            var toolkits = new List<Toolkit>();
            for (var i = 0; i < 18; i++)
            {
                toolkits.Add(new Toolkit()
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

            return toolkits;
        }

        public TimeManager TimeManager { get; set; }
        public ObservableCollection<Timeregistration> Timeregistrations {
            get => _timeregistrations;
            set
            {
                _timeregistrations = value;
                RaisePropertyChangedEvent("Timeregistrations");
            } 
        }
        
        public List<Toolkit> ListOfToolkits { get; set; }
        public List<string> ListOfToolkitNames { get; set; }

        public TimeregistrationViewModel()
        {
            IRepository<ObservableCollection<Timeregistration>> timeregRepo = new Repository<ObservableCollection<Timeregistration>>("TimeregistrationSaveLocation");
            Timeregistrations = new ObservableCollection<Timeregistration>();
            Timeregistrations = timeregRepo.GetData();

            _toolkitRepo = new Repository<List<Toolkit>>("ToolkitSaveLocation");
            ListOfToolkits = _toolkitRepo.GetData();
            ListOfToolkitNames = (from tk in ListOfToolkits select tk.DisplayName).ToList();

            // Used for easy UI population during design phases.
            //InitializeTimeregList();

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
            UpdateLists();
            Timeregistrations.Add(new Timeregistration
            {
                ToolkitNames = ListOfToolkitNames,
                Toolkits = ListOfToolkits,
                Teams = new ObservableCollection<string>(),
                Timeslots = new ObservableCollection<string>()
            });
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
            return Timeregistrations.Count > 0;
        }

        protected virtual void Sync()
        {
            TimeManager.SyncTimeregs(Timeregistrations.ToList());
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
            return Timeregistrations.Any(tk => tk.ToBeDeleted);
        }

        protected virtual void DeleteSelectedToolkits()
        {
            Timeregistrations = new ObservableCollection<Timeregistration>(Timeregistrations.Where(x => !x.ToBeDeleted));
        }

        private bool CanSelectToolkits()
        {
            return Timeregistrations.Count > 0;
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
            foreach (var timereg in Timeregistrations)
            {
                timereg.ToBeDeleted = true;
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
            foreach (var timereg in Timeregistrations)
            {
                timereg.ToBeDeleted = false;
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
            foreach (var timereg in Timeregistrations)
            {
                timereg.ToBeDeleted = !timereg.ToBeDeleted;
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
            TimeManager.SaveTimeregistrations(Timeregistrations.ToList());
        }

        private void UpdateLists()
        {
            ListOfToolkits = _toolkitRepo.GetData();
            ListOfToolkitNames = (from tk in ListOfToolkits select tk.DisplayName).ToList();
        }
    }
}
