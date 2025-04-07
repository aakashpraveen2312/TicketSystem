using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Models
{
    public class User
    {
        public int? L_RECID { get; set; }
        [DisplayName("Name")]
        public string L_USERNAME { get; set; }
        [DisplayName("Password")]
        public string L_PASSWORD { get; set; }
        [DisplayName("Role")]
        public string L_ROLE { get; set; }
        [DisplayName("EmailID")]
        public string L_EMAILID { get; set; }
        public int? L_COMPANYID { get; set; }
        [DisplayName("Code")]
        public string L_USERID { get; set; }
        [DisplayName("Project")]
        public string L_PROJECTID { get; set; }
        [DisplayName("Mobile Number")]
        public int? L_Contact { get; set; }
        [DisplayName("User Group")]

        public int? L_UGRECID { get; set; }
        [DisplayName("Domain")]
        public string L_DOMAIN { get; set; }
        [DisplayName("SortOrder")]
        public int? L_SORTORDER { get; set; }



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
        public int TUG_CODE { get; set; }
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
        public List<User> DataS { get; set; }
        public User Data { get; set; }
    }
}