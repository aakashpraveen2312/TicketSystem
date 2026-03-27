using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class ContractExclusion
    {
        public int SerialNumber { get; set; }
        public int CEX_RECID { get; set; }
        public int CEX_PRECID { get; set; }
        public int CEX_CRECID { get; set; }
        [DisplayName("Description")]
        public string CEX_DESCRIPTION { get; set; }
        [DisplayName("Sort")]
        public int CEX_SORTORDER { get; set; }
        [DisplayName("Disable")]
        public bool CEX_ExclusionDisable
        {
            get => CExclusionDisable == "Y";
            set => CExclusionDisable = value ? "Y" : "N";
        }

        [JsonProperty("CEX_DISABLE")]
        private string CExclusionDisable { get; set; }
    }
    public class ContractExclusionRootObjects
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<ContractExclusion> Data { get; set; }
    }
    public class ContractExclusionRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public ContractExclusion Data { get; set; }
    }
}