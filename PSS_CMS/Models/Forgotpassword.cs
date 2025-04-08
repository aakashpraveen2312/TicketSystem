using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Forgotpassword
    {
        [DisplayName("User Name")]
        public string Username { get; set; }
        [DisplayName("Email")]
        public string EmailId { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        [DisplayName("New Password")]
        public string newPassword { get; set; }
        [DisplayName("Otp")]
        public string otp { get; set; }

    }
}