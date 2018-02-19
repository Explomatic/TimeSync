using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using TimeSync.Model;

namespace TimeSync.DataAccess
{
    public class SharepointClient
    {
        private Repository _repo = new Repository();
        private UserInfo _userInfo;
        private ClientContext _clientContext;
        private string _toolkitDomain = "NCDMZ";

        public SharepointClient()
        {
            _userInfo = _repo.GetUserInfo();
        }

        public UserInfo GetUserId(UserInfo userInfo, ToolkitInfo toolkitInfo)
        {
            ClientContext clientContext = new ClientContext(toolkitInfo.Url);
            var email = $"{userInfo.Initials}@netcompany.com";
            UserCollection userCollection = clientContext.Web.SiteUsers;
            clientContext.Load(userCollection);
            clientContext.ExecuteQuery();

            User user = userCollection.GetByEmail(email);
            clientContext.Load(user);
            clientContext.ExecuteQuery();

            toolkitInfo.UserId = user.Id;
            userInfo.ToolkitInfos.Add(toolkitInfo);

            return userInfo;
        }



        public void MakeTimeRegistration(Timeregistration timereg, UserInfo userInfo)
        {
            ToolkitInfo toolkitInfo =
                userInfo.ToolkitInfos.Single(toolkit => toolkit.CustomerName == timereg.Customer);

            _clientContext = new ClientContext(toolkitInfo.Url);
            _clientContext.Credentials = new NetworkCredential(userInfo.Initials, userInfo.Password, _toolkitDomain);

            var timeregList = "tidsregistrering";
            var sharepointList = _clientContext.Web.Lists.GetByTitle(timeregList);
            _clientContext.Load(sharepointList);
            _clientContext.ExecuteQuery();

            var doneBy = new SPFieldLookupValue(419, userInfo.Initials);
            var author = new SPFieldLookupValue(419, userInfo.Initials);
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
        }

        public void MakeTimeregistrations(List<Timeregistration> timeregs)
        {
            //Do some loop over list where we create Microsoft.SharePoint.Client.ListItem and put into SP.List oList -- SEE UNIT TEST PROJECT
            //Send to Toolkit -- SEE UNIT TEST PROJECT
            throw new NotImplementedException();
        }

    }
}
