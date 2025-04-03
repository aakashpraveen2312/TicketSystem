using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Timeline
    {
        public string status { get; set; }
        public string date { get; set; }
        public string Username { get; set; }
    }

    public class TimelineResponse
    {
        public string Status { get; set; }
        public List<Timeline> TicketTimeline { get; set; }
    }
}