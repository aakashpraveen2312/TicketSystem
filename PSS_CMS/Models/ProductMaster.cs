using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class ProductMaster
    {
        public int P_RECID { get; set; }
        [DisplayName("Grace Period")]
        public int P_GRACEPERIOD { get; set; }
        public int P_CRECID { get; set; }
        [DisplayName("Code")]
        public string P_CODE { get; set; }
        [DisplayName("Product Name")]
        public string P_NAME { get; set; }
        [DisplayName("Sort")]
        public int P_SORTORDER { get; set; }

        [DisplayName("Disable")]
        public bool P_ProductDisable
        {
            get => ProductDisable == "Y";
            set => ProductDisable = value ? "Y" : "N";
        }

        [JsonProperty("P_DISABLE")]
        private string ProductDisable { get; set; }

    }
    public class ProductMasterRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<ProductMaster> Data { get; set; }
    }
    public class ProductMasterRootObjects
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public ProductMaster Data { get; set; }
    }
}