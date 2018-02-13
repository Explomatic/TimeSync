using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeSync.Model;

namespace TimeSync.DataAccess
{
    public class SharepointClient
    {
        public void GetUserId(string toolkitName)
        {
            throw new NotImplementedException();
        }



        public void MakeTimeRegistration(Timeregistration timereg)
        {
            throw new NotImplementedException();
        }

        public void MakeTimeregistrations(List<Timeregistration> timeregs)
        {
            //Do some loop over list where we create Microsoft.SharePoint.Client.ListItem and put into SP.List oList -- SEE UNIT TEST PROJECT
            //Send to Toolkit -- SEE UNIT TEST PROJECT
            throw new NotImplementedException();
        }

    }
}
