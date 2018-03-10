using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeSync.DataAccess;
using TimeSync.Model;

namespace TimeSync.UI.ViewModel
{
    class ToolkitInfoViewModel : ObservableObject
    {
        private readonly IRepository<ToolkitInfo> _repo = new Repository<ToolkitInfo>();
        private readonly TimeManager _timeManager = new TimeManager();
        private ToolkitInfo _toolkitInfo;
        private string _toolkitName;
        private string _toolkitUrl;

        public ToolkitInfoViewModel()
        {
            _toolkitInfo = _repo.GetData();
        }



    }
}
