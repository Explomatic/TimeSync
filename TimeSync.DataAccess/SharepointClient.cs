using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.SharePoint;
using Microsoft.SharePoint.ApplicationPages.Calendar.Exchange;
using Microsoft.SharePoint.Client;
using TimeSync.Model;

namespace TimeSync.DataAccess
{
    public class SharepointClient : ISharepointClient
    {
        public virtual int GetUserIdFromToolkit(ToolkitUser toolkitUser, Toolkit toolkit)
        {
            var clientContext = new ClientContext(toolkit.Url);
            var userCollection = clientContext.Web.SiteUsers;
            clientContext.Load(userCollection);
            clientContext.ExecuteQuery();

            var email = $"{toolkitUser.Name}@netcompany.com";
            var user = userCollection.GetByEmail(email);
            clientContext.Load(user);
            clientContext.ExecuteQuery();

            return user.Id;
        }

        public virtual int MakeTimeregistration(Timeregistration timereg, ToolkitUser toolkitUser, Toolkit toolkit)
        {
            var clientContext = new ClientContext(toolkit.Url)
            {
                Credentials = new NetworkCredential(toolkitUser.Name, toolkitUser.SecurePassword, toolkitUser.Domain)
            };

            const string timeregList = "tidsregistrering";
            var sharepointList = clientContext.Web.Lists.GetByTitle(timeregList);
            clientContext.Load(sharepointList);
            clientContext.ExecuteQuery();

            var itemCreateInfo = new ListItemCreationInformation();
            var sharepointListItem = sharepointList.AddItem(itemCreateInfo);

            var success = timereg.CouldConvertDurationToHours();
            if (!success) return -1;

            var doneBy = new SPFieldLookupValue(toolkit.UserId, toolkitUser.Name);
            var author = new SPFieldLookupValue(toolkit.UserId, toolkitUser.Name);
            if (timereg.IsWorkPackage)
            {
                sharepointListItem["Case"] = FindRelatedCaseForWorkPackage(clientContext, timereg.CaseId);
                var toolkitWorkPackage = new SPFieldLookupValue(timereg.CaseId, "");
                sharepointListItem["WorkPackage"] = toolkitWorkPackage;
            }
            else
            {
                var toolkitCase = new SPFieldLookupValue(timereg.CaseId, $"{toolkit.CustomerName}-{timereg.CaseId}");
                sharepointListItem["Case"] = toolkitCase;
            }

            //sharepointListItem["Hours"] = timereg.Hours;
            sharepointListItem["Hours"] = timereg.Hours;
            sharepointListItem["DoneBy"] = doneBy;
            sharepointListItem["Author"] = author;
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

        public virtual FieldLookupValue FindRelatedCaseForWorkPackage(ClientContext clientContext, int timeregCaseId)
        {
            const string list = "Arbejdspakker";
            var sharepointList = clientContext.Web.Lists.GetByTitle(list);
            clientContext.Load(sharepointList);
            clientContext.ExecuteQuery();

            var query = new CamlQuery
            {
                ViewXml =
                    $@"<View><Query><Where><Eq><FieldRef Name='ID' /><Value Type='Text'>{
                            timeregCaseId
                        }</Value></Eq></Where></Query><ViewFields><FieldRef Name='RelatedCase' /></ViewFields></View>"
            };

            //

            var listItems = sharepointList.GetItems(query);
            clientContext.Load(listItems);
            clientContext.ExecuteQuery();

            var item = listItems[0];
            return (FieldLookupValue) item.FieldValues.Single(field => field.Key == "RelatedCase").Value;
        }

        //TODO: Add support for multiple timeregs per day for a case (i.e. group them by case id)
        //Do some loop over list where we create Microsoft.SharePoint.Client.ListItem and put into SP.List oList -- SEE UNIT TEST PROJECT
        //Send to Toolkit -- SEE UNIT TEST PROJECT
        public virtual void MakeTimeregistrations(IEnumerable<Timeregistration> timeregs, ToolkitUser toolkitUser, Toolkit toolkit)
        {
            var clientContext = new ClientContext(toolkit.Url)
            {
                Credentials = new NetworkCredential(toolkitUser.Name, toolkitUser.SecurePassword, toolkitUser.Domain)
            };

            const string timeregSpListName = "tidsregistrering";
            var timeregSpList = clientContext.Web.Lists.GetByTitle(timeregSpListName);
            clientContext.Load(timeregSpList);
            clientContext.ExecuteQuery();

            var listItems = new Dictionary<CaseDateTimeslot, ListItem>();

            var doneBy = new SPFieldLookupValue(toolkit.UserId, toolkitUser.Name);
            var author = new SPFieldLookupValue(toolkit.UserId, toolkitUser.Name);
            var itemCreateInfo = new ListItemCreationInformation();

            var timeregsByCaseIdDateTimeslot = GroupTimeregistrations(timeregs);
            foreach (var caseDateTimeslotGroup in timeregsByCaseIdDateTimeslot)
            {
                var sharepointListItem = timeregSpList.AddItem(itemCreateInfo);
                
                var caseDateTimeslot = caseDateTimeslotGroup.Key;
                var toolkitCase = new SPFieldLookupValue(caseDateTimeslot.CaseId, $"{toolkit.CustomerName}-{caseDateTimeslot.CaseId}");
                var doneDate = caseDateTimeslot.Date;
                var hours = CalculateDuration(caseDateTimeslotGroup.Value);
                
                sharepointListItem["Hours"] = hours;
                sharepointListItem["DoneBy"] = doneBy;
                sharepointListItem["Author"] = author;
                sharepointListItem["Case"] = toolkitCase;
                sharepointListItem["DoneDate"] = doneDate;

                sharepointListItem.Update();
                listItems.Add(caseDateTimeslot, sharepointListItem);
            } 
                        
            try
            {
                clientContext.ExecuteQuery();
            }
            catch (Exception)
            {
                throw new Exception($"Unable to save timeregs to Toolkit. Toolkit: DisplayName={toolkit.DisplayName}, Url={toolkit.Url}");
            }

            foreach (var item in listItems)
            {
                var isSynchronised = item.Value.Id != -1;
                foreach (var timereg in timeregsByCaseIdDateTimeslot[item.Key])
                {
                    timereg.IsSynchronized = isSynchronised;
                }
            }
        }

        public virtual double CalculateDuration(List<Timeregistration> timeregs)
        {
            double duration = 0;
            foreach (var timereg in timeregs)
            {
                var success = timereg.CouldConvertDurationToHours();
                if (!success)
                    throw new ArgumentException($"The amount of hours given ({timereg.Duration}) for the timeregistration is invalid. Timeregistration: Toolkit={timereg.ToolkitDisplayName}, Team={timereg.Team}, Case ID={timereg.CaseId}, timeslot={timereg.Timeslot}");

                duration += timereg.Hours;
            }

            return duration;
        }

        public virtual Dictionary<CaseDateTimeslot,List<Timeregistration>> GroupTimeregistrations(IEnumerable<Timeregistration> timeregs)
        {
            return timeregs.GroupBy(timereg => new CaseDateTimeslot {CaseId = timereg.CaseId, Date = timereg.DoneDate, Timeslot = timereg.Timeslot})
                .ToDictionary(x => x.Key, x => x.ToList());
        }

        public virtual List<Team> GetTeamsFromToolkit(ToolkitUser tkUser, Toolkit tk)
        {
            var clientContext = new ClientContext(tk.Url) { Credentials = new NetworkCredential(tkUser.Name, tkUser.SecurePassword, tkUser.Domain) };

            return tk.GetTeamsWithoutSLA ?  GetAllTeams(clientContext) : GetTeamsWithActiveSLA(clientContext);
        }

        public virtual double CalculateTimespan(Timeslot timeslot)
        {
            var now = DateTime.Now;
            var time1 = new DateTime();
            var time2 = new DateTime();
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

        public virtual Toolkit GetTimeslotInformationFromToolkit(ToolkitUser tkUser, Toolkit tk)
        {
            var clientContext = new ClientContext(tk.Url) { Credentials = new NetworkCredential(tkUser.Name, tkUser.SecurePassword, tkUser.Domain) };

            const string list = "tidsregistrering";
            var oList = clientContext.Web.Lists.GetByTitle(list);
            clientContext.Load(oList);
            clientContext.ExecuteQuery();

            var usesTimeslots = CheckIfToolkitUsesTimeslots(clientContext, oList, tk);

            if (!usesTimeslots) return tk;

            var timeregsWithTimeslot = GetTopTimeregsWithTimeslot(clientContext, oList, tk);

            var casesForTimeregsWithTimeslot =
                GetCasesForTimeregsWithTimeslot(clientContext, timeregsWithTimeslot);

            var teamsUsingTimeslots = casesForTimeregsWithTimeslot.GroupBy(tkCase => tkCase.Team)
                .Select(tkCase => tkCase.First()).OrderBy(tkCase => tkCase.Team).ToList();

            foreach (var tkCase in teamsUsingTimeslots)
            {
                foreach (var team in tk.Teams)
                {
                    if (team.Name == tkCase.Team) team.UsesTimeslots = true;
                }
            }

            tk.Timeslots = tk.TimeslotFieldName == "TimeSlot" ? ExtractTimeslotsFromTimeslotList(clientContext) : ExtractTimeslotsFromTimeregList(clientContext, tk);

            return tk;
        }

        private static List<Timeslot> ExtractTimeslotsFromTimeslotList(ClientContext clientContext)
        {
            const string list = "Time Slots";
            var oList = clientContext.Web.Lists.GetByTitle(list);
            clientContext.Load(oList);
            clientContext.ExecuteQuery();

            var query = new CamlQuery
            {
                ViewXml = @"<View><Query><OrderBy><FieldRef Name='ID' Ascending='FALSE'/></OrderBy></Query><ViewFields><FieldRef Name='ID'/><FieldRef Name='Title'/></ViewFields></View>"
            };

            var listItems = oList.GetItems(query);
            clientContext.Load(listItems);
            clientContext.ExecuteQuery();

            var timeslots = new List<Timeslot>();

            foreach (var item in listItems)
            {
                var timeslot = new Timeslot
                {
                    TimeInterval = new TimeInterval
                    {
                        Id = (from field in item.FieldValues where field.Key == "ID" select (int)field.Value).Single(),
                        Interval = (from field in item.FieldValues
                                    where field.Key == "Title"
                                    select (string)field.Value)
                            .Single()
                    }
                };
                timeslots.Add(timeslot);
            }

            return timeslots.OrderBy(ts => ts.TimeInterval.Id).ToList();
        }

        // ReSharper disable once InconsistentNaming
        private static List<Team> GetTeamsWithActiveSLA(ClientContext clientContext)
        {
            const string list = "SLA";
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

                    var enumFlv = (IEnumerable) fv.Value;

                    if (enumFlv != null)
                    {
                        teams.AddRange(from FieldLookupValue flv in enumFlv
                            select new Team()
                            {
                                Name = flv.LookupValue
                            });
                    }
                    else
                    {
                        var flv = (FieldLookupValue)fv.Value;
                        var team = new Team()
                        {
                            Name = flv.LookupValue
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

            return teams.GroupBy(team => team.Name).Select(g => g.First()).OrderBy(team => team.Name).ToList();
        }

        private static List<Timeslot> ExtractTimeslotsFromTimeregList(ClientContext clientContext, Toolkit tk)
        {
            var timeslots = new List<Timeslot>();
            const string list = "tidsregistrering";
            var oList = clientContext.Web.Lists.GetByTitle(list);
            clientContext.Load(oList);
            clientContext.ExecuteQuery();

            var rowLimit = 5;
            var loopCounter = 0;
            double timespan = 0;

            var query = new CamlQuery
            {
                ViewXml =
                    $@"<View><Query><Where><IsNotNull><FieldRef Name='{
                            tk.TimeslotFieldName
                        }'/></IsNotNull></Where><OrderBy><FieldRef Name='ID' Ascending='FALSE'/></OrderBy></Query><ViewFields><FieldRef Name='{
                            tk.TimeslotFieldName
                        }' /></ViewFields><RowLimit>{rowLimit}</RowLimit></View>"
            };

            while (timespan < 24 && loopCounter < 10)
            {
                var listItems = oList.GetItems(query);
                clientContext.Load(listItems);
                clientContext.ExecuteQuery();

                foreach (var item in listItems)
                {
                    if (tk.TimeslotIsFieldLookup)
                    {
                        var timeInterval = (
                            from field in item.FieldValues
                            where field.Key == tk.TimeslotFieldName
                            select new TimeInterval
                            {
                                Id = ((FieldLookupValue)field.Value).LookupId,
                                Interval = ((FieldLookupValue)field.Value).LookupValue
                            }
                            ).Single();

                        if (timeslots.All(ti => ti.TimeInterval.Id != timeInterval.Id))
                            timeslots.Add(new Timeslot {TimeInterval = timeInterval});
                    }
                    else
                    {
                        var timeInterval = (
                            from field in item.FieldValues
                            where field.Key == tk.TimeslotFieldName
                            select new TimeInterval
                            {
                                Id = -1,
                                Interval = (string) field.Value
                            }
                        ).Single();
                        if (timeslots.All(ti => ti.TimeInterval.Interval != timeInterval.Interval))
                            timeslots.Add(new Timeslot { TimeInterval = timeInterval });
                    }
                }

                query = UpdateCamlQuery(timeslots, tk.TimeslotFieldName, rowLimit);

                loopCounter++;
            }

            return timeslots;
        }

        public static CamlQuery UpdateCamlQuery(List<Timeslot> timeslots, string tkTimeslotFieldName, int rowLimit)
        {
            var equalsQuery = timeslots.Select(timeslot => TextNotEquals(tkTimeslotFieldName, timeslot.TimeInterval.Interval))
                .Aggregate(WrapInAnd);

            var equalsNotNullQuery = WrapInAnd(equalsQuery, $@"<IsNotNull><FieldRef Name='{tkTimeslotFieldName}'/></IsNotNull>");

            return new CamlQuery
            {
                ViewXml =
                    $@"<View><Query><Where>{
                            equalsNotNullQuery
                        }</Where><OrderBy><FieldRef Name='ID' Ascending='FALSE'/></OrderBy></Query><ViewFields><FieldRef Name='{
                            tkTimeslotFieldName
                        }' /></ViewFields><RowLimit>{rowLimit}</RowLimit></View>"
            };
        }

        private static string WrapInAnd(string first, string second)
        {
            return $"<And>{first}{second}</And>";
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
                    var timeslot = new Timeslot { TimeInterval = ExtractTimeIntervalFromTimeslot(tkTimeslot) };
                    timeslots.Add(timeslot);
                }
            }
            else
            {
                foreach (var tkTimeslot in listOfTimregsWithTimeslot)
                {
                    if (timeslots.Any(ts => ts.TimeInterval.Interval == tkTimeslot.Timeslot)) continue;
                    var timeslot = new Timeslot { TimeInterval = ExtractTimeIntervalFromTimeslot(tkTimeslot) };
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

        private static IEnumerable<ToolkitCase> GetCasesForTimeregsWithTimeslot(ClientContext clientContext, IEnumerable<TimeregWithTimeslot> timeregsWithTimeslot)
        {
            const string list = "sager";
            var spList = clientContext.Web.Lists.GetByTitle(list);
            clientContext.Load(spList);
            clientContext.ExecuteQuery();

            var uniqueTimeregsWithTimeslot = timeregsWithTimeslot.GroupBy(timereg => timereg.CaseId)
                .Select(g => g.First()).OrderBy(timereg => timereg.CaseId).ToList();

            var uniqueCaseIds = (from uniqueTimereg in uniqueTimeregsWithTimeslot select uniqueTimereg.CaseId).ToList();

            var query = GenerateCamlQueryForIds(uniqueCaseIds);

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
        }

        private static List<TimeregWithTimeslot> GetTopTimeregsWithTimeslot(ClientRuntimeContext clientContext, List spList, Toolkit tk)
        {
            var rowLimit = 250;
            var query = new CamlQuery()
            {
                ViewXml = $@"<View><Query><Where><IsNotNull><FieldRef Name='{tk.TimeslotFieldName}'/></IsNotNull></Where><OrderBy><FieldRef Name='ID' Ascending='FALSE'/></OrderBy></Query><ViewFields><FieldRef Name='Sag_x003a_Sags_x0020_Id' /><FieldRef Name='{tk.TimeslotFieldName}' /></ViewFields><RowLimit>{rowLimit}</RowLimit></View>"
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
                            select ((FieldLookupValue)field.Value).LookupValue).Single()
                        : (from field in item.FieldValues
                            where field.Key == tk.TimeslotFieldName
                            select field.Value.ToString()).Single(),
                    TimeslotId = tk.TimeslotIsFieldLookup
                        ? (from field in item.FieldValues
                            where field.Key == tk.TimeslotFieldName
                            select ((FieldLookupValue)field.Value).LookupId).Single()
                        : -1,
                    CaseId = (from field in item.FieldValues
                        where field.Key == "Sag_x003a_Sags_x0020_Id"
                        select ExtractCaseIdFromField(((FieldLookupValue)field.Value).LookupValue)).Single()
                };

                listOfTimeregsWithTimelots.Add(timeregWithTimeslot);
            }

            return listOfTimeregsWithTimelots;
        }

        private static int ExtractCaseIdFromField(string spCaseId)
        {
            if (spCaseId == "Beregnes") return -1;
            var rx = new Regex(@"(\w+)\-(\d+)");
            var matches = rx.Matches(spCaseId);

            int caseId;
            int.TryParse(matches[0].Groups[2].Value, out caseId);
            return caseId;
        }

        private static CamlQuery GenerateCamlQueryForIds(IEnumerable<int> listOfCaseIds)
        {
            var values = listOfCaseIds.Select(caseId => TextEquals("ID", caseId)).Aggregate(WrapInOr);

            return new CamlQuery
            {
                ViewXml =
                    $@"<View><Query><Where>{
                            values
                        }</Where></Query><ViewFields><FieldRef Name='ID' /><FieldRef Name='Team' /></ViewFields></View>"
            };
        }

        private static string WrapInOr(string first, string second)
        {
            return $"<Or>{first}{second}</Or>";
        }

        private static string TextEquals(string column, int value)
        {
            return $"<Eq><FieldRef Name='{column}' /><Value Type='Text'>{value}</Value></Eq>";
        }

        private static string TextNotEquals(string column, string value)
        {
            return $"<Neq><FieldRef Name='{column}' /><Value Type='Text'>{value}</Value></Neq>";
        }

        private static string TextEquals(string column, string value)
        {
            return $"<Eq><FieldRef Name='{column}' /><Value Type='Text'>{value}</Value></Eq>";
        }

        private static bool CheckIfToolkitUsesTimeslots(ClientContext clientContext, List spList, Toolkit tk)
        {
            var usesDefaultTimeSlotList = CheckForDefaultTimeSlotList(clientContext, spList);

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
            while (counter < 4)
            {
                var listItems = spList.GetItems(query);
                clientContext.Load(listItems);
                clientContext.ExecuteQuery();

                var rx = new Regex(@"\d{2}\:\d{2}\s{0,1}\-\s{0,1}\d{2}\:\d{2}");
                foreach (var item in listItems)
                {
                    foreach (var field in item.FieldValues)
                    {
                        if (field.Value == null) continue;
                        if (field.Value is FieldUserValue) continue;
                        var value = field.Value as FieldLookupValue;
                        if (value != null)
                        {
                            if (value.LookupValue == null) continue;
                            if (!rx.IsMatch(value.LookupValue)) continue;
                            tk.TimeslotFieldName = field.Key;
                            tk.TimeslotIsFieldLookup = true;
                            return true;
                        }
                        if (!rx.IsMatch(field.Value.ToString())) continue;
                        tk.TimeslotFieldName = field.Key;
                        tk.TimeslotIsFieldLookup = false;
                        return true;

                    }
                }
                query.ListItemCollectionPosition = listItems.ListItemCollectionPosition;

                counter++;
            }

            return false;
        }

        private static bool CheckForDefaultTimeSlotList(ClientRuntimeContext clientContext, List spList)
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

    public class CaseDateTimeslot
    {
        public int CaseId { get; set; }
        public DateTime Date { get; set; }
        public string Timeslot { get; set; }
    }
}
