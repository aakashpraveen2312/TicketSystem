using System;
using System.Collections.Generic;

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

        public int TotalAccountantCount { get; set; }
        public int TotalHelpdeskCount { get; set; }
        public int TotalTicketCount { get; set; }
        public int TotalClosedTicketCount { get; set; }

        public string Admindescription { get; set; }
        public string Managerdescription { get; set; }
        public string Userdescription { get; set; }

        public string Helpdeskdescription { get; set; }
        public string Accountantdescription { get; set; }
        public string Ticketname { get; set; }
        public string ClosedTicketname { get; set; }
    }

    public class Tickethistorys
    {
        public string TicketId { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string CustomerName { get; set; }
        public DateTime Date { get; set; }
    }

    public class TicketResponse
    {
        public string Status { get; set; }

        public List<Tickethistorys> tickets { get; set; }
    }
}