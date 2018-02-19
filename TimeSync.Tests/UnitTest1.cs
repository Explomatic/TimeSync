using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using TimeSync.Model;

namespace TimeSync.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private ClientContext _clientContext = null;
        private string _ToolkitUsername;
        private string _ToolkitPassword;
        private string _ToolkitDomain;

        [TestInitialize]
        public void Init()
        {
            _ToolkitUsername = "moma";
            _ToolkitPassword = "cTWq0yoW#dUyjfZUrw6m";
            _ToolkitDomain = "ncdmz";

            _clientContext = new ClientContext("https://goto.netcompany.com/cases/GTO22/NCMOD");
            _clientContext.Credentials = new NetworkCredential(_ToolkitUsername, _ToolkitPassword, _ToolkitDomain);
        }

        [TestMethod]
        public void GetUserIdByEmailTest()
        {
            var email = "moma@netcompany.com";
            UserCollection userCollection = _clientContext.Web.SiteUsers;
            _clientContext.Load(userCollection);
            _clientContext.ExecuteQuery();

            User user = userCollection.GetByEmail(email);
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

            User user = userCollection.GetByLoginName(loginName);
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
        public void GetUserInfoFromRepoTest()
        {
            UserInfo userInfo;
            string saveLocation = "SavedData.txt";
            try
            {
                using (FileStream fileStream = new FileStream(saveLocation, FileMode.Open))
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    string storedJsonString = streamReader.ReadToEnd();
                    userInfo = JsonConvert.DeserializeObject<UserInfo>(storedJsonString);
                }
            }
            catch (FileNotFoundException)
            {
                userInfo = new UserInfo();
            }

            Assert.AreEqual(userInfo.Initials, "MOMA");
            Assert.AreEqual(userInfo.Password, "blablabla");
            Assert.AreEqual(userInfo.ToolkitInfos.Count, 2);

            ToolkitInfo toolkitInfo1 = userInfo.ToolkitInfos[0];
            ToolkitInfo toolkitInfo2 = userInfo.ToolkitInfos[1];

            Assert.AreEqual(toolkitInfo1.Url, "https://goto.netcompany.com/cases/GTO22/NCMOD");
            Assert.AreEqual(toolkitInfo1.CustomerName, "NCMOD");
            Assert.AreEqual(toolkitInfo1.UserId, 419);

            Assert.AreEqual(toolkitInfo2.Url, "https://goto.netcompany.com/cases/GTO539/NCOPEATEAM");
            Assert.AreEqual(toolkitInfo2.CustomerName, "ATEAM");
            Assert.AreEqual(toolkitInfo2.UserId, 43);
        }

        [TestMethod]
        public void SaveUserInfoToRepoTest()
        {
            string _saveLocation = "SavedData.txt";
            UserInfo userInfo = new UserInfo();
            ToolkitInfo toolkitInfo1 = new ToolkitInfo();
            ToolkitInfo toolkitInfo2 = new ToolkitInfo();

            toolkitInfo1.Url = "https://goto.netcompany.com/cases/GTO22/NCMOD";
            toolkitInfo1.CustomerName = "NCMOD";
            toolkitInfo1.UserId = 419;

            toolkitInfo2.Url = "https://goto.netcompany.com/cases/GTO539/NCOPEATEAM";
            toolkitInfo2.CustomerName = "ATEAM";
            toolkitInfo2.UserId = 43;

            userInfo.Initials = "MOMA";
            userInfo.Password = "blablabla";

            userInfo.ToolkitInfos.Add(toolkitInfo1);
            userInfo.ToolkitInfos.Add(toolkitInfo2);
            
            bool couldSerializeAndSaveData = false;

            FileMode mode = FileMode.Create;
            try
            {
                string jsonString = JsonConvert.SerializeObject(userInfo);

                using (FileStream fileStream = new FileStream(_saveLocation, mode))
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.Write(jsonString);
                }

                couldSerializeAndSaveData = true;

            }
            catch(Exception e)
            {
                
            }

            Assert.IsTrue(couldSerializeAndSaveData);
        }
    }
}
