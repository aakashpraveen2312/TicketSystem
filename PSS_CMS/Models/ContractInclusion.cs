using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class ContractInclusion
    {
        public int SerialNumber { get; set; }
        public int CIN_RECID { get; set; }
        public int CIN_PRECID { get; set; }
        public int CIN_CRECID { get; set; }
        [DisplayName("Description")]
        public string CIN_DESCRIPTION { get; set; }
        [DisplayName("Sort")]
        public int CIN_SORTORDER { get; set; }
        [DisplayName("Disable")]
        public bool CIN_InclusionDisable
        {
            get => CInclusionDisable == "Y";
            set => CInclusionDisable = value ? "Y" : "N";
        }

        [JsonProperty("CIN_DISABLE")]
        private string CInclusionDisable { get; set; }
    }
    public class ContractInclusionRootObjects
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<ContractInclusion> Data { get; set; }
    }
    public class ContractInclusionRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public ContractInclusion Data { get; set; }
    }
}