using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
