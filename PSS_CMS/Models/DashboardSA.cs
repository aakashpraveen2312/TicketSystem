using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class DashboardSA
    {
        public int TotalUserCount { get; set; }
        public int TotalCustomerCount { get; set; }
        public int TotalProductCount { get; set; }
        public int TotalRoleCount { get; set; }
        public int OpenDate { get; set; }
        public int OpenLastweek { get; set; }
        public int OpenLastMonth { get; set; }
    }
    public class SAdashboard
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
    }
}