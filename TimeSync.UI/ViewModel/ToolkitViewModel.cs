using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly IRepository<List<Toolkit>> _repo;
        private List<Toolkit> _toolkits;

        public ObservableCollection<Toolkit> ListOfToolkits { get; set; }

        public TimeManager TimeManager { get; set; }

        public ToolkitViewModel()
        {
            _repo = new Repository<List<Toolkit>>("ToolkitSaveLocation");
            ListOfToolkits = new ObservableCollection<Toolkit>();
            _toolkits = _repo.GetData();
            //PopulateWrapperClass(_toolkitInfo);
        }

        //private void PopulateWrapperClass(ToolkitInfo toolkitInfo)
        //{
        //    foreach (KeyValuePair<string, Toolkit> entry in toolkitInfo.Toolkits)
        //    {
        //        ListOfToolkits.Add(new ToolkitInfoViewModelWrapper { ToolkitName = entry.Key, ToolkitUrl = entry.Value.Url });
        //    }
        //}

        public ICommand AddNewToolkitCommand => new DelegateCommand(AddNewToolkit);

        public virtual void AddNewToolkit()
        {
            ListOfToolkits.Add(new Toolkit());
        }

        public ICommand SaveToolkitsCommand => new DelegateCommand(SaveToolkits);

        public virtual void SaveToolkits()
        {
            //PopulateToolkitInfoObject(ListOfToolkits);
            TimeManager.SaveToolkitInfo(TimeManager.UserInfo, _toolkits);
        }

    //    private void PopulateToolkitInfoObject(ObservableCollection<ToolkitInfoViewModelWrapper> listOfToolkits)
    //    {
    //        foreach (var toolkitInfoWrapper in listOfToolkits)
    //        {
    //            Toolkit toolkit = new Toolkit()
    //            {
    //                Url = toolkitInfoWrapper.ToolkitUrl
    //            };
    //            if ( !_toolkitInfo.Toolkits.ContainsKey(toolkitInfoWrapper.ToolkitName) )
    //            {
    //                _toolkitInfo.Toolkits.Add(toolkitInfoWrapper.ToolkitName, toolkit);
    //            }
    //        }     
    //    }
    }

    class ToolkitInfoViewModelWrapper
    {
        public string ToolkitName { get; set; }
        public string ToolkitUrl { get; set; }
    }
}
