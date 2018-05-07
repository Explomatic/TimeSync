using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace TimeSync.Model
{
    public class ToolkitUser
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public SecureString SecurePassword { get; set; }
        public readonly string Domain;
        public bool ToSavePassword { get; set; } = false;

        public ToolkitUser()
        {
            Domain = "NCDMZ";
        }
    }
}
