using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.SharePoint;
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
            ClientContext clientContext = new ClientContext(toolkit.Url);
            clientContext.Credentials = new NetworkCredential(toolkitUser.Name, toolkitUser.Password, toolkitUser.Domain);

            var timeregList = "tidsregistrering";
            var sharepointList = clientContext.Web.Lists.GetByTitle(timeregList);
            clientContext.Load(sharepointList);
            clientContext.ExecuteQuery();

            var doneBy = new SPFieldLookupValue(toolkit.UserId, toolkitUser.Name);
            var author = new SPFieldLookupValue(toolkit.UserId, toolkitUser.Name);
            var toolkitCase = new SPFieldLookupValue(timereg.CaseId, $"{timereg.Customer}-{timereg.CaseId}");

            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            ListItem sharepointListItem = sharepointList.AddItem(itemCreateInfo);

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

        public void MakeTimeregistrations(List<Timeregistration> timeregs, ToolkitUser toolkitUser, Toolkit toolkit)
        {
            //Do some loop over list where we create Microsoft.SharePoint.Client.ListItem and put into SP.List oList -- SEE UNIT TEST PROJECT
            //Send to Toolkit -- SEE UNIT TEST PROJECT
            throw new NotImplementedException();
        }

        public List<TimeSlot> GetTimeSlotsFromToolkit(ToolkitUser tkUser, Toolkit tk)
        {
            var clientContext = new ClientContext(tk.Url) {Credentials = new NetworkCredential(tkUser.Name, tkUser.Password, tkUser.Domain)};

            var list = "tidsregistrering";
            var oList = clientContext.Web.Lists.GetByTitle(list);
            clientContext.Load(oList);
            clientContext.ExecuteQuery();

            string timeSlotFieldName = "";
            List<TimeSlot> timeslots = new List<TimeSlot>();

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
                        var timeslot = new TimeSlot();
                        try
                        {
                            if (field.Value.GetType() == typeof(FieldLookupValue))
                            {
                                if (!rx.IsMatch(((FieldLookupValue)field.Value).LookupValue)) continue;
                                timeslot.FieldName = field.Key;
                                timeslot.IsFieldLookup = true;
                                FieldLookupValue fvl = (FieldLookupValue)field.Value;
                                timeslot.TimeInterval = new TimeInterval() { Id = fvl.LookupId, Interval = fvl.LookupValue };
                                timeslots.Add(timeslot);
                            }
                            else
                            {
                                if (!rx.IsMatch(field.Value.ToString())) continue;
                                timeSlotFieldName = field.Key;
                                if (!timeslots.Contains(field.Value.ToString()))
                                {
                                    timeslots.Add(field.Value.ToString());
                                }
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                timespan = timeslots.Sum((Func<string, double>)CalculateTimespan);
                query = UpdateCamlQuery(timeSlotFieldName, timeslots, timeregsPerPage);
            }
        }

        private CamlQuery UpdateCamlQuery(string fieldName, List<string> timeSlots, int rowLimit)
        {
            List<CamlQuery> subQueries = timeSlots.Select(timeSlot => new CamlQuery()
                {
                    ViewXml = $@"<Neq><FieldRef Name='{fieldName}'/><Value Type='Text'>{timeSlot}</Value></Neq>"
                })
                .ToList();

            subQueries.Add(new CamlQuery() { ViewXml = $@"<IsNotNull><FieldRef Name=""{fieldName}""/></IsNotNull>" });

            CamlQuery query = new CamlQuery()
            {
                ViewXml = $@"</Where><OrderBy><FieldRef Name='ID' Ascending='FALSE'/></OrderBy></Query><RowLimit>{rowLimit}</RowLimit></View>"
            };

            for (int i = 0; i < subQueries.Count - 1; i++)
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

        private double CalculateTimespan(string timeslot)
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
    }
}
