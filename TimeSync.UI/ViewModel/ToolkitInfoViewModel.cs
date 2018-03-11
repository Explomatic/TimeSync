using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeSync.DataAccess;
using TimeSync.Model;

namespace TimeSync.UI.ViewModel
{
    class ToolkitInfoViewModel : ObservableObject
    {
        private readonly IRepository<ToolkitInfo> _repo = new Repository<ToolkitInfo>("ToolkitInfoSaveLocation");
        private readonly TimeManager _timeManager = new TimeManager();
        private ToolkitInfo _toolkitInfo;
        public ObservableCollection<ToolkitInfoViewModelWrapper> ListOfToolkits { get; set; }

        public ToolkitInfoViewModel()
        {
            ListOfToolkits = new ObservableCollection<ToolkitInfoViewModelWrapper>();
            ListOfToolkits.Add(new ToolkitInfoViewModelWrapper()
            {
                ToolkitName = "NCMOD",
                ToolkitUrl = "https://asd"
            });
            ListOfToolkits.Add(new ToolkitInfoViewModelWrapper()
            {
                ToolkitName = "AKA",
                ToolkitUrl = "https://aaaa"
            });
            //_toolkitInfo = _repo.GetData();
        }

        public ICommand AddNewToolkitCommand
        {
            get { return new DelegateCommand(AddNewToolkit); }
        }

        public void AddNewToolkit()
        {
            ListOfToolkits.Add(new ToolkitInfoViewModelWrapper());
        }

        public ICommand SynchroniseCommand
        {
            get { return new DelegateCommand(Synchronise); }
        }

        public void Synchronise()
        {
            PopulateToolkitInfoObject(ListOfToolkits);
            _repo.SaveData(_toolkitInfo);
        }

        private void PopulateToolkitInfoObject(ObservableCollection<ToolkitInfoViewModelWrapper> listOfToolkits)
        {
            foreach (var toolkitInfoWrapper in listOfToolkits)
            {
                Toolkit toolkit = new Toolkit()
                {
                    Url = toolkitInfoWrapper.ToolkitUrl
                };
                if ( !_toolkitInfo.Toolkits.ContainsKey(toolkitInfoWrapper.ToolkitName) )
                {
                    _toolkitInfo.Toolkits.Add(toolkitInfoWrapper.ToolkitName, toolkit);
                }
            }

            
        }
    }

    class ToolkitInfoViewModelWrapper
    {
        public string ToolkitName { get; set; }
        public string ToolkitUrl { get; set; }
    }
}
