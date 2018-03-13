using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using TimeSync.Model;
using System.Security;
using TimeSync.DataAccess;

namespace TimeSync.Tests
{
    [TestClass]
    public class SharepointTests
    {
        private IRepository<ToolkitUser> _toolkitUserRepository;
        private ClientContext _clientContext = null;
        private string _toolkitUsername;
        private string _toolkitPassword;
        private string _toolkitDomain;

        [TestInitialize]
        public void Init()
        {
            _toolkitUserRepository = new Repository<ToolkitUser>("ToolkitUserSaveLocation");

            var toolkitUser = _toolkitUserRepository.GetData();
            _toolkitUsername = toolkitUser.Name;
            _toolkitPassword = $@"{toolkitUser.Password}";
            _toolkitDomain = toolkitUser.Domain;

            _clientContext = new ClientContext("https://goto.netcompany.com/cases/GTO22/NCMOD");
            _clientContext.Credentials = new NetworkCredential(_toolkitUsername, _toolkitPassword, _toolkitDomain);
        }

        [TestMethod]
        public void GetUserIdByEmailTest()
        {
            var email = "moma@netcompany.com";
            UserCollection userCollection = _clientContext.Web.SiteUsers;
            _clientContext.Load(userCollection);
            _clientContext.ExecuteQuery();

            Microsoft.SharePoint.Client.User user = userCollection.GetByEmail(email);
            _clientContext.Load(user);
            _clientContext.ExecuteQuery();

            Assert.AreEqual(user.Id, 419);
        }

        [TestMethod]
        public void GetUserIdByLoginNameTest()
        {
            var loginName = "i:0#.w|ncdmz\\moma";
            UserCollection userCollection = _clientContext.Web.SiteUsers;
            _clientContext.Load(userCollection);
            _clientContext.ExecuteQuery();

            Microsoft.SharePoint.Client.User user = userCollection.GetByLoginName(loginName);
            _clientContext.Load(user);
            _clientContext.ExecuteQuery();

            Assert.AreEqual(user.Id, 419);
        }

        [TestMethod]
        public void CatchFailedTimeregTest()
        {
            var employeeName = "Morten Madsen";
            var employeeId = 419; // Must be same as credentials
            var customer = "NCMOD";
            int toolkitCaseId = 14641; // Must be postitive
            double hours = 0.5;
            DateTime doneDate = DateTime.Now;

            var list = "tidsregistrering";
            var oList = _clientContext.Web.Lists.GetByTitle(list);
            _clientContext.Load(oList);
            _clientContext.ExecuteQuery();

            var doneBy = new SPFieldLookupValue(employeeId, employeeName);
            var author = new SPFieldLookupValue(employeeId, employeeName);
            var toolkitCase = new SPFieldLookupValue(toolkitCaseId, $"{customer}-{toolkitCaseId}");

            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            ListItem oListItem = oList.AddItem(itemCreateInfo);

            oListItem["Hours"] = hours;
            oListItem["DoneBy"] = doneBy;
            oListItem["Author"] = author;
            oListItem["Case"] = toolkitCase;
            oListItem["DoneDate"] = doneDate;

            oListItem.Update();
            _clientContext.ExecuteQuery();

            var id = Convert.ToInt32(oListItem.Id.ToString());
            Assert.AreEqual(id, -1);
        }

        [TestMethod]
        public void MakeTimeregTest()
        {
            var employeeName = "Morten Madsen";
            var customer = "NCMOD";
            int toolkitCaseId = 14641;
            double hours = 0.5;
            DateTime doneDate = DateTime.Now;

            var list = "tidsregistrering";
            var oList = _clientContext.Web.Lists.GetByTitle(list);
            _clientContext.Load(oList);
            _clientContext.ExecuteQuery();

            var doneBy = new SPFieldLookupValue(419, employeeName);
            var author = new SPFieldLookupValue(419, employeeName);
            var toolkitCase = new SPFieldLookupValue(toolkitCaseId, $"{customer}-{toolkitCaseId}");

            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            ListItem oListItem = oList.AddItem(itemCreateInfo);

            oListItem["Hours"] = hours;
            oListItem["DoneBy"] = doneBy;
            oListItem["Author"] = author;
            oListItem["Case"] = toolkitCase;
            oListItem["DoneDate"] = doneDate;

            oListItem.Update();
            _clientContext.ExecuteQuery();

            var id = Convert.ToInt32(oListItem.Id.ToString());
            Assert.AreNotEqual(id, -1);
        }

        [TestMethod]
        public void ChangeTimeregTest()
        {
            var employeeName = "Morten Madsen";
            var customer = "NCMOD";
            int toolkitCaseId = 14641;
            double hours = 0.5;
            DateTime today = DateTime.Today;
            var yesterday = today.AddDays(-1);
            DateTime doneDate = DateTime.Now;

            var timeregId = 60428;

            var list = "tidsregistrering";
            var oList = _clientContext.Web.Lists.GetByTitle(list);
            _clientContext.Load(oList);
            _clientContext.ExecuteQuery();

            CamlQuery query = new CamlQuery();
            query.ViewXml = $@"
                        <View>
                            <Query>
                                <Where>
                                    <Eq>
                                        <FieldRef Name='ID'/>
                                        <Value Type='Text'>{timeregId}</Value>
                                    </Eq>
                                </Where>
                            </Query>
                        </View>";

            ListItemCollection collListItem = oList.GetItems(query);
            _clientContext.Load(collListItem);
            _clientContext.ExecuteQuery();

            var tmpItems = collListItem[0];

            //tmpItems["ID"] = timeregId;
            tmpItems["DoneDate"] = yesterday;

            tmpItems.Update();
            _clientContext.ExecuteQuery();

            var id = Convert.ToInt32(tmpItems.Id.ToString());
            Assert.AreEqual(id, timeregId);
        }

        [TestMethod]
        public void CheckTimeSlotDefinitionTest()
        {
            var clientContext = new ClientContext("https://goto.netcompany.com/cases/GTO627/FTFA");
            clientContext.Credentials = new NetworkCredential(_toolkitUsername, _toolkitPassword, _toolkitDomain);
            var list = "Time Slots";
            var oList = clientContext.Web.Lists.GetByTitle(list);
            clientContext.Load(oList);
            clientContext.ExecuteQuery();

            Assert.AreEqual(1, 1);
        }

        [TestMethod]
        public void FetchAllListFromToolkitTest()
        {
            var ftfa = "https://goto.netcompany.com/cases/GTO627/FTFA";
            var hk = "https://goto.netcompany.com/cases/GTO170/HKA";
            var clientContext = new ClientContext(hk);
            clientContext.Credentials = new NetworkCredential(_toolkitUsername, _toolkitPassword, _toolkitDomain);
            var allLists = _clientContext.Web.Lists;
            _clientContext.Load(allLists);
            _clientContext.ExecuteQuery();
            var blah = new List<string>();
            foreach (var listitem in allLists)
            {
                blah.Add(listitem.Title);
            }

            Assert.AreEqual(1,2);
        }
    }
}
