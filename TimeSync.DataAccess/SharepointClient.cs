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
    public static class SharepointClient
    {
        private static ClientContext _clientContext;

        public static int GetUserIdFromToolkit(ToolkitInfo toolkitInfo, ToolkitUser toolkitUser)
        {
            //ClientContext clientContext = new ClientContext(toolkitInfo.Toolkits[toolkitInfo].Url);
            //var email = $"{toolkitUser.Name}@netcompany.com";
            //UserCollection userCollection = clientContext.Web.SiteUsers;
            //clientContext.Load(userCollection);
            //clientContext.ExecuteQuery();

            //User user = userCollection.GetByEmail(email);
            //clientContext.Load(user);
            //clientContext.ExecuteQuery();

            //return user.Id;
            throw new NotImplementedException();
        }

        public static int MakeTimeRegistration(Timeregistration timereg, ToolkitUser toolkitUser)
        {
            ////ToolkitInfo toolkitInfo = toolkitUser.ToolkitInfos.Single(toolkit => toolkit.CustomerName == timereg.Customer);

            //_clientContext = new ClientContext(toolkitInfo.Url);
            //_clientContext.Credentials = new NetworkCredential(userInfo.Username, userInfo.Password, Domain);

            //var timeregList = "tidsregistrering";
            //var sharepointList = _clientContext.Web.Lists.GetByTitle(timeregList);
            //_clientContext.Load(sharepointList);
            //_clientContext.ExecuteQuery();

            //var doneBy = new SPFieldLookupValue(toolkitInfo.UserId, userInfo.Username);
            //var author = new SPFieldLookupValue(toolkitInfo.UserId, userInfo.Username);
            //var toolkitCase = new SPFieldLookupValue(timereg.CaseId, $"{timereg.Customer}-{timereg.CaseId}");

            //ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            //ListItem sharepointListItem = sharepointList.AddItem(itemCreateInfo);

            //sharepointListItem["Hours"] = timereg.Hours;
            //sharepointListItem["DoneBy"] = doneBy;
            //sharepointListItem["Author"] = author;
            //sharepointListItem["Case"] = toolkitCase;
            //sharepointListItem["DoneDate"] = timereg.DoneDate;

            //sharepointListItem.Update();
            //_clientContext.ExecuteQuery();

            //return sharepointListItem.Id;
            throw new NotImplementedException();
        }

        public static void MakeTimeregistrations(List<Timeregistration> timeregs)
        {
            //Do some loop over list where we create Microsoft.SharePoint.Client.ListItem and put into SP.List oList -- SEE UNIT TEST PROJECT
            //Send to Toolkit -- SEE UNIT TEST PROJECT
            throw new NotImplementedException();
        }
    }
}
