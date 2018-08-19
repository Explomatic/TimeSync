using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SharePoint.Client;
using TimeSync.Model;
using System.Text.RegularExpressions;
using System.Web.UI;
using TimeSync.DataAccess;

namespace TimeSync.Tests
{
    [TestClass]
    public class SharepointTests
    {
        private IRepository<ToolkitUser> _toolkitUserRepository;
        private ClientContext _clientContext = null;
        private SharepointClient _sharepointClient;
        private ToolkitUser _toolkitUser;
        private IEncryption _encryptionManager = new Encryption();
        
        private List<Timeregistration> InitializeTimeregList()
        {
            var timeregs = new List<Timeregistration>();
            for (var i = 0; i < 21; i++)
            {
                var now = DateTime.Now;
//                var date = (DateTime.Now).AddDays(i < 7 ? 0 : (i < 14 ? 1 : 2)),
                var date = new DateTime(now.Year, now.Month, now.Day).AddDays(i < 7 ? 0 : (i < 14 ? 1 : 2));
                timeregs.Add(new Timeregistration()
                {
                    Team = "Operations",
                    CaseId = i%3,
                    DoneDate = date,
                    Duration = "08:00-10:00",
                    Hours = 0,
                    IsSynchronized = true,
                    IsWorkPackage = false,
                    Teams = new ObservableCollection<string>()
                    {
                        "Operations","Infrastruktur"
                    },
                    Timeslots = new ObservableCollection<string>()
                    {
                        "08:00-16:30",
                        "16:30-06:00",
                        "06:00-08:00"
                    },
                    Timeslot = "08:00-16:30",
                    ToolkitDisplayName = $"Hest {i % 3}",
                    ToolkitNames = new List<string>()
                    {
                        "Hest 1","Hest 2", "Hest 3"
                    },
                    Toolkits = InitializeToolkitList(),
                    ToBeDeleted = false
                });
            }

            return timeregs;
        }
        private List<Toolkit> InitializeToolkitList()
        {
            var toolkits = new List<Toolkit>();
            for (var i = 0; i < 21; i++)
            {
                toolkits.Add(new Toolkit()
                {
                    CustomerName = "HEST",
                    DisplayName = $"Hest {i % 3}",
                    GetTeamsWithoutSLA = false,
                    Teams = new List<Team>()
                    {
                        new Team()
                        {
                            Name = "Operations",
                            UsesTimeslots = true
                        },
                        new Team()
                        {
                            Name = "Infrastruktur",
                            UsesTimeslots = false
                        }
                    },
                    TimeslotFieldName = "hesthest",
                    TimeslotIsFieldLookup = false,
                    Timeslots = null,
                    ToBeDeleted = false,
                    Url = "HTTPS://HEST.DK"
                });
            }

            return toolkits;
        }

