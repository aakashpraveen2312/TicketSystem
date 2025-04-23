using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Models
{
    public class Projectmaster
    {
        public int P_PROJECTRECID { get; set; }
        public int P_CRECID { get; set; }
        public string P_Product{ get; set; }
        public string P_CODE { get; set; }

        [DisplayName("Customer Name")]
        public string P_NAME { get; set; }

        [DisplayName("Sort")]
        public int P_SORTORDER { get; set; }

        [DisplayName("Disable")]
        public bool IsDisabled
        {
            get => TUG_DISABLE == "Y";
            set => TUG_DISABLE = value ? "Y" : "N";
        }
        [JsonProperty("P_DISABLE")]

        private string TUG_DISABLE { get; set; }
        [DisplayName("Product Name")]

        public string SelectedOption { get; set; }
        public List<SelectListItem> Options { get; set; }
    }

    public class ComboBoxProduct
    {
        public string TPM_CODE { get; set; }
        public string TPM_PRODUCTNAME { get; set; }
    }


    public class ProjectMasterRootObject
    {
        public List<ComboBoxProduct> Product { get; set; }

        public string Message { get; set; }
        public string Status { get; set; }
        public List<Projectmaster> Data { get; set; }
    }
    public class ProjectMasterObjects
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public Projectmaster Data { get; set; }
    }

  
}

