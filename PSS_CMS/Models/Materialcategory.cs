using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Materialcategory
    {
        public int MC_RECID { get; set; }
        public int MC_CRECID { get; set; }
        [DisplayName("Sort")]
        public int MC_SORTORDER { get; set; }
        [DisplayName("Code")]
        public string MC_CODE { get; set; }
        [DisplayName("Description")]
        public string MC_DESCRIPTION { get; set; }
        public string MC_DATETIME { get; set; }

        [DisplayName("Disable")]
        public bool IsDisabled
        {
            get => MC_DISABLE == "Y";
            set => MC_DISABLE = value ? "Y" : "N";
        }
        [JsonProperty("MC_DISABLED")]

        private string MC_DISABLE { get; set; }

        public string SelectedCategory { get; set; }
    }
    public class MaterialcategoryRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<Materialcategory> Data { get; set; }
    }
    public class MaterialcategorypObjects
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public Materialcategory Data { get; set; }
    }
}