using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PSS_CMS.Models
{

    public class User
    {
        public int? U_RECID { get; set; }

       
        [StringLength(50, ErrorMessage = "Username can't be longer than 50 characters")]
        [DisplayName("Name")]
        public string U_USERNAME { get; set; }

       
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Password must be between 4 and 100 characters long")]
        [DisplayName("Password")]
        public string U_PASSWORD { get; set; }

       
        [DisplayName("Role")]
        public string U_RCODE { get; set; }

      
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.(com)$", ErrorMessage = "Email must be a valid .com address")]
        [DisplayName("Email ID")]
        public string U_EMAILID { get; set; }

        public int? U_CRECID { get; set; }

      
        [StringLength(10, ErrorMessage = "Code can't be longer than 10 characters")]
        [DisplayName("Code")]
        public string U_USERCODE { get; set; }

       
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be exactly 10 digits")]
        [DisplayName("Mobile Number")]
        public string U_MOBILENO { get; set; }
        public string U_DOMAIN { get; set; }

       
        [StringLength(10, ErrorMessage = "SortOrder can't be longer than 10 characters")]
        [DisplayName("Sort")]
        public string U_SORTORDER { get; set; }

        [DisplayName("Disable")]
        public bool U_UserDisable
        {
            get => UserDisable == "Y";
            set => UserDisable = value ? "Y" : "N";
        }

        [JsonProperty("U_DISABLE")]
        private string UserDisable { get; set; }

        public string SelectedRole { get; set; }
        public string R_NAME { get; set; }
        public string R_CODE { get; set; }
    }

    public class ApiResponseUserObject
    {
        
        public string Status { get; set; }
        public string Message { get; set; }
        public User Data { get; set; }
    }
    public class ApiResponseUserObjects
    {

        public string Status { get; set; }
        public string Message { get; set; }
        public List<User> Data { get; set; }
    }
}