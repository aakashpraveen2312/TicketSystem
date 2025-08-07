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
        public string U_USERNAME { get; set; }

        [DisplayName("Password")]
        [Required]
        public string U_PASSWORD { get; set; }

        [DisplayName("Domain")]
        [Required]
        public string U_DOMAIN { get; set; }

        [DisplayName("User ID")]
        [Required]
        public string U_EMAILID { get; set; }
        public string U_RCODE { get; set; }
        public string U_DISABLE { get; set; }
        public string U_USERCODE { get; set; }
        public string U_MOBILENO { get; set; }
        public string MaterialConsumption { get; set; }

        public int U_RECID { get; set; }
        public int U_SORTORDER { get; set; }
        public int U_CRECID { get; set; }
        public string U_USERMANAGER { get; set; }



    }

    public class APIResponseLogin
    {
        public List<Login> Data { get; set; }
        public string U_USERNAME { get; set; }
        public string U_EMAILID { get; set; }
        public string U_PASSWORD { get; set; }
        public string U_DOMAIN { get; set; }
        public string U_RCODE { get; set; }
        public string U_DISABLE { get; set; }
        public string U_USERCODE { get; set; }
        public string U_MOBILENO { get; set; }
        public string Message { get; set; }
        public string Warning { get; set; }
        public string expiryWarning { get; set; }
        public string Status { get; set; }
        public string APIkey { get; set; }
        public string MaterialConsumption { get; set; }
        public int U_RECID { get; set; }
        public int U_SORTORDER { get; set; }
        public int U_CRECID { get; set; }
    }

}