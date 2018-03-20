using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using TimeSync.Model;

namespace TimeSync.DataAccess
{
    public class SharepointClient : ISharepointClient
    {
        private ClientContext _clientContext;

        public int GetUserIdFromToolkit(ToolkitUser toolkitUser, Toolkit toolkit)
        {
            ClientContext clientContext = new ClientContext(toolkit.Url);
            UserCollection userCollection = clientContext.Web.SiteUsers;
            clientContext.Load(userCollection);
            clientContext.ExecuteQuery();


            var email = $"{toolkitUser.Name}@netcompany.com";
            User user = userCollection.GetByEmail(email);
            clientContext.Load(user);
            clientContext.ExecuteQuery();

            return user.Id;
        }

        public int MakeTimeregistration(Timeregistration timereg, ToolkitUser toolkitUser, Toolkit toolkit)
        {
            _clientContext = new ClientContext(toolkit.Url);
            _clientContext.Credentials = new NetworkCredential(toolkitUser.Name, toolkitUser.Password, toolkitUser.Domain);

            var timeregList = "tidsregistrering";
            var sharepointList = _clientContext.Web.Lists.GetByTitle(timeregList);
            _clientContext.Load(sharepointList);
            _clientContext.ExecuteQuery();

            var doneBy = new SPFieldLookupValue(toolkit.UserId, toolkitUser.Name);
            var author = new SPFieldLookupValue(toolkit.UserId, toolkitUser.Name);
            var toolkitCase = new SPFieldLookupValue(timereg.CaseId, $"{timereg.Customer}-{timereg.CaseId}");

            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            ListItem sharepointListItem = sharepointList.AddItem(itemCreateInfo);

            sharepointListItem["Hours"] = timereg.Hours;
            sharepointListItem["DoneBy"] = doneBy;
            sharepointListItem["Author"] = author;
            sharepointListItem["Case"] = toolkitCase;
            sharepointListItem["DoneDate"] = timereg.DoneDate;

            try
            {
                sharepointListItem.Update();
                _clientContext.ExecuteQuery();
                return sharepointListItem.Id;
            }
            catch
            {
                return -1;
            }
            

            
        }

        public void MakeTimeregistrations(List<Timeregistration> timeregs, ToolkitUser toolkitUser, Toolkit toolkit)
        {
            //Do some loop over list where we create Microsoft.SharePoint.Client.ListItem and put into SP.List oList -- SEE UNIT TEST PROJECT
            //Send to Toolkit -- SEE UNIT TEST PROJECT
            throw new NotImplementedException();
        }
    }
}
