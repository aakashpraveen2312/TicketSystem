using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Models
{
    public class ServiceMaterialConsumption
    {
        public int sMT_CTRECID { get; set; }
        public int sMT_CRECID { get; set; }
        public int sMT_MCRECID { get; set; }
        public int sMT_MRECID { get; set; }
        public int sMT_RECID { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        [DisplayName("Billable")]
        public bool sMT_BILLABLE
        {
            get => sMT_BILLABLED == "Y";
            set => sMT_BILLABLED = value ? "Y" : "N";
        }

        [JsonProperty("SMT_BILLABLE")]
        private string sMT_BILLABLED { get; set; }


        public string sMT_DISABLE { get; set; }


        [DisplayName("Sort")]
        public int sMT_SORTORDER { get; set; }
        [DisplayName("UOM")]
        public string sMT_UOM { get; set; }
        
        [DisplayName("Price")]
        public decimal sMT_PRICE { get; set; }
        [DisplayName("Quantity")]
        public int sMT_QUANTITY { get; set; }
        [DisplayName("Discount (%)")]
        public int sMT_DISCOUNT { get; set; }
        [DisplayName("Total Amount")]
        public decimal sMT_TOTALAMOUNT { get; set; }
        [DisplayName("CGST (%)")]
        public int sMT_CGST { get; set; }
        [DisplayName("SGST (%)")]
        public int sMT_SGST { get; set; }
        [DisplayName("Net Amount")]

        public decimal sMT_NETAMOUNT { get; set; }
        [DisplayName("Item Category")]
        public string MATERIALCATEGORY { get; set; }
        [DisplayName("Item")]
        public string MATERIAL { get; set; }
        [DisplayName("Item Group")]
        public string ITEMGROUP { get; set; }
        [DisplayName("Item Category")]
        public string ITEMCATEGORY { get; set; }
        [DisplayName("Type")]
        public string sMT_TYPE { get; set; }
        public string SMT_MNAME { get; set; }
        public string SMT_MCATDESC { get; set; }

        [DisplayName("Service Charges")]

        public decimal sMT_SERVICECHARGES { get; set; }

        public int M_RECID { get; set; }
        public int MC_RECID { get; set; }
        public string M_CODE { get; set; }
        public string M_NAME { get; set; }
        public string MC_DESCRIPTION { get; set; }
        public int M_MCRECID { get; set; }
        public string M_UOM { get; set; }
        public string I_CUOM { get; set; }
        public string M_TYPE { get; set; }
        public decimal M_QUANTITY { get; set; }
        public decimal I_SMQUANTITY { get; set; }
        public decimal M_PRICE { get; set; }
        public decimal I_PRICE { get; set; }
        public decimal M_DISCOUNT { get; set; }
        public decimal I_SMDISCOUNT { get; set; }
        public decimal M_TOTALAMOUNT { get; set; }
        public decimal I_SMTOTALAMOUNT { get; set; }
        public decimal M_CGST { get; set; }
        public decimal I_CGST { get; set; }
        public decimal M_SGST { get; set; }
        public decimal I_SGST { get; set; }
        public decimal M_NETAMOUNT { get; set; }
        public decimal I_SMNETAMOUNT { get; set; }
        public int M_SORTORDER { get; set; }
        public int I_SORTORDER { get; set; }
        public string M_DISABLE { get; set; }

        public int M_CRECID { get; set; }

        public string SelectedCategory { get; set; }
        public string SelectedMaterial { get; set; }
        public string SelectedItemGroup { get; set; }
        public string SelectedItemCategory { get; set; }


        public int sMT_MGRECID { get; set; }
        public int IG_RECID { get; set; }
        public string IG_DESCRIPTION { get; set; }
        public int IC_RECID { get; set; }
        public string IC_DESCRIPTION { get; set; }
        public int I_RECID { get; set; }
        public string I_DESCRIPTION { get; set; }
        public List<SelectListItem> ItemGroups { get; set; }
        public List<SelectListItem> ItemCategories { get; set; }
        public List<SelectListItem> Items { get; set; }

        public ServiceMaterialConsumption()
        {
            ItemGroups = new List<SelectListItem>();
            ItemCategories = new List<SelectListItem>();
            Items = new List<SelectListItem>();
        }

    }
    public class ServiceMaterialConsumptionRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<ServiceMaterialConsumption> Data { get; set; }
    }
    public class ServiceMaterialConsumptionObjects
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public ServiceMaterialConsumption Data { get; set; }
    }
}
