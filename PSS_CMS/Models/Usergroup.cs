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
        public int R_RECID { get; set; }
        public int R_CRECID { get; set; }
        public string R_CODE { get; set; }

        [DisplayName("Name")]
        public string R_NAME { get; set; }

        [DisplayName("Description")]
        public string R_ROLEDESCRIPTION { get; set; }

        [DisplayName("Sort")]
        public int R_SORTORDER { get; set; }

        [DisplayName("Disable")]
        public bool IsDisabled
        {
            get => R_DISABLE == "Y";
            set => R_DISABLE = value ? "Y" : "N";
        }
        [JsonProperty("QLI_DISABLE")]

        private string R_DISABLE { get; set; }
 
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