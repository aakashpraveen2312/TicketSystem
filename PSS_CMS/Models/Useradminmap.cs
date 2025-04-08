using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Useradminmap
    {
        public int UA_RECID { get; set; }
        public string UA_USERID { get; set; }
        public string UA_ADMINID { get; set; }
        public int UA_CRECID { get; set; }
    }
    public class UserAdminRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<Useradminmap> Data { get; set; }
    }
    public class UserAdminObjects
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public Useradminmap Data { get; set; }
    }
}