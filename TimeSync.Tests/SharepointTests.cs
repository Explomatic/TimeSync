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
using System.Text.RegularExpressions;
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

        [TestMethod]
        public void CheckTimeSlotTimeregFtfaTest()
        {
            var clientContext = new ClientContext("https://goto.netcompany.com/cases/GTO627/FTFA");
            clientContext.Credentials = new NetworkCredential("moma", @"", "NCDMZ");
            var timeregId = 10984;
            var timeSlotLookUp = "08:30-16:30";

            var list = "tidsregistrering";
            var oList = clientContext.Web.Lists.GetByTitle(list);
            clientContext.Load(oList);
            clientContext.ExecuteQuery();

            CamlQuery query = new CamlQuery();
            query.ViewXml = $@"
                        <View>
                            <Query>
                                <Where>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='ID'/>
                                            <Value Type='Text'>{timeregId}</Value>
                                        </Eq>
                                        <Eq>
                                            <FieldRef Name='TimeSlot'/>
                                            <Value Type='Text'>{timeSlotLookUp}</Value>
                                        </Eq>
                                    </And>
                                </Where>
                            </Query>
                        </View>";

            ListItemCollection collListItem = oList.GetItems(query);
            clientContext.Load(collListItem);
            clientContext.ExecuteQuery();

            var tmpItems = collListItem[0];

            var timeSlot = new SPFieldLookupValue(4, "");
            tmpItems["TimeSlots"] = timeSlot;

            Assert.AreEqual(1, 2);
        }

        [TestMethod]
        public void CheckTimeSlotTimeregHkTest()
        {
            var clientContext = new ClientContext("https://goto.netcompany.com/cases/GTO170/HKA");
            clientContext.Credentials = new NetworkCredential("moma", @"", "NCDMZ");
            var timeregId = 41327;
            var timeSlotLookUp = "07:30-17:00";
            var timeSlotNavn = "Periode_x0020_for_x0020_tidsregi";

            var list = "tidsregistrering";
            var oList = clientContext.Web.Lists.GetByTitle(list);
            clientContext.Load(oList);
            clientContext.ExecuteQuery();

            CamlQuery query = new CamlQuery();
            query.ViewXml = $@"
                        <View>
                            <Query>
                                <Where>
                                    <And>
                                        <Eq>
                                            <FieldRef Name='ID'/>
                                            <Value Type='Text'>{timeregId}</Value>
                                        </Eq>
                                        <Eq>
                                            <FieldRef Name='{timeSlotNavn}'/>
                                            <Value Type='Text'>{timeSlotLookUp}</Value>
                                        </Eq>
                                    </And>
                                </Where>
                            </Query>
                        </View>";

            ListItemCollection collListItem = oList.GetItems(query);
            clientContext.Load(collListItem);
            clientContext.ExecuteQuery();

            var tmpItems = collListItem[0];

            Assert.AreEqual(1, 2);
        }

        [TestMethod]
        public void MakeTimeregWithTimeSlotTest()
        {
            var clientContext = new ClientContext("https://goto.netcompany.com/cases/GTO170/HKA");
            clientContext.Credentials = new NetworkCredential("moma", @"", "NCDMZ");
            var employeeName = "Morten Madsen";
            var customer = "HKA";
            int toolkitCaseId = 23217;
            double hours = 0;
            DateTime doneDate = DateTime.Now;
            var timeSlotLookUp = "07:30-17:00";
            var timeSlotNavn = "Periode_x0020_for_x0020_tidsregi";

            var list = "tidsregistrering";
            var oList = clientContext.Web.Lists.GetByTitle(list);
            clientContext.Load(oList);
            clientContext.ExecuteQuery();

            var doneBy = new SPFieldLookupValue(676, employeeName);
            var author = new SPFieldLookupValue(676, employeeName);
            var toolkitCase = new SPFieldLookupValue(toolkitCaseId, $"{customer}-{toolkitCaseId}");

            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            ListItem oListItem = oList.AddItem(itemCreateInfo);

            oListItem["Hours"] = hours;
            oListItem["DoneBy"] = doneBy;
            oListItem["Author"] = author;
            oListItem["Case"] = toolkitCase;
            oListItem["DoneDate"] = doneDate;
            oListItem[timeSlotNavn] = timeSlotLookUp;

            oListItem.Update();
            clientContext.ExecuteQuery();

            var id = Convert.ToInt32(oListItem.Id.ToString());
            Assert.AreNotEqual(id, -1);
        }

        [TestMethod]
        public void GetTopNTimeregsHkTest()
        {
            var clientContext = new ClientContext("https://goto.netcompany.com/cases/GTO170/HKA");
            clientContext.Credentials = new NetworkCredential("moma", @"", "NCDMZ");
            var timeregId = 41327;
            var timeSlotLookUp = "07:30-17:00";
            var timeSlotNavn = "Periode_x0020_for_x0020_tidsregi";

            var list = "tidsregistrering";
            var oList = clientContext.Web.Lists.GetByTitle(list);
            clientContext.Load(oList);
            clientContext.ExecuteQuery();

            var N = 10;

            CamlQuery query = new CamlQuery();
            query.ViewXml = $@"
                        <View>
                            <Query>
                                <Where>
                                    <Eq>
                                        <FieldRef Name='{timeSlotNavn}'/>
                                        <Value Type='Text'>{timeSlotLookUp}</Value>
                                    </Eq>
                                </Where>
                                <OrderBy>  
                                    <FieldRef Name='ID' Ascending='FALSE'/>   
                                </OrderBy>
                            </Query>
                            <RowLimit>{N}</RowLimit>
                        </View>";

            ListItemCollection collListItem = oList.GetItems(query);
            clientContext.Load(collListItem);
            clientContext.ExecuteQuery();

            Assert.AreEqual(N, collListItem.Count);
        }

        [TestMethod]
        public void GetAllTimeSlotsFromHkTest()
        {
            var clientContext = new ClientContext("https://goto.netcompany.com/cases/GTO170/HKA");
            clientContext.Credentials = new NetworkCredential("moma", @"", "NCDMZ");
            var timeSlotNavn = "Periode_x0020_for_x0020_tidsregi";

            var list = "tidsregistrering";
            var oList = clientContext.Web.Lists.GetByTitle(list);
            clientContext.Load(oList);
            clientContext.ExecuteQuery();

            string timeSlotFieldName = "";
            List<string> timeSlots = new List<string>();

            var timeregsPerPage = 5;

            CamlQuery query = new CamlQuery();
            query.ViewXml = $@"
                        <View>
                            <Query>
                                <OrderBy>  
                                    <FieldRef Name='ID' Ascending='FALSE'/>   
                                </OrderBy>
                            </Query>
                            <RowLimit>{timeregsPerPage}</RowLimit>
                        </View>";

            var cnt = 0;
            do
            {
                ListItemCollection listItems = null;

                listItems = oList.GetItems(query);
                clientContext.Load(listItems);
                clientContext.ExecuteQuery();

                var rx = new Regex(@"\d{2}\:\d{2}\s{0,1}\-\s{0,1}\d{2}\:\d{2}");

                foreach (var item in listItems)
                {
                    foreach (var field in item.FieldValues)
                    {
                        try
                        {
                            if ( field.Value.GetType() == typeof(FieldLookupValue) )
                            {
                                if (rx.IsMatch(((FieldLookupValue)field.Value).LookupValue))
                                {
                                    timeSlotFieldName = field.Key;
                                    timeSlots.Add(field.Value.ToString());
                                }
                            }
                            else
                            {
                                if (rx.IsMatch(field.Value.ToString()))
                                {
                                    timeSlotFieldName = field.Key;
                                    if (!timeSlots.Contains(field.Value.ToString()))
                                    {
                                        timeSlots.Add(field.Value.ToString());
                                    }
                                    
                                }
                            }
                            
                            
                        }
                        catch
                        {
                            continue;
                        }
                        
                    }   
                }
                query = UpdateCamlQuery(timeSlotFieldName, timeSlots, timeregsPerPage);
                cnt++;
            }
            while (cnt < 5);

            Assert.AreEqual(4, timeSlots.Count);
            Assert.AreEqual(timeSlotNavn, timeSlotFieldName);
        }

        private CamlQuery UpdateCamlQuery(string fieldName, List<string> timeSlots, int rowLimit)
        { 
            List<CamlQuery> subQueries = new List<CamlQuery>();
            foreach (var timeSlot in timeSlots)
            {
                var subQuery = new CamlQuery()
                {
                    ViewXml = $@"
                    <Neq>
                        <FieldRef Name='{fieldName}'/>
                        <Value Type='Text'>{timeSlot}</Value>
                    </Neq>"
                };
                subQueries.Add(subQuery);
            }

            subQueries.Add(new CamlQuery(){ViewXml = $@"<IsNotNull><FieldRef Name=""{fieldName}""/></IsNotNull>"});

            CamlQuery query = new CamlQuery()
            {
                ViewXml = $@"</Where><OrderBy><FieldRef Name='ID' Ascending='FALSE'/></OrderBy></Query><RowLimit>{rowLimit}</RowLimit></View>"
            };

            for (int i=0; i < subQueries.Count-1; i++)
            {
                query.ViewXml = @"</And>" + query.ViewXml;
            }

            var cnt = 0;
            foreach (var subQuery in subQueries)
            {
                if (cnt >= 2)
                {
                    query.ViewXml = @"<And>" + query.ViewXml;
                }
                query.ViewXml = subQuery.ViewXml + query.ViewXml;
                cnt++;
            }

            query.ViewXml = @"<View><Query><Where><And>" + query.ViewXml;


            return query;
        }

        [TestMethod]
        public void UpdateCamlQueryTest()
        {
            string fieldName = "Periode_x0020_for_x0020_tidsregi";
            List<string> timeSlots = new List<string>
            {
                "07:00-17:00",
                "17:00-22:00",
                "22:00-04:00",
                "04:00-06:00",
                "06:00-07:00"
            };
            var rowLimit=5;
            CamlQuery query = UpdateCamlQuery(fieldName, timeSlots, rowLimit);

            timeSlots = new List<string>
            {
                "07:00-17:00",
                "17:00-22:00",
                "22:00-04:00",
                "04:00-06:00"
            };
            rowLimit = 5;
            query = UpdateCamlQuery(fieldName, timeSlots, rowLimit);

            timeSlots = new List<string>
            {
                "07:00-17:00",
                "17:00-22:00",
                "22:00-04:00"
            };
            rowLimit = 5;
            query = UpdateCamlQuery(fieldName, timeSlots, rowLimit);

            timeSlots = new List<string>
            {
                "07:00-17:00",
                "17:00-22:00"
            };
            rowLimit = 5;
            query = UpdateCamlQuery(fieldName, timeSlots, rowLimit);

            timeSlots = new List<string>
            {
                "07:00-17:00"
            };
            rowLimit = 5;
            query = UpdateCamlQuery(fieldName, timeSlots, rowLimit);

            Assert.AreEqual(1, 2);       
        }

        [TestMethod]
        public void AnyMissingTimeslotsTest()
        {
            List<string> timeSlots = new List<string>
            {
                "07:00-17:00",
                "17:00-22:00",
                "22:00-04:00",
                "04:00-06:00",
                "06:00-07:00"
            };

            var timespan = 0;
            var now = DateTime.Now;
            foreach (var timeslot in timeSlots)
            {
                DateTime time1 = new DateTime();
                DateTime time2 = new DateTime();
                var rx = new Regex(@"((\d{2})\:(\d{2}))\s{0,1}\-\s{0,1}((\d{2})\:(\d{2}))");
                var matches = rx.Matches(timeslot);
                foreach (Match match in matches)
                {
                    var hour1 = int.Parse(match.Groups[2].Value);
                    var min1 = int.Parse(match.Groups[3].Value);
                    time1 = new DateTime(now.Year, now.Month, now.Day, hour1, min1, 0);

                    var hour2 = int.Parse(match.Groups[5].Value);
                    var min2 = int.Parse(match.Groups[6].Value);
                    time2 = hour2 > hour1 ? new DateTime(now.Year, now.Month, now.Day, hour2, min2, 0) : new DateTime(now.Year, now.Month, now.Day+1, hour2, min2, 0);
                }
                timespan += time2.Subtract(time1).Hours;
            }

            Assert.AreEqual(24, timespan);
        }
    }
}
