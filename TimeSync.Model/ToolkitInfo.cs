using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSync.Model
{
    public class ToolkitInfo
    {
        public Dictionary<string, Toolkit> Toolkits;
        public ToolkitInfo()
        {
            Toolkits = new Dictionary<string, Toolkit>();
        }

        public void AddToolkit(string url, string customerName)
        {
            Toolkit toolkit = new Toolkit();
            toolkit.Url = url;
            Toolkits.Add(customerName, toolkit);
        }

    }

    public class Toolkit 
    {
        public int UserId { get; set; }
        public string Url { get; set; }
    }
}