        [TestInitialize]
        public void Init()
        {
            _toolkitUserRepository = new Repository<ToolkitUser>();

            _toolkitUser = _toolkitUserRepository.GetData();

            try
            {
                _toolkitUser.SecurePassword =
                    new NetworkCredential("", _encryptionManager.DecryptText(_toolkitUser.Password)).SecurePassword;
            }
            catch (Exception)
            {
                _toolkitUser.SecurePassword = new NetworkCredential("", "").SecurePassword;
            }
            _toolkitUser.Password = "";

            _sharepointClient = new SharepointClient();
            _clientContext = new ClientContext("https://goto.netcompany.com/cases/GTO22/NCMOD")
            {
                Credentials = new NetworkCredential(_toolkitUser.Name, _toolkitUser.SecurePassword, _toolkitUser.Domain)
            };
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
            var clientContext = new ClientContext("https://goto.netcompany.com/cases/GTO627/FTFA")
            {
                Credentials = new NetworkCredential(_toolkitUser.Name, _toolkitUser.SecurePassword, _toolkitUser.Domain)
            };
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
            var dame = "https://goto.netcompany.com/cases/GTO20/DAMECRM1";
            var clientContext = new ClientContext(dame)
            {
                Credentials = new NetworkCredential(_toolkitUser.Name, _toolkitUser.SecurePassword, _toolkitUser.Domain)
            };
            var allLists = clientContext.Web.Lists;
            clientContext.Load(allLists);
            clientContext.ExecuteQuery();
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
            var clientContext =
                new ClientContext("https://goto.netcompany.com/cases/GTO627/FTFA")
                {
                    Credentials = new NetworkCredential(_toolkitUser.Name, _toolkitUser.SecurePassword, _toolkitUser.Domain)
                };
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
            var clientContext =
                new ClientContext("https://goto.netcompany.com/cases/GTO170/HKA")
                {
                    Credentials = new NetworkCredential(_toolkitUser.Name, _toolkitUser.SecurePassword, _toolkitUser.Domain)
                };
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
            var clientContext =
                new ClientContext("https://goto.netcompany.com/cases/GTO170/HKA")
                {
                    Credentials = new NetworkCredential(_toolkitUser.Name, _toolkitUser.SecurePassword, _toolkitUser.Domain)
                };
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
            var clientContext =
                new ClientContext("https://goto.netcompany.com/cases/GTO170/HKA")
                {
                    Credentials = new NetworkCredential(_toolkitUser.Name, _toolkitUser.SecurePassword, _toolkitUser.Domain)
                };
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
            var clientContext =
                new ClientContext("https://goto.netcompany.com/cases/GTO170/HKA")
                {
                    Credentials = new NetworkCredential(_toolkitUser.Name, _toolkitUser.SecurePassword, _toolkitUser.Domain)
                };
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

            double timespan = 0;
            while (timespan < 24)
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
                                if (!rx.IsMatch(((FieldLookupValue) field.Value).LookupValue)) continue;
                                timeSlotFieldName = field.Key;
                                timeSlots.Add(field.Value.ToString());
                            }
                            else
                            {
                                if (!rx.IsMatch(field.Value.ToString())) continue;
                                timeSlotFieldName = field.Key;
                                if (!timeSlots.Contains(field.Value.ToString()))
                                {
                                    timeSlots.Add(field.Value.ToString());
                                }
                            }   
                        }
                        catch
                        {
                            continue;
                        }
                    }   
                }
                timespan = timeSlots.Sum((Func<string, double>) CalculateTimespan);
                query = UpdateCamlQueryForTesting(timeSlotFieldName, timeSlots, timeregsPerPage);
            }

            Assert.AreEqual(24, timespan);
            Assert.AreEqual(4, timeSlots.Count);
            Assert.AreEqual(timeSlotNavn, timeSlotFieldName);
        }

        [TestMethod]
        public void GetAllTimeSlotsFromFtfaTest()
        {
            var clientContext = new ClientContext("https://goto.netcompany.com/cases/GTO627/FTFA")
            {
                Credentials = new NetworkCredential(_toolkitUser.Name, _toolkitUser.SecurePassword, _toolkitUser.Domain)
            };
            var timeSlotNavn = "TimeSlots";

            var list = "tidsregistrering";
            var oList = _clientContext.Web.Lists.GetByTitle(list);
            clientContext.Load(oList);
            clientContext.ExecuteQuery();

            string timeSlotFieldName = "";
            List<string> timeSlots = new List<string>();

            var timeregsPerPage = 5;

            var query = new CamlQuery
            {
                ViewXml = $@"
                        <View>
                            <Query>
                                <OrderBy>  
                                    <FieldRef Name='ID' Ascending='FALSE'/>   
                                </OrderBy>
                            </Query>
                            <RowLimit>{timeregsPerPage}</RowLimit>
                        </View>"
            };

            double timespan = 0;
            var count = 0;
            while (timespan < 24)
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
                            if (field.Value.GetType() == typeof(FieldLookupValue))
                            {
                                if (!rx.IsMatch(((FieldLookupValue)field.Value).LookupValue)) continue;
                                var fvl = (FieldLookupValue)field.Value;
                                timeSlotFieldName = field.Key;
                                timeSlots.Add(fvl.LookupValue);
                            }
                            else
                            {
                                if (!rx.IsMatch(field.Value.ToString())) continue;
                                timeSlotFieldName = field.Key;
                                if (!timeSlots.Contains(field.Value.ToString()))
                                {
                                    timeSlots.Add(field.Value.ToString());
                                }
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }

                timespan = timeSlots.Sum((Func<string, double>)CalculateTimespan);
                if (timeSlots.Count > 0)
                {
                    query.ListItemCollectionPosition = null;
                    query = UpdateCamlQueryForTesting(timeSlotFieldName, timeSlots, timeregsPerPage);
                }
                else
                {
                    query.ListItemCollectionPosition = listItems.ListItemCollectionPosition;
                }
                count++;

                if (count * timeregsPerPage >= 30)
                {
                    var letMeBreak = 0;
                }

                if (count > 50)
                {
                    break;
                }
            }

            Assert.AreEqual(24, timespan);
            Assert.AreEqual(4, timeSlots.Count);
            Assert.AreEqual(timeSlotNavn, timeSlotFieldName);
        }

         private CamlQuery UpdateCamlQueryForTesting(string fieldName, List<string> timeSlots, int rowLimit)
        { 
            List<CamlQuery> subQueries = timeSlots.Select(timeSlot => new CamlQuery()
                {
                    ViewXml = $@"<Neq><FieldRef Name='{fieldName}'/><Value Type='Text'>{timeSlot}</Value></Neq>"
                })
                .ToList();

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
            CamlQuery query = UpdateCamlQueryForTesting(fieldName, timeSlots, rowLimit);

            timeSlots = new List<string>
            {
                "07:00-17:00",
                "17:00-22:00",
                "22:00-04:00",
                "04:00-06:00"
            };
            rowLimit = 5;
            query = UpdateCamlQueryForTesting(fieldName, timeSlots, rowLimit);

            timeSlots = new List<string>
            {
                "07:00-17:00",
                "17:00-22:00",
                "22:00-04:00"
            };
            rowLimit = 5;
            query = UpdateCamlQueryForTesting(fieldName, timeSlots, rowLimit);

            timeSlots = new List<string>
            {
                "07:00-17:00",
                "17:00-22:00"
            };
            rowLimit = 5;
            query = UpdateCamlQueryForTesting(fieldName, timeSlots, rowLimit);

            timeSlots = new List<string>
            {
                "07:00-17:00"
            };
            rowLimit = 5;
            query = UpdateCamlQueryForTesting(fieldName, timeSlots, rowLimit);

            Assert.AreEqual(1, 2);       
        }

        [TestMethod]
        public void AnyMissingTimeslotsTest()
        {
            List<Timeslot> timeSlots = new List<Timeslot>
            {
                new Timeslot()
                {
                    TimeInterval = new TimeInterval()
                    {
                        Id = -1,
                        Interval = "17:00-22:00"
                    }
                },
                new Timeslot()
                {
                    TimeInterval = new TimeInterval()
                    {
                        Id = -1,
                        Interval = "22:00-04:00"
                    }
                },
                new Timeslot()
                {
                    TimeInterval = new TimeInterval()
                    {
                        Id = -1,
                        Interval = "04:00-06:00"
                    }
                },
                new Timeslot()
                {
                    TimeInterval = new TimeInterval()
                    {
                        Id = -1,
                        Interval = "06:00-07:00"
                    }
                }
            };

            var timespan = timeSlots.Sum((Func<Timeslot, double>) _sharepointClient.CalculateTimespan);

            Assert.AreEqual(24, timespan);
        }

        private static double CalculateTimespan(string timeslot)
        {
            var now = DateTime.Now;
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
                time2 = hour2 > hour1 ? new DateTime(now.Year, now.Month, now.Day, hour2, min2, 0) : new DateTime(now.Year, now.Month, now.Day + 1, hour2, min2, 0);
            }

            return time2.Subtract(time1).TotalHours;
        }

        [TestMethod]
        public void FindAllTeamsTest()
        {
            var tk = new Toolkit()
            {
                Url = "https://goto.netcompany.com/cases/GTO60/Akapedia"
            };

            tk.Teams = _sharepointClient.GetTeamsFromToolkit(_toolkitUser, tk);

            Assert.AreEqual(5, tk.Teams.Count);
        }

        [TestMethod]
        public void FindTimeSlotInfoForTeamsWithSLAFtfaTest()
        {
            var tk = new Toolkit()
            {
                Url = "https://goto.netcompany.com/cases/GTO627/FTFA"
            };

            tk.Teams = _sharepointClient.GetTeamsFromToolkit(_toolkitUser, tk);
            tk = _sharepointClient.GetTimeslotInformationFromToolkit(_toolkitUser, tk);

            Assert.AreEqual(3, tk.Teams.Count);
            Assert.AreEqual(3, tk.Teams.Count(team => team.UsesTimeslots));
            Assert.AreEqual(4, tk.Timeslots.Count);
            Assert.AreEqual(24, tk.Timeslots.Sum((Func<Timeslot, double>)_sharepointClient.CalculateTimespan));
        }

        [TestMethod]
        public void FindTimeSlotInfoForAllTeamsFtfaTest()
        {
            var tk = new Toolkit()
            {
                Url = "https://goto.netcompany.com/cases/GTO627/FTFA",
                GetTeamsWithoutSLA = true
            };

            tk.Teams = _sharepointClient.GetTeamsFromToolkit(_toolkitUser, tk);
            tk = _sharepointClient.GetTimeslotInformationFromToolkit(_toolkitUser, tk);

            Assert.AreEqual(8, tk.Teams.Count);
            Assert.AreEqual(6, tk.Teams.Count(team => team.UsesTimeslots));
            Assert.AreEqual(4, tk.Timeslots.Count);
            Assert.AreEqual(24, tk.Timeslots.Sum((Func<Timeslot, double>)_sharepointClient.CalculateTimespan));
        }

        [TestMethod]
        public void FindTimeSlotInfoForAllTeamsHkTest()
        {
            var tk = new Toolkit()
            {
                Url = "https://goto.netcompany.com/cases/GTO170/HKA"
            };

            tk.Teams = _sharepointClient.GetTeamsFromToolkit(_toolkitUser, tk);
            tk = _sharepointClient.GetTimeslotInformationFromToolkit(_toolkitUser, tk);

            Assert.AreEqual(8, tk.Teams.Count);
            Assert.AreEqual(8, tk.Teams.Count(team => team.UsesTimeslots));
            Assert.IsFalse(tk.TimeslotIsFieldLookup);
            Assert.AreEqual("Periode_x0020_for_x0020_tidsregi", tk.TimeslotFieldName);
        }

        [TestMethod]
        public void UpdateSPCamlQueryTest()
        {
            const string fieldName = "Periode_x0020_for_x0020_tidsregi";
            CamlQuery query;
            var timeSlots = new List<Timeslot>
            {
                new Timeslot {TimeInterval = new TimeInterval {Interval = "07:00-17:00"}}
            };
            var rowLimit = 5;
            query = SharepointClient.UpdateCamlQuery(timeSlots, fieldName, rowLimit);

            timeSlots = new List<Timeslot>
            {
                new Timeslot {TimeInterval = new TimeInterval {Interval = "07:00-17:00"}},
                new Timeslot {TimeInterval = new TimeInterval {Interval = "17:00-22:00"}}
            };
            rowLimit = 5;
            query = SharepointClient.UpdateCamlQuery(timeSlots, fieldName, rowLimit);

            timeSlots = new List<Timeslot>
            {
                new Timeslot {TimeInterval = new TimeInterval {Interval = "07:00-17:00"}},
                new Timeslot {TimeInterval = new TimeInterval {Interval = "17:00-22:00"}},
                new Timeslot {TimeInterval = new TimeInterval {Interval = "22:00-04:00"}}
            };
            rowLimit = 5;
            query = SharepointClient.UpdateCamlQuery(timeSlots, fieldName, rowLimit);

            timeSlots = new List<Timeslot>
            {
                new Timeslot {TimeInterval = new TimeInterval {Interval = "07:00-17:00"}},
                new Timeslot {TimeInterval = new TimeInterval {Interval = "17:00-22:00"}},
                new Timeslot {TimeInterval = new TimeInterval {Interval = "22:00-04:00"}},
                new Timeslot {TimeInterval = new TimeInterval {Interval = "04:00-06:00"}}
            };
            rowLimit = 5;
            query = SharepointClient.UpdateCamlQuery(timeSlots, fieldName, rowLimit);

            timeSlots = new List<Timeslot>
            {
                new Timeslot {TimeInterval = new TimeInterval {Interval = "07:00-17:00"}},
                new Timeslot {TimeInterval = new TimeInterval {Interval = "17:00-22:00"}},
                new Timeslot {TimeInterval = new TimeInterval {Interval = "22:00-04:00"}},
                new Timeslot {TimeInterval = new TimeInterval {Interval = "04:00-06:00"}},
                new Timeslot {TimeInterval = new TimeInterval {Interval = "06:00-07:00"}}
            };
            rowLimit = 5;
            query = SharepointClient.UpdateCamlQuery(timeSlots, fieldName, rowLimit);

            Assert.AreEqual(1, 2);
        }

        [TestMethod]
        public void TimeregWorkPackageTest()
        {
            var timereg = new Timeregistration
            {
                CaseId = 21,
                DoneDate = new DateTime(2018, 5, 7),
                Duration = "11:00-20:00",
                IsWorkPackage = true,
                Team = "Operations",
                ToolkitDisplayName = "ATEAM"
            };

            var toolkit = new Toolkit
            {
                CustomerName = "NCOPEATEAM",
                DisplayName = "ATEAM",
                Url = "https://goto.netcompany.com/cases/GTO539/NCOPEATEAM",
                UserId = 43,
                Teams = new List<Team> {new Team {Name = "Operations", UsesTimeslots = false}}
            };

            var id = _sharepointClient.MakeTimeregistration(timereg, _toolkitUser, toolkit);

            Assert.AreNotEqual(-1, id);
        }
    }
}
