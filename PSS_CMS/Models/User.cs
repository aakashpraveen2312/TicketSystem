using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class User
    {
        public int L_RECID { get; set; }
        public int L_USERNAME { get; set; }
        public int L_PASSWORD { get; set; }
        public int L_ROLE { get; set; }
        public int L_EMAILID { get; set; }
        public int L_COMPANYID { get; set; }
        public int L_USERID { get; set; }
        public int L_PROJECTID { get; set; }
        public int L_Contact { get; set; }
        public int L_UGRECID { get; set; }
        public int L_DOMAIN { get; set; }
        public int L_SORTORDER { get; set; }
        public int L_DISABLE { get; set; }
    }

    public class ApiResponseUser
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public User Data { get; set; }
        public List<User> Datas { get; set; }
    }
}