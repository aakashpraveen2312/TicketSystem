using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Exclusion
    {
        public int SerialNumber { get; set; }
        public int EX_RECID { get; set; }
        public int EX_PRECID { get; set; }
        public int EX_CRECID { get; set; }
        [DisplayName("Description")]
        public string EX_DESCRIPTION { get; set; }
        [DisplayName("Sort")]
        public int EX_SORTORDER { get; set; }
        [DisplayName("Disable")]
        public bool EX_ExclusionDisable
        {
            get => ExclusionDisable == "Y";
            set => ExclusionDisable = value ? "Y" : "N";
        }

        [JsonProperty("EX_DISABLE")]
        private string ExclusionDisable { get; set; }
    }
    public class ExclusionRootObjects
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<Exclusion> Data { get; set; }
    }
    public class ExclusionRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public Exclusion Data { get; set; }
    }
}