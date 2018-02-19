using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSync.Model
{

    public class UserInfo
    {
        public string Initials;
        public string Password;
        public List<ToolkitInfo> ToolkitInfos;

        public UserInfo()
        {
            ToolkitInfos = new List<ToolkitInfo>();
        }
    }
}
