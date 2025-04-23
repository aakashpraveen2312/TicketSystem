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
        public string TPM_RECID { get; set; }
        public int TPM_CRECID { get; set; }
        public string TPM_CODE { get; set; }
        [DisplayName("Product Name")]
        public string TPM_PRODUCTNAME { get; set; }
        [DisplayName("Sort")]
        public int TPM_SORTORDER { get; set; }
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