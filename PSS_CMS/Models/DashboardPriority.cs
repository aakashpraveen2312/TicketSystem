using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class DashboardPriority
    {
        public string PriorityType { get; set; }
        public int TotalTickets { get; set; }
        public int ClosedTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int OpenTickets { get; set; }
    }
    public class DashboardPriorityList
    {
        public List<DashboardPriority> Data { get; set; }
    }
}