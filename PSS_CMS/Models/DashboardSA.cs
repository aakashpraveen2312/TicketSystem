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

    }
    public class SAdashboard
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
    }
}