using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Usergroup
    {
        public int TUG_RECID { get; set; }
        public int TUG_CRECID { get; set; }
        [Required(ErrorMessage = "Code is required.")]
        [DisplayName("Code")]
        [RegularExpression(@"^[A-Za-z]+[0-9]{4}$", ErrorMessage = "Code must start with letters and have exactly 4 digits.")]
        public string TUG_CODE { get; set; }



        [DisplayName("Name")]
        public string TUG_NAME { get; set; }

        [DisplayName("Desigination")]
        public string TUG_ROLEDESCRIPTION { get; set; }

        [DisplayName("Sort")]
        public int TUG_SORTORDER { get; set; }

        [DisplayName("Disable")]
        public bool IsDisabled
        {
            get => TUG_DISABLE == "Y";
            set => TUG_DISABLE = value ? "Y" : "N";
        }
        [JsonProperty("QLI_DISABLE")]

        private string TUG_DISABLE { get; set; }
    }
    public class UserGroupRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<Usergroup> Data { get; set; }
    }
    public class UserGroupObjects
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public Usergroup Data { get; set; }
    }
}