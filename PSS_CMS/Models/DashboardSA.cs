using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class DashboardSA
    {
        public int TotalAdminCount { get; set; }
        public int TotalManagerCount { get; set; }
        public int TotalUserCount { get; set; }
        public int TotalCustomerCount { get; set; }
        public int TotalProductCount { get; set; }
        public int TotalCompanyLocationCount { get; set; }
        //Changes - Aakash
        public int TotalAccountantCount { get; set; }
        public int TotalHelpdeskCount { get; set; }
        public int TotalTicketCount { get; set; }
        public int TotalClosedTicketCount { get; set; }

        public string Admindescription { get; set; }
        public string Managerdescription { get; set; }
        public string Userdescription { get; set; }
        //Changes - Aakash
        public string Helpdeskdescription { get; set; }
        public string Accountantdescription { get; set; }
        public string Ticketname { get; set; }
        public string ClosedTicketname { get; set; }
    }
    public class SAdashboard
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
    }
}