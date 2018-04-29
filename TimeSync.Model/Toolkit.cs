using System;
using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSync.Model
{
    public class Toolkit 
    {
        public int UserId { get; set; }
        public string Url { get; set; }
        public List<Team> Teams { get; set; }
        public string CustomerName { get; set; }
    }
    //ReSharper disable InconsistentNaming
    public class Team
    {
        public string Name { get; set; }
        public bool UsesTimeSlots { get; set; }
        public string TimeSlotFieldName { get; set; }
        public List<TimeSlot> TimeSlots { get; set; }
        public CamlQuery GetSPQuery(int rowLimit)
        {
            var query = new CamlQuery()
            {
                ViewXml = $@"
                        <View>
                            <Query>
                                <Where>
                                    <Eq>
                                        <FieldRef Name='Team'></FieldRef>
                                        <Value Type ='Text'>{Name}</Value>
                                    </Eq>
                                </Where>
                                <OrderBy>  
                                    <FieldRef Name='ID' Ascending='FALSE'/>   
                                </OrderBy>
                            </Query>
                            <RowLimit>{rowLimit}</RowLimit>
                        </View>"
            };

            return query;
        }
    }
    public class TimeSlot
    {
        public bool IsFieldLookup { get; set; }
        public TimeInterval TimeInterval { get; set; }
        public void GetSPQuery(ListItem listItem, TimeInterval timeInterval)
        {
            throw new NotImplementedException();
        }
    }
    public struct TimeInterval
    {
        public int Id;
        public string Interval;
    }
}
