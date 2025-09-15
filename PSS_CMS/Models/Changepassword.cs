using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Changepassword
    {
        public string U_PWD { get; set; }
       
        [Required(ErrorMessage = "Please enter the New Password")]
        public string U_Changepassword { get; set; }
       
        [Required(ErrorMessage = "Please enter the Confirm Password")]
        public string U_NewPassword { get; set; }
 
    }
}