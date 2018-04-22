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
        public bool HasTimeSlots { get; set; }
        public string CustomerName { get; set; }
        public List<TimeSlot> TimeSlots { get; set; }
    }
    //ReSharper disable InconsistentNaming
    public class TimeSlot
    {
        public bool IsFieldLookup { get; set; }
        public string FieldName { get; set; }
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
