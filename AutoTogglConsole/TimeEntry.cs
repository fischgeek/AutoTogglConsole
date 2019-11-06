using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTogglConsole
{
    public class TimeEntryWrapper
    {
        public TimeEntry time_entry { get; set; }
    }
    public class TimeEntryData
    {
        public TimeEntry data { get; set; }
    }
    public class TimeEntry
    {
        public int id { get; set; }
        public string description { get; set; }
        public int wid { get; set; } // workspace id
        public int pid { get; set; } // project id
        public int tid { get; set; } // task id
        public bool billable { get; set; }
        public DateTime start { get; set; }
        public DateTime stop { get; set; }
        public int duration { get; set; }
        public string created_with { get; set; }
        public string[] tags { get; set; }
        public bool duronly { get; set; }
        public DateTime at { get; set; }
    }
}
