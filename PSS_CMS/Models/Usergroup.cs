using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Usergroup
    {
        public int UG_RECID { get; set; }
        [DisplayName("Code")]
        public string UG_CODE { get; set; }
        [DisplayName("Name")]
        public string UG_NAME { get; set; }
        [DisplayName("Desigination")]
        public string UG_DESIGINATION { get; set; }
        [DisplayName("Sort")]
        public int UG_SORTORDER { get; set; }
        [DisplayName("Disable")]
        public bool IsDisabled
        {
            get => _ug_disable == "Y";
            set => _ug_disable = value ? "Y" : "N";
        }


        public string Disable { get; set; }

        // This field directly maps to the JSON property
        [JsonProperty("UG_DISABLE")]
        private string _ug_disable { get; set; }
    }
}