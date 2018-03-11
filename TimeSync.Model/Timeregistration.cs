using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        public bool AddHours(string duration)
        {
            double hours;
            bool durationIsNumberOfHours = double.TryParse(duration, out hours);
            if (durationIsNumberOfHours)
            {
                Hours = hours;
                return true;
            }
            else
            {
                // expected string format "08(:,.)00 - 08(:,.)30"
                bool stringFormattedAsExpected = ParseDurationString(duration, out hours);
                return stringFormattedAsExpected;
            }
            
        }

        private bool ParseDurationString(string duration, out double hours)
        {
            hours = 0;
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

                //if (ContainsTimeDelimiter(fromTime) && ContainsTimeDelimiter(toTime))
                //{
                //    hours = CalculateTimeDurationWithDelimiter(fromTime, toTime);
                //}
                //else
                //{
                //    hours = CalculateTimeDurationWithoutDelimiter(fromTime, toTime);
                //}


                return true;
            }
        }

        private DateTime ConvertToDateTime(string time)
        {
            DateTime dt;
            if (time.Contains(":"))
            {
                bool couldParse = DateTime.TryParse(time, out dt);
            }
            else if (time.Contains(",") || time.Contains("."))
            {
                var idx = time.IndexOfAny(",.".ToCharArray());
                var newTime = $"{time.Substring(0, idx)}{time.Substring(idx)}";
                bool couldParse = DateTime.TryParse(newTime, out dt);
            }
            else
            {
                var newTime = $"{time.Substring(0, 2)}:{time.Substring(3)}";
                bool couldParse = DateTime.TryParse(newTime, out dt);
            }

            return dt;
        }

        private double CalculateTimeDurationWithoutDelimiter(string fromTime, string toTime)
        {
            return 0;
        }

        private double CalculateTimeDurationWithDelimiter(string fromTime, string toTime)
        {
            DateTime fromDateTime;
            DateTime toDateTime;

            bool couldParseFrom = DateTime.TryParse(fromTime, out fromDateTime);
            bool couldParseTo = DateTime.TryParse(toTime, out toDateTime);

            if (couldParseFrom && couldParseTo)
            {
                
            }
            return 0;
        }

        private bool ContainsTimeDelimiter(string time)
        {
            return time.Any(element => ":".ToCharArray().Contains((element)));
        }
    }
}
