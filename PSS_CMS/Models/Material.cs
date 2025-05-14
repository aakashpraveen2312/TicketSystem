using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Material
    {
        public int M_RECID { get; set; }
        public int M_CRECID { get; set; }
        [DisplayName("Sort")]
        public int M_SORTORDER { get; set; }
        public int M_MCRECID { get; set; }
        [DisplayName("Quantity")]
        public int M_QUANTITY { get; set; }
        [DisplayName("Discount (%)")]
        public int M_DISCOUNT { get; set; }
        [DisplayName("CGST (%)")]
        public int M_CGST { get; set; }
        [DisplayName("SGST(%)")]
        public int M_SGST { get; set; }
        [DisplayName("Price")]
        public decimal M_PRICE { get; set; }
        [DisplayName("Total Amount")]
        public decimal M_TOTALAMOUNT { get; set; }
        [DisplayName("Net Amount")]
        public decimal M_NETAMOUNT { get; set; }
        [DisplayName("Code")]
        public string M_CODE { get; set; }
        [DisplayName("Description")]
        public string M_NAME { get; set; }
        [DisplayName("UOM")]
        public string M_UOM { get; set; }
        [DisplayName("Type")]
        public string M_TYPE { get; set; }

        [DisplayName("Disable")]
        public bool IsDisabled
        {
            get => M_DISABLE == "Y";
            set => M_DISABLE = value ? "Y" : "N";
        }
        [JsonProperty("M_DISABLED")]

        private string M_DISABLE { get; set; }

        public string SelectedMaterial { get; set; }
    }
    public class MaterialRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<Material> Data { get; set; }
    }
    public class MaterialpObjects
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public Material Data { get; set; }
    }
}