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
        private const string Domain = "NCDMZ";

        /// <summary>
        /// Returns users userId in the specified toolkit.
        /// </summary>
        /// <param name="toolkitInfo"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static int GetUserIdFromToolkit(ToolkitInfo toolkitInfo, UserInfo userInfo)
        {
            ClientContext clientContext = new ClientContext(toolkitInfo.Url);
            var email = $"{userInfo.UserInitials}@netcompany.com";
            UserCollection userCollection = clientContext.Web.SiteUsers;
            clientContext.Load(userCollection);
            clientContext.ExecuteQuery();

            User user = userCollection.GetByEmail(email);
            clientContext.Load(user);
            clientContext.ExecuteQuery();

            return user.Id;
        }

        /// <summary>
        /// Sends timeregistration to the associated toolkit. Returns timeregId.
        /// </summary>
        /// <param name="timereg"></param>
        /// <param name="userInfo"></param>
        public static int MakeTimeRegistration(Timeregistration timereg, UserInfo userInfo)
        {
            ToolkitInfo toolkitInfo = userInfo.ToolkitInfos.Single(toolkit => toolkit.CustomerName == timereg.Customer);
            
            _clientContext = new ClientContext(toolkitInfo.Url);
            _clientContext.Credentials = new NetworkCredential(userInfo.UserInitials, userInfo.UserPassword, Domain);

            var timeregList = "tidsregistrering";
            var sharepointList = _clientContext.Web.Lists.GetByTitle(timeregList);
            _clientContext.Load(sharepointList);
            _clientContext.ExecuteQuery();

            var doneBy = new SPFieldLookupValue(toolkitInfo.UserId, userInfo.UserInitials);
            var author = new SPFieldLookupValue(toolkitInfo.UserId, userInfo.UserInitials);
            var toolkitCase = new SPFieldLookupValue(timereg.CaseId, $"{timereg.Customer}-{timereg.CaseId}");

            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            ListItem sharepointListItem = sharepointList.AddItem(itemCreateInfo);

            sharepointListItem["Hours"] = timereg.Hours;
            sharepointListItem["DoneBy"] = doneBy;
            sharepointListItem["Author"] = author;
            sharepointListItem["Case"] = toolkitCase;
            sharepointListItem["DoneDate"] = timereg.DoneDate;

            sharepointListItem.Update();
            _clientContext.ExecuteQuery();

            return sharepointListItem.Id;
        }

        public static void MakeTimeregistrations(List<Timeregistration> timeregs)
        {
            //Do some loop over list where we create Microsoft.SharePoint.Client.ListItem and put into SP.List oList -- SEE UNIT TEST PROJECT
            //Send to Toolkit -- SEE UNIT TEST PROJECT
            throw new NotImplementedException();
        }
    }
}
