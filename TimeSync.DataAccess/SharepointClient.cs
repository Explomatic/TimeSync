using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using TimeSync.Model;

namespace TimeSync.DataAccess
{
    public class SharepointClient
    {
        private Repository _repo = new Repository();
        private UserInfo _userInfo;

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


        }

        public void MakeTimeregistrations(List<Timeregistration> timeregs)
        {
            //Do some loop over list where we create Microsoft.SharePoint.Client.ListItem and put into SP.List oList -- SEE UNIT TEST PROJECT
            //Send to Toolkit -- SEE UNIT TEST PROJECT
            throw new NotImplementedException();
        }

    }
}
