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
        private readonly IRepository<ToolkitInfo> _repo;
        private readonly TimeManager _timeManager;
        private ToolkitInfo _toolkitInfo;
        public ObservableCollection<ToolkitInfoViewModelWrapper> ListOfToolkits { get; set; }

        public ToolkitInfoViewModel()
        {
            _repo = new Repository<ToolkitInfo>("ToolkitInfoSaveLocation");
            _timeManager = new TimeManager();
            ListOfToolkits = new ObservableCollection<ToolkitInfoViewModelWrapper>();
            _toolkitInfo = _repo.GetData();
            PopulateWrapperClass(_toolkitInfo);
        }

        private void PopulateWrapperClass(ToolkitInfo toolkitInfo)
        {
            foreach (KeyValuePair<string, Toolkit> entry in toolkitInfo.Toolkits)
            {
                ListOfToolkits.Add(new ToolkitInfoViewModelWrapper { ToolkitName = entry.Key, ToolkitUrl = entry.Value.Url });
            }

        }

        public ICommand AddNewToolkitCommand => new DelegateCommand(AddNewToolkit);

        public void AddNewToolkit()
        {
            ListOfToolkits.Add(new ToolkitInfoViewModelWrapper());
        }

        public ICommand SynchroniseCommand => new DelegateCommand(Synchronise);

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
