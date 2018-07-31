using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace TimeSync.Model
{
    public class ToolkitUser : BaseModel
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public readonly string Domain;
        [IgnoreDataMember]
        public SecureString SecurePassword { get; set; }

        public ToolkitUser()
        {
            Domain = "NCDMZ";
        }
    }
}
