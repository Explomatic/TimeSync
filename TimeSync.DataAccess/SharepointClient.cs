using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;

namespace TimeSync.DataAccess
{
    public static class SharepointClient
    {
        private static ClientContext _clientContext;
        private const string Domain = "NCDMZ";

        /// <summary>
        /// Returns users userId in the specified toolkit.
        /// </summary>
        /// <param name="toolkitUrl"></param>
        /// <param name="userInitials"></param>
        /// <returns></returns>
        public static int GetUserIdFromToolkit(string toolkitUrl, string userInitials)
        {
            ClientContext clientContext = new ClientContext(toolkitUrl);
            var email = $"{userInitials}@netcompany.com";
            UserCollection userCollection = clientContext.Web.SiteUsers;
            clientContext.Load(userCollection);
            clientContext.ExecuteQuery();

            User user = userCollection.GetByEmail(email);
            clientContext.Load(user);
            clientContext.ExecuteQuery();

            return user.Id;
        }

        /// <summary>
        /// Sends timeregistration of amount 'hours' on 'doneDate' to the 'toolkitUrl' for the 'userInitials'. Returns timeregId.
        /// </summary>
        /// <param name="userInitials"></param>
        /// <param name="userPassword"></param>
        /// <param name="toolkitUrl"></param>
        /// <param name="toolkitUserId"></param>
        /// <param name="customer"></param>
        /// <param name="caseId"></param>
        /// <param name="hours"></param>
        /// <param name="doneDate"></param>
        public static int MakeTimeRegistration(string userInitials, string userPassword, string toolkitUrl, int toolkitUserId, string customer, int caseId, double hours, DateTime doneDate)
        {
            _clientContext = new ClientContext(toolkitUrl);
            _clientContext.Credentials = new NetworkCredential(userInitials, userPassword, Domain);

            var timeregList = "tidsregistrering";
            var sharepointList = _clientContext.Web.Lists.GetByTitle(timeregList);
            _clientContext.Load(sharepointList);
            _clientContext.ExecuteQuery();

            var doneBy = new SPFieldLookupValue(toolkitUserId, userInitials);
            var author = new SPFieldLookupValue(toolkitUserId, userInitials);
            var toolkitCase = new SPFieldLookupValue(caseId, $"{customer}-{caseId}");

            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            ListItem sharepointListItem = sharepointList.AddItem(itemCreateInfo);

            sharepointListItem["Hours"] = hours;
            sharepointListItem["DoneBy"] = doneBy;
            sharepointListItem["Author"] = author;
            sharepointListItem["Case"] = toolkitCase;
            sharepointListItem["DoneDate"] = doneDate;

            sharepointListItem.Update();
            _clientContext.ExecuteQuery();

            return sharepointListItem.Id;
        }
    }
}
