using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class ContractUser
    {
        public int SerialNumber { get; set; }
        public int U_CONTRACTRECID { get; set; }
        public string U_USERCODE { get; set; }
        [DisplayName("First Name")]
        public string U_CONTRACTUSERFIRSTNAME { get; set; }
        [DisplayName("last Name")]
        public string U_CONTRACTUSERLASTNAME { get; set; }
        [DisplayName("Email")]
        public string U_CONTRACTUSEREMAIL { get; set; }
        [DisplayName("Mobile")]
        public string U_CONTRACTMOBILE { get; set; }
        [DisplayName("Desigination")]
        public string U_CONTRACTDESIGNATION { get; set; }
    }
    public class RootObjectsContractUser
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<ContractUser> Data { get; set; }
    }

    public class ContractUserMasterObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public ContractUser Data { get; set; }
    }
}