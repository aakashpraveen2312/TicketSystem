using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class TicketAgeingViewModel
    {
        public string TicketNo { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public string Status { get; set; }
        public double AgeHours { get; set; }
        public string AgeBucket { get; set; }
    }

}