using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Useradminmap
    {
        public bool USERSELECTED { get; set; }
        public bool Selected { get; set; }

        public int UA_RECID { get; set; }
        public int U_RECID { get; set; }
        public string U_USERNAME { get; set; }
        public string U_RCODE { get; set; }
        public string U_CRECID { get; set; }
        public string U_USERCODE { get; set; }

        public string P_CODE { get; set; }
        public string P_NAME { get; set; }
        public int P_SORTORDER { get; set; }
        public int P_CRECID { get; set; }
        public int P_RECID { get; set; }
        public int CU_RECID { get; set; }
        public string P_DISABLE { get; set; }
        public string CU_NAME { get; set; }
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