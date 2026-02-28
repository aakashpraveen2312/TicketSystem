using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class TicketAgeingViewModel
    {
        public string TicketNo { get; set; }
        public string ticketDate { get; set; }
        public string referenceNo { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public string Status { get; set; }
        public double AgeHours { get; set; }
        public string ageText { get; set; }
        public string AgeBucket { get; set; }
    }
    public class TIcketagePdfObject
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string FileUrl { get; set; }
        public List<TicketAgeingViewModel> Data { get; set; }


    }


}