﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Dashborardchart
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int CloseTickets { get; set; }
        public int OpenDate { get; set; }
        public int OpenLastweek { get; set; }
        public int OpenLastMonth { get; set; }

        public string Status { get; set; }
        public MonthWiseData MonthWise { get; set; }
        public WeekWiseData WeekWise { get; set; }

        public List<Ticketdashboard> RecentTickets { get; set; }
    }
    public class MonthWiseData
    {
        public int MonthTotalTickets { get; set; }
        public int MonthOpenTickets { get; set; }
        public int MonthCloseTickets { get; set; }
        public int MonthResolvedTickets { get; set; }
    }
    public class WeekWiseData
    {
        public int WeekTotalTickets { get; set; }
        public int WeekOpenTickets { get; set; }
        public int WeekCloseTickets { get; set; }
        public int WeekResolvedTickets { get; set; }
    }
    public class Ticketdashboard
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }      
    }
}