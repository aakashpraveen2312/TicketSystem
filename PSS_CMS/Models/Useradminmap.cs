using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Useradminmap
    {
        public bool USERSELECTED { get; set; }
        public string L_USERNAME { get; set; }
        public string L_ROLE { get; set; }
        public string L_USERID { get; set; }
        public int UA_RECID { get; set; }

        public int P_PROJECTRECID { get; set; }
        public string P_CODE { get; set; }
        public string P_NAME { get; set; }
        public int P_SORTORDER { get; set; }
        public int P_CRECID { get; set; }
        public string P_DISABLE { get; set; }
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