using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.ApplicationPages.Calendar.Exchange;
using Microsoft.SharePoint.Client;
using TimeSync.Model;

namespace TimeSync.DataAccess
{
    public class SharepointClient : ISharepointClient
    {
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
            var clientContext = new ClientContext(toolkit.Url)
            {
                Credentials = new NetworkCredential(toolkitUser.Name, toolkitUser.Password, toolkitUser.Domain)
            };

            const string timeregList = "tidsregistrering";
            var sharepointList = clientContext.Web.Lists.GetByTitle(timeregList);
            clientContext.Load(sharepointList);
            clientContext.ExecuteQuery();

            var doneBy = new SPFieldLookupValue(toolkit.UserId, toolkitUser.Name);
            var author = new SPFieldLookupValue(toolkit.UserId, toolkitUser.Name);
            var toolkitCase = new SPFieldLookupValue(timereg.CaseId, $"{timereg.Customer}-{timereg.CaseId}");

            var itemCreateInfo = new ListItemCreationInformation();
            var sharepointListItem = sharepointList.AddItem(itemCreateInfo);

            sharepointListItem["Hours"] = timereg.Hours;
            sharepointListItem["DoneBy"] = doneBy;
            sharepointListItem["Author"] = author;
            sharepointListItem["Case"] = toolkitCase;
            sharepointListItem["DoneDate"] = timereg.DoneDate;

            try
            {
                sharepointListItem.Update();
                clientContext.ExecuteQuery();
                return sharepointListItem.Id;
            }
            catch
            {
                return -1;
            }
        }

        public List<ListItem> MakeTimeregistrations(List<Timeregistration> timeregs, ToolkitUser toolkitUser, Toolkit toolkit)
        {
            //Do some loop over list where we create Microsoft.SharePoint.Client.ListItem and put into SP.List oList -- SEE UNIT TEST PROJECT
            //Send to Toolkit -- SEE UNIT TEST PROJECT
            var clientContext = new ClientContext(toolkit.Url)
            {
                Credentials = new NetworkCredential(toolkitUser.Name, toolkitUser.Password, toolkitUser.Domain)
            };

            const string timeregList = "tidsregistrering";
            var sharepointList = clientContext.Web.Lists.GetByTitle(timeregList);
            clientContext.Load(sharepointList);
            clientContext.ExecuteQuery();

            var spListItems = new List<ListItem>();

            var doneBy = new SPFieldLookupValue(toolkit.UserId, toolkitUser.Name);
            var author = new SPFieldLookupValue(toolkit.UserId, toolkitUser.Name);
            var itemCreateInfo = new ListItemCreationInformation();
            foreach (var timereg in timeregs)
            {
                var toolkitCase = new SPFieldLookupValue(timereg.CaseId, $"{timereg.Customer}-{timereg.CaseId}");

                var sharepointListItem = sharepointList.AddItem(itemCreateInfo);

                sharepointListItem["Hours"] = timereg.Hours;
                sharepointListItem["DoneBy"] = doneBy;
                sharepointListItem["Author"] = author;
                sharepointListItem["Case"] = toolkitCase;
                sharepointListItem["DoneDate"] = timereg.DoneDate;

                sharepointListItem.Update();
                spListItems.Add(sharepointListItem);
            }
            
            try
            {
                clientContext.ExecuteQuery();
                return spListItems;
            }
            catch
            {
                return new List<ListItem>();
            }
        }

        public List<Team> GetTeamsFromToolkit(ToolkitUser tkUser, Toolkit tk)
        {
            var clientContext = new ClientContext(tk.Url) { Credentials = new NetworkCredential(tkUser.Name, tkUser.Password, tkUser.Domain) };

            return tk.GetTeamsWithoutSLA ?  GetAllTeams(clientContext) : GetTeamsWithActiveSLA(clientContext);
        }

        public double CalculateTimespan(Timeslot timeslot)
        {
            var now = DateTime.Now;
            DateTime time1 = new DateTime();
            DateTime time2 = new DateTime();
            var rx = new Regex(@"((\d{2})\:(\d{2}))\s{0,1}\-\s{0,1}((\d{2})\:(\d{2}))");
            var matches = rx.Matches(timeslot.TimeInterval.Interval);
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

        public Toolkit GetTimeslotInformationFromToolkit(ToolkitUser tkUser, Toolkit tk)
        {
            var clientContext = new ClientContext(tk.Url) { Credentials = new NetworkCredential(tkUser.Name, tkUser.Password, tkUser.Domain) };

            const string list = "tidsregistrering";
            var oList = clientContext.Web.Lists.GetByTitle(list);
            clientContext.Load(oList);
            clientContext.ExecuteQuery();

            var usesTimeslots = CheckIfToolkitUsesTimeslots(clientContext, oList, tk);

            if (!usesTimeslots) return tk;

            var listOfTimregsWithTimeslot = GetTop250TimeregsWithTimeslot(clientContext, oList, tk);

            var listOfCasesForTimeregsWithTimeslot =
                GetCasesForTimeregsWithTimeslot(clientContext, tk, listOfTimregsWithTimeslot);

            var listOfTeamsUsingTimeslots = listOfCasesForTimeregsWithTimeslot.GroupBy(tkCase => tkCase.Team)
                .Select(tkCase => tkCase.First()).OrderBy(tkCase => tkCase.Team).ToList();

            foreach (var tkCase in listOfTeamsUsingTimeslots)
            {
                foreach (var team in tk.Teams)
                {
                    if (team.Name == tkCase.Team) team.UsesTimeslots = true;
                }
            }

            tk.Timeslots = ExtractTimeslots(listOfTimregsWithTimeslot, tk);

            return tk;
        }

        private static List<Team> GetTeamsWithActiveSLA(ClientContext clientContext)
        {
            var list = "SLA";
            var oList = clientContext.Web.Lists.GetByTitle(list);

            clientContext.Load(oList);
            clientContext.ExecuteQuery();

            var query = new CamlQuery
            {
                ViewXml = @"
                        <View>
                            <Query>
                                <Where>
                                    <Eq>
                                        <FieldRef Name='Status'></FieldRef>
                                        <Value Type ='Text'>10 - Aktiv</Value>
                                    </Eq>
                                </Where>
                            </Query>
                            <ViewFields>
                                <FieldRef Name='Team'></FieldRef>
                            </ViewFields>
                        </View>"
            };

            var listItems = oList.GetItems(query);
            clientContext.Load(listItems);
            clientContext.ExecuteQuery();

            var teams = new List<Team>();

            foreach (var item in listItems)
            {
                foreach (var fv in item.FieldValues)
                {
                    if (fv.Key != "Team") continue;
                    try
                    {
                        teams.AddRange(from FieldLookupValue fvl in (IEnumerable)fv.Value
                                       select new Team()
                                       {
                                           Name = fvl.LookupValue
                                       });
                    }
                    catch (Exception e)
                    {
                        var fvl = (FieldLookupValue)fv.Value;
                        var team = new Team()
                        {
                            Name = fvl.LookupValue
                        };
                        teams.Add(team);
                    }
                }
            }
            return teams.GroupBy(team => team.Name).Select(g => g.First()).OrderBy(team => team.Name).ToList();
        }

        private static List<Team> GetAllTeams(ClientContext clientContext)
        {
            const string list = "Teams";
            var oList = clientContext.Web.Lists.GetByTitle(list);

            clientContext.Load(oList);
            clientContext.ExecuteQuery();

            var query = new CamlQuery
            {
                ViewXml = "<View><ViewFields><FieldRef Name='Title'></FieldRef></ViewFields></View>"
            };

            var listItems = oList.GetItems(query);
            clientContext.Load(listItems);
            clientContext.ExecuteQuery();

            var teams = new List<Team>();
            foreach (var item in listItems)
            {
                var team = new Team()
                {
                    Name = (from field in item.FieldValues where field.Key == "Title" select field.Value.ToString())
                        .Single()
                };
                teams.Add(team);
            }

            //var teams = listItems.Select(item => new Team
            //{
            //    Name =
            //        (from field in item.FieldValues where field.Key == "Title" select field.Value.ToString()).SingleOrDefault()
            //}).ToList();

            return teams.GroupBy(team => team.Name).Select(g => g.First()).OrderBy(team => team.Name).ToList();
        }

        private static List<Timeslot> ExtractTimeslots(IEnumerable<TimeregWithTimeslot> listOfTimregsWithTimeslot, Toolkit tk)
        {
            var timeslots = new List<Timeslot>();

            if (tk.TimeslotIsFieldLookup)
            {
                var uniqueTimeslots = listOfTimregsWithTimeslot.GroupBy(tkTimereg => tkTimereg.TimeslotId)
                    .Select(t => t.First()).OrderBy(t => t.TimeslotId).ToList();
                foreach (var tkTimeslot in uniqueTimeslots)
                {
                    var timeslot = new Timeslot {TimeInterval = ExtractTimeIntervalFromTimeslot(tkTimeslot)};
                    timeslots.Add(timeslot);
                }
            }
            else
            {
                foreach (var tkTimeslot in listOfTimregsWithTimeslot)
                {
                    if (timeslots.Any(ts => ts.TimeInterval.Interval == tkTimeslot.Timeslot)) continue;
                    var timeslot = new Timeslot {TimeInterval = ExtractTimeIntervalFromTimeslot(tkTimeslot)};
                    timeslots.Add(timeslot);
                }
            }

            return timeslots;
        }

        private static TimeInterval ExtractTimeIntervalFromTimeslot(TimeregWithTimeslot tkTimeslot)
        {
            return new TimeInterval()
            {
                Id = tkTimeslot.TimeslotId,
                Interval = tkTimeslot.Timeslot
            };
        }

        private static List<ToolkitCase> GetCasesForTimeregsWithTimeslot(ClientContext clientContext, Toolkit tk, List<TimeregWithTimeslot> listOfTimregsWithTimeslot)
        {
            const string list = "sager";
            var spList = clientContext.Web.Lists.GetByTitle(list);
            clientContext.Load(spList);
            clientContext.ExecuteQuery();

            //.GroupBy(team => team.Name).Select(g => g.First()).OrderBy(team => team.Name).ToList();

            var listOfUniqueTimregsWithTimeslot = listOfTimregsWithTimeslot.GroupBy(timereg => timereg.CaseId)
                .Select(g => g.First()).OrderBy(timereg => timereg.CaseId).ToList();

            var listOfUniqueCaseIds = (from uniqueCaseId in listOfUniqueTimregsWithTimeslot select uniqueCaseId.CaseId).ToList();

            var query = GenereateCamlQueryForIds(listOfUniqueCaseIds);

            var listItems = spList.GetItems(query);
            clientContext.Load(listItems);
            clientContext.ExecuteQuery();


            var tkCases = new List<ToolkitCase>();
            foreach (var item in listItems)
            {
                var tkCase = new ToolkitCase()
                {
                    CaseId = int.Parse((from field in item.FieldValues
                        where field.Key == "ID"
                        select field.Value.ToString()).Single()),
                    Team = (from field in item.FieldValues
                        where field.Key == "Team"
                        select ((FieldLookupValue) field.Value).LookupValue).Single()
                };
                tkCases.Add(tkCase);
            }

            return tkCases;

            //return listItems.Select(item => new ToolkitCase
            //    {
            //        CaseId = int.Parse((from field in item.FieldValues where field.Key == "ID" select field.Value.ToString()).Single()),
            //        Team = (from field in item.FieldValues where field.Key == "Team" select ((FieldLookupValue) field.Value).LookupValue).Single()
            //    })
            //    .ToList();
        }

        private static List<TimeregWithTimeslot> GetTop250TimeregsWithTimeslot(ClientContext clientContext, List spList, Toolkit tk)
        {
            var query = new CamlQuery()
            {
                ViewXml = $@"<View><Query><Where><IsNotNull><FieldRef Name='{tk.TimeslotFieldName}'/></IsNotNull></Where><OrderBy><FieldRef Name='ID' Ascending='FALSE'/></OrderBy><ViewFields><FieldRef Name='Sag_x003a_Sags_x0020_Id' /><FieldRef Name='{tk.TimeslotFieldName}' /></ViewFields></Query><RowLimit>250</RowLimit></View>"
            };

            var listItems = spList.GetItems(query);
            clientContext.Load(listItems);
            clientContext.ExecuteQuery();

            var listOfTimeregsWithTimelots = new List<TimeregWithTimeslot>();
            foreach (var item in listItems)
            {
                var timeregWithTimeslot = new TimeregWithTimeslot
                {
                    Timeslot = tk.TimeslotIsFieldLookup
                        ? (from field in item.FieldValues
                            where field.Key == tk.TimeslotFieldName
                            select ((FieldLookupValue) field.Value).LookupValue).Single()
                        : (from field in item.FieldValues
                            where field.Key == tk.TimeslotFieldName
                            select field.Value.ToString()).Single(),
                    TimeslotId = tk.TimeslotIsFieldLookup
                        ? (from field in item.FieldValues
                            where field.Key == tk.TimeslotFieldName
                            select ((FieldLookupValue) field.Value).LookupId).Single()
                        : -1,
                    CaseId = (from field in item.FieldValues
                        where field.Key == "Sag_x003a_Sags_x0020_Id"
                        select ExtractCaseIdFromField(((FieldLookupValue) field.Value).LookupValue)).Single()
                };

                listOfTimeregsWithTimelots.Add(timeregWithTimeslot);
            }

            return listOfTimeregsWithTimelots;
        }

        private static int ExtractCaseIdFromField(string spCaseId)
        {
            if (spCaseId == "Beregnes") return -1;
            var rx = new Regex(@"([a-zA-Z]+)\-(\d+)");
            var matches = rx.Matches(spCaseId);

            int caseId;
            int.TryParse(matches[0].Groups[2].Value, out caseId);
            return caseId;
        }

        private static string IdAsValueType(int value)
        {
            return $"<Value Type='Text'>{value}</Value>";
        }

        private static CamlQuery GenereateCamlQueryForIds(IEnumerable<int> listOfCaseIds)
        {
            var values = listOfCaseIds.Aggregate("", (current, caseId) => current + IdAsValueType(caseId));
            return new CamlQuery()
            {
                ViewXml = $"<View><Where><Or><In><FieldRef Name='ID'/><Values>{values}</Values></In></Or></Where></View>"
            };
        }

        //private static string IdAsValueType(string customer, int value)
        //{
        //    return $"<Value Type='Text'>{customer}-{value}</Value>";
        //}

        //private static CamlQuery GenereateCamlQueryForIds(IEnumerable<int> listOfCaseIds, string customerName)
        //{
        //    var values =
        //        listOfCaseIds.Aggregate("", (current, caseId) => current + IdAsValueType(customerName, caseId));
        //    return new CamlQuery()
        //    {
        //        ViewXml = $"<View><Where><Or><In><FieldRef Name='Sag_x003a_Sags_x0020_Id'/><Values>{values}</Values></In></Or></Where><View>"
        //    };
        //}

        private static bool CheckIfToolkitUsesTimeslots(ClientContext clientContext, List spList, Toolkit tk)
        {
            var usesDefaultTimeSlotList = CheckForDefaultTimeSlotList(clientContext, spList, tk);

            if (usesDefaultTimeSlotList)
            {
                tk.TimeslotFieldName = "TimeSlot";
                tk.TimeslotIsFieldLookup = true;
                return true;
            }

            var usesTimeSlots = CheckForGeneralTimeSlotField(clientContext, spList, tk);
            return usesTimeSlots;
        }

        private static bool CheckForGeneralTimeSlotField(ClientContext clientContext, List spList, Toolkit tk)
        {
            const int timeregsPerPage = 25;

            var query = new CamlQuery()
            {
                ViewXml = $@"<View><Query><OrderBy><FieldRef Name='ID' Ascending='FALSE'/></OrderBy></Query><RowLimit>{timeregsPerPage}</RowLimit></View>"
            };

            var counter = 0;
            while (counter*timeregsPerPage < 1000)
            {
                var listItems = spList.GetItems(query);
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
                                tk.TimeslotFieldName = field.Key;
                                tk.TimeslotIsFieldLookup = true;
                                return true;
                            }
                            if (!rx.IsMatch(field.Value.ToString())) continue;
                            tk.TimeslotFieldName = field.Key;
                            tk.TimeslotIsFieldLookup = false;
                            return true;
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }

                query.ListItemCollectionPosition = listItems.ListItemCollectionPosition;

                counter++;
            }

            return false;
        }

        private static bool CheckForDefaultTimeSlotList(ClientRuntimeContext clientContext, List spList, Toolkit tk)
        {
            var query = new CamlQuery()
            {
                ViewXml = "<View><Query><Where><IsNotNull><FieldRef Name='TimeSlot'/></IsNotNull></Where></Query><RowLimit>25</RowLimit><ViewFields><FieldRef Name='ID'/></ViewFields></View>"
            };
            var listItems = spList.GetItems(query);
            clientContext.Load(listItems);
            clientContext.ExecuteQuery();

            return listItems.Count != 0;
        }
    }

    
}
