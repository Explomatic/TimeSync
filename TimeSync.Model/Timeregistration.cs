using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TimeSync.Model
{
    public class Timeregistration
    {
        public int CaseId { get; set; }
        public DateTime DoneDate { get; set; }
        public string Customer { get; set; }
        public double Hours { get; set; }
        public bool IsSynchronized { get; set; }

        public ObservableCollection<string> ListOfToolkitNames { get; set; }

        public Timeregistration()
        {
            ListOfToolkitNames = new ObservableCollection<string>();
            DoneDate = DateTime.Today;
        }

        public bool AddHours(string duration)
        {
            var durationIsNumberOfHours = CheckIfDurationIsNumberOfHours(duration);
            double hours;
            bool stringFormattedAsExpected;
            if (durationIsNumberOfHours)
            {
                duration = duration.Replace(',', '.');
                stringFormattedAsExpected = double.TryParse(duration, NumberStyles.Any, CultureInfo.InvariantCulture, out hours);
            }
            else
            {
                stringFormattedAsExpected = ParseTimeIntervalString(duration, out hours);
            }

            Hours = hours;
            return stringFormattedAsExpected;

        }

        private bool CheckIfDurationIsNumberOfHours(string duration)
        {
            return duration.Trim().Length < 5;
        }


        private bool ParseTimeIntervalString(string duration, out double hours)
        {
            hours = 0;
            // expected string format "08(:,.)00 - 08(:,.)30"
            var result = Regex.Matches(duration, @"([\d]+)[\:\,\.]([\d]+)|([\d]+)");
            if (result.Count == 0)
            {
                return false;
            }
            else
            {
                var fromTime = result[0].Value;
                var toTime = result[1].Value;

                DateTime from = ConvertToDateTime(fromTime);
                DateTime to = ConvertToDateTime(toTime);
                var timespan = to.Subtract(from);
                hours = timespan.TotalHours;

                return true;
            }
        }

        private DateTime ConvertToDateTime(string time)
        {
            DateTime dt;
            if (time.Contains(":"))
            {
                var couldParse = DateTime.TryParse(time, out dt);
            }
            else if (time.Contains(",") || time.Contains("."))
            {
                var idx = time.IndexOfAny(",.".ToCharArray());
                var newTime = $"{time.Substring(0, idx)}:{time.Substring(idx)}";
                var couldParse = DateTime.TryParse(newTime, out dt);
            }
            else
            {
                var newTime = $"{time.Substring(0, 2)}:{time.Substring(3)}";
                var couldParse = DateTime.TryParse(newTime, out dt);
            }

            return dt;
        }
    }
}
