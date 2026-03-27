using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Inclusion
    {
        public int SerialNumber { get; set; }
        public int IN_RECID { get; set; }
        public int IN_PRECID { get;set;} 
        public int IN_CRECID { get; set; }
        [DisplayName("Description")]
        public string IN_DESCRIPTION { get; set; }
        [DisplayName("Sort")]
        public int IN_SORTORDER { get; set; }
        [DisplayName("Disable")]
        public bool IN_InclusionDisable
        {
            get => InclusionDisable == "Y";
            set => InclusionDisable = value ? "Y" : "N";
        }

        [JsonProperty("IN_DISABLE")]
        private string InclusionDisable { get; set; }
        
    }
    public class InclusionRootObjects
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<Inclusion> Data { get; set; }
    }
    public class InclusionRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public Inclusion Data { get; set; }
    }
}