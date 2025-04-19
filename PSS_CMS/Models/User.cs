using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class User
    {
        public int? L_RECID { get; set; }
        public string selectedvalue { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username can't be longer than 50 characters")]
        [DisplayName("Name")]
        public string L_USERNAME { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters long")]
        [DisplayName("Password")]
        public string L_PASSWORD { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [DisplayName("Role")]
        public string L_ROLE { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.(com)$", ErrorMessage = "Email must be a valid .com address")]

        [Required(ErrorMessage = "Email ID is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [DisplayName("Email ID")]
        public string L_EMAILID { get; set; }

        public int? L_COMPANYID { get; set; }

        [Required(ErrorMessage = "User Code is required")]
        [StringLength(20, ErrorMessage = "Code can't be longer than 20 characters")]
        [DisplayName("Code")]
        public string L_USERID { get; set; }

        [StringLength(50, ErrorMessage = "Project name can't be longer than 50 characters")]
        [DisplayName("Project")]
        public string L_PROJECTID { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [DisplayName("Mobile Number")]
        public string l_MOBILENO { get; set; }
        public string L_Contact { get; set; }

        [DisplayName("User Group")]
        public string L_UGRECID { get; set; }

        [StringLength(100, ErrorMessage = "Domain can't be longer than 100 characters")]
        [DisplayName("Domain")]
        public string L_DOMAIN { get; set; }

        [StringLength(10, ErrorMessage = "SortOrder can't be longer than 10 characters")]
        [DisplayName("Sort")]
        public string L_SORTORDER { get; set; }

        [DisplayName("Disable")]
        public bool L_UserDisable
        {
            get => UserDisable == "Y";
            set => UserDisable = value ? "Y" : "N";
        }

        [JsonProperty("L_DISABLE")]
        private string UserDisable { get; set; }

        public List<SelectListItem> Outlets { get; set; }
        public List<SelectListItem> Projects { get; set; }
    }

    public class ComboBoxItem
    {
        public string TUG_CODE { get; set; }
        public string TUG_NAME { get; set; }
    }

    public class ComboBoxProjectItem
    {
        public int P_CODE { get; set; }
        public string P_NAME { get; set; }
    }

    public class ApiResponseUser
    {
        public List<ComboBoxItem> Outlet { get; set; }
        public List<ComboBoxProjectItem> Project { get; set; }

        public string Status { get; set; }
        public string Message { get; set; }
        public List<User> Data { get; set; }
        public User Datas { get; set; }
    }
    public class ApiResponseUserS
    {
        
        public string Status { get; set; }
        public string Message { get; set; }
        public User Data { get; set; }
    }
}