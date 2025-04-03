using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Login
    {
        public string L_RECID { get; set; }

        [DisplayName("User Name")]
        [Required]
        public string L_USERNAME { get; set; }

        [DisplayName("Password")]
        [Required]
        public string L_PASSWORD { get; set; }
        public string L_ROLE { get; set; }
        public string L_SORTORDER { get; set; }
        public string L_DISABLE { get; set; }
        public string L_EMAILID { get; set; }

        public string Message { get; set; }

        public string Status { get; set; }
        public string L_USERID { get; set; }
        public string L_COMPANYID { get; set; }
        public string L_PROJECTID { get; set; }

    }

     public class APIResponseLogin
    {
        public List<Login> Data { get; set; }
        public string Status { get; set; }
        public string L_PASSWORD { get; set; }
        public string L_ROLE { get; set; }
        public string L_SORTORDER { get; set; }
        public string L_DISABLE { get; set; }
        public string L_EMAILID { get; set; }
        public string Message { get; set; }
        public string L_USERNAME { get; set; }
        public string L_USERID { get; set; }
        public string L_COMPANYID { get; set; }
        public string L_PROJECTID { get; set; }
     

    }

}