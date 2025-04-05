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
        public int TUG_RECID { get; set; }
        public int TUG_CRECID { get; set; }

        [DisplayName("Code")]
        public string TUG_CODE { get; set; }

        [DisplayName("Name")]
        public string TUG_NAME { get; set; }

        [DisplayName("Desigination")]
        public string TUG_ROLEDESIGINATION { get; set; }

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
}