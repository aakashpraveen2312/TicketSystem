using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class DashboardUser
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int CloseTickets { get; set; }
        public int OpenDate { get; set; }
        public int OpenLastweek { get; set; }
        public int OpenLastMonth { get; set; }

        public string Status { get; set; }
        public MonthWiseDataUser MonthWise { get; set; }
        public WeekWiseDataUser WeekWise { get; set; }

        public List<TicketdashboardUser> RecentTickets { get; set; }
    }
    public class MonthWiseDataUser
    {
        public int MonthTotalTickets { get; set; }
        public int MonthOpenTickets { get; set; }
        public int MonthCloseTickets { get; set; }
        public int MonthResolvedTickets { get; set; }
    }
    public class WeekWiseDataUser
    {
        public int WeekTotalTickets { get; set; }
        public int WeekOpenTickets { get; set; }
        public int WeekCloseTickets { get; set; }
        public int WeekResolvedTickets { get; set; }
    }
    public class TicketdashboardUser
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
    }
}