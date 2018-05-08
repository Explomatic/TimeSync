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
        public bool GetTeamsWithoutSLA { get; set; }
        public string TimeslotFieldName { get; set; }
        public bool TimeslotIsFieldLookup { get; set; }
        public List<Timeslot> Timeslots { get; set; }
        public string CustomerName { get; set; }
        public string DisplayName { get; set; }
    }
    public class Team
    {
        public string Name { get; set; }
        public bool UsesTimeslots { get; set; } = false;
    }
    public class Timeslot
    {
        public TimeInterval TimeInterval { get; set; }
    }
    public struct TimeInterval
    {
        public int Id;
        public string Interval;
    }

    public class TimeregWithTimeslot
    {
        public int CaseId { get; set; }
        public string Timeslot { get; set; }
        public int TimeslotId { get; set; }
    }

    public class ToolkitCase
    {
        public int CaseId { get; set; }
        public string Team { get; set; }
    }
}
