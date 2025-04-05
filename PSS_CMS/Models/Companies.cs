using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Data
    {
        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("API Key")]
        public string APIKey { get; set; }

        [DisplayName("Authorization Key")]
        public string AuthorizationKey { get; set; }
    }
    public class Companies
    {
        public string Password { get; set; }
        public Data Data { get; set; }
        public string SerialNumber { get; set; }


        [DisplayName("App UserName")]
        [Required(ErrorMessage = "Please tell us how we should address you.")]
        public string C_APPUSERNAME { get; set; }

        public string C_RECID { get; set; }


        [Required(ErrorMessage = "* Mandatory")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Code should contain only letters (both uppercase and lowercase) and numerics.")]
        [DisplayName("Code")]
        public string C_CODE { get; set; }


        [Required(ErrorMessage = "* Mandatory")]
        [DisplayName("Company Name")]
        [RegularExpression(@"^[a-zA-Z\s\.]*$", ErrorMessage = "Name should not contain digits")]
        public string C_NAME { get; set; }


        [DisplayName("Address")]
        [Required(ErrorMessage = "* Mandatory")]
        public string C_ADDRESS { get; set; }

        [DisplayName("Country")]
        [Required(ErrorMessage = "* Mandatory")]
        [RegularExpression(@"^[a-zA-Z]+(\s[a-zA-Z]+)*$", ErrorMessage = "Country should not contain digits.")]
        public string C_COUNTRY { get; set; }


        [DisplayName("Pincode")]
        [Required(ErrorMessage = "* Mandatory")]
        [Range(100000, 99999999, ErrorMessage = "Pincode must be a valid 6 to 8 -digit number")]
        public string C_PINCODE { get; set; }


        [Required(ErrorMessage = "* Mandatory")]
        [DisplayName("Phone No")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please enter exactly 10 digits without any special characters or spaces.")]
        public string C_PHONE { get; set; }




        [DisplayName("Web")]
        [Required(ErrorMessage = "* Mandatory")]
        public string C_WEB { get; set; }



        [Required(ErrorMessage = "* Mandatory")]
        [DisplayName("Email")]
        [RegularExpression(@"[a-z]\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Your Email is not valid.")]
        public string C_EMAILID { get; set; }



        [DisplayName("IECode")]
        public string C_IECODE { get; set; }
        [DisplayName("Domain")]
        [Required(ErrorMessage = "* Mandatory")]
        [RegularExpression("^(?=.*[A-Z])(?=.*\\d)[A-Z0-9]*$", ErrorMessage = "Domain name must contain at least one uppercase letter and one numeric character.")]
        public string C_Domain { get; set; }

        [DisplayName("ApiKey")]
        public string C_APIKEY { get; set; }
        [DisplayName("Authorization")]
        public string C_AUTHKEY { get; set; }


        [DisplayName("City")]
        [RegularExpression(@"^[a-zA-Z\s\.]*$", ErrorMessage = "RBICode should be capital letters with Numerics.")]

        public string C_RBICODE { get; set; }



        [RegularExpression(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[0-9A-Z]{1}Z[0-9A-Z]{1}$", ErrorMessage = "GST must follow the valid format (e.g., 22AAAAA0000A1Z5).")]
        [Required(ErrorMessage = "* Mandatory")]
        [StringLength(15, ErrorMessage = "GST must be exactly 15 characters long.")]
        [DisplayName("GST")]
        public string C_GST { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public string C_SORTORDER { get; set; }

        public string C_DISABLE { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string Pincode { get; set; }

        public string Phone { get; set; }
        public string Web { get; set; }

        public string EmailId { get; set; }
        public string IECode { get; set; }
        public string RBICode { get; set; }

        public string GST { get; set; }


        public string Sortorder { get; set; }
        public bool Disable { get; set; }


        //BREADCRUMBS

        public string COMPANYRECID { get; set; }
        public string Status { get; set; }
    }
}