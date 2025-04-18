using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class CompanyInfo
    {
        public int C_RECID { get; set; }
        [DisplayName("Code")]

        public string C_CODE { get; set; }
        [DisplayName("Name")]

        public string C_NAME { get; set; }
        [DisplayName("Address")]

        public string C_ADDRESS { get; set; }
        [DisplayName("Country")]

        public string C_COUNTRY { get; set; }
        public string C_PINCODE { get; set; }
        [DisplayName("Mobile Number")]

        public string C_PHONE { get; set; }
        public string C_WEB { get; set; }
        [DisplayName("Email ID")]

        public string C_EMAILID { get; set; }

        public int C_SORTORDER { get; set; }
        public string C_DISABLE { get; set; }
        public string C_APPUSERNAME { get; set; }
        [DisplayName("Domain")]

        public string C_DOMAIN { get; set; }


        [RegularExpression(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[0-9A-Z]{1}Z[0-9A-Z]{1}$", ErrorMessage = "GST must follow the valid format (e.g., 22AAAAA0000A1Z5)")]
        [StringLength(15, ErrorMessage = "GST must be exactly 15 characters long.")]
        public string C_GST { get; set; }

        public string C_GSTATTACHEMENT { get; set; }


        [StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"^[A-Z]{5}\d{4}[A-Z]$", ErrorMessage = "PAN number must be 10 characters (ABCDE1234F)")]
        public string C_PANNUMBER { get; set; }

        public string C_PANATTACHEMENT { get; set; }

        [StringLength(11)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "TIN number must be 11 digits (27123456789)")]
        public string C_TINNUMBER { get; set; }

        public string C_TINATTACHEMENT { get; set; }

        [Required(ErrorMessage = "* Mandatory")]
        [RegularExpression(@"^[a-zA-Z\s\.]*$", ErrorMessage = "Name should not contain digits")]
        public string C_AUTHSIGNNAME { get; set; }

        [Required(ErrorMessage = "* Mandatory")]
        [RegularExpression(@"^[a-zA-Z\s\.]*$", ErrorMessage = "Name should not contain digits")]
        public string C_AUTHPOSITION { get; set; }
        public string C_AUTHORIZESIGNATURE { get; set; }
        public string C_HEADERIMAGE { get; set; }
        public string C_FOOTERIMAGE { get; set; }
        public string C_BANKNAME { get; set; }
        public string C_BRANCHNAME { get; set; }
        public string C_ACCOUNTNO { get; set; }
        public string C_ACCOUNTNAME { get; set; }
        public string C_ACCOUNTTYPE { get; set; }
        public string C_BANKLOCATION { get; set; }
        public string C_BANKADDRESS { get; set; }
        public string C_IFSCCODE { get; set; }
        [DisplayName("Logo")]

        public string C_LOGO { get; set; }

    }

    public class ApiResponseInfo
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<CompanyInfo> Data { get; set; }
        public int Recid { get; set; }
    }

    public class ApiResponseInfos
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public CompanyInfo Data { get; set; }
        public int Recid { get; set; }
    }

}