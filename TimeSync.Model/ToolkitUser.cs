using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace TimeSync.Model
{
    public class ToolkitUser
    {
        public string Name { get; set; }
        public SecureString Password { get; set; }
        public readonly string Domain;

        public ToolkitUser()
        {
            Domain = "NCDMZ";
        }
    }
}
