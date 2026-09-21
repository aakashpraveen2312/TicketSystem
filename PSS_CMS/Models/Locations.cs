using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Locations
    {
        public int L_RECID { get; set; }
        [DisplayName("Code")]
        public string L_CODE { get; set; }
        public int L_CRECID { get; set; }
        [DisplayName("Sort Order")]
        public int L_SORTORDER { get; set; }
        [DisplayName("Location Name")]
        public string  L_NAME { get; set; }
        public string  L_TYPE { get; set; }
        public string  L_DISABLE { get; set; }
        public string  L_DATETIME { get; set; }
        public bool Disable { get; set; }
    
        [DisplayName("Location Type")]
        [Required(ErrorMessage = "Location Type is required")]

        public int LT_RECID { get; set; }

        public string LT_NAME { get; set; }
        public int? LT_SORTORDER { get; set; }
        public string LT_DISABLE { get; set; }
        public string LocationTypeName { get; set; }
        public int LT_CRECID { get; set; }

        [DisplayName("Partner Name")]
        [Required(ErrorMessage = "Partner Name is required")]
        public string L_PartnerName { get; set; }



        [DisplayName("Email ID")]
        [Required(ErrorMessage = "Email ID is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Enter a valid email format (e.g., name@example.com)")]
        public string L_EmailID { get; set; }


        [DisplayName("Mobile Number")]
        [Required(ErrorMessage = "Mobile Number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile Number must be exactly 10 digits")]
        public string L_ContactNumber { get; set; }


        [DisplayName("GST Number")]
        public string L_GSTNumber { get; set; }
    }

    public class LocationsObjects
    {
        public List<Locations> Data { get; set; }
    }
  
}