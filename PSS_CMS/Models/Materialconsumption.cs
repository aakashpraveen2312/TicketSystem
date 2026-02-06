using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Models
{
    public class Materialconsumption
    {
        public int tM_TCRECID { get; set; }
        public int tM_CRECID { get; set; }
        public int tM_MCRECID { get; set; }
        public int tM_MRECID { get; set; }
        public int tM_RECID { get; set; }
        [JsonIgnore]
        [DisplayName("Billable")]
        public bool tM_BILLABLE
        {
            get => tM_BILLABLED == "Y";
            set => tM_BILLABLED = value ? "Y" : "N";
        }

        [JsonProperty("TM_BILLABLE")]
        private string tM_BILLABLED { get; set; }


        public string tM_DISABLE { get; set; }


        [DisplayName("Sort")]
        public int tM_SORTORDER { get; set; }
        [DisplayName("UOM")]
        public string tM_UOM { get; set; }
        [DisplayName("Price")]
        public decimal tM_PRICE { get; set; }
        [DisplayName("Quantity")]
        public int tM_QUANTITY { get; set; }
        [DisplayName("Discount (%)")]
        public int tM_DISCOUNT { get; set; }
        [DisplayName("Total Amount")]
        public decimal tM_TOTALAMOUNT { get; set; }
        [DisplayName("CGST (%)")]
        public int tM_CGST { get; set; }
        [DisplayName("SGST (%)")]
        public int tM_SGST { get; set; }
        [DisplayName("Net Amount")]

        public decimal tM_NETAMOUNT { get; set; }
        [DisplayName("Item Category")]
        public string MATERIALCATEGORY { get; set; }
        [DisplayName("Item")]
        public string MATERIAL { get; set; }
        [DisplayName("Item Group")]
        public string ITEMGROUP { get; set; }
        [DisplayName("Item Category")]
        public string ITEMCATEGORY { get; set; }
        [DisplayName("Type")]
        public string tM_TYPE { get; set; }
        public string TM_MNAME { get; set; }
        public string TM_MCATDESC { get; set; }


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

      
        public int tM_MGRECID { get; set; }
        public int IG_RECID { get; set; }
        public string IG_DESCRIPTION { get; set; }
        public int IC_RECID { get; set; }
        public string IC_DESCRIPTION { get; set; } 
        public int I_RECID { get; set; }
        public string I_DESCRIPTION { get; set; }
        public List<SelectListItem> ItemGroups { get; set; }
        public List<SelectListItem> ItemCategories { get; set; }
        public List<SelectListItem> Items { get; set; }

        public Materialconsumption()
        {
            ItemGroups = new List<SelectListItem>();
            ItemCategories = new List<SelectListItem>();
            Items = new List<SelectListItem>();
        }

    }
    public class MaterialconsumptionRootObject
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public List<Materialconsumption> Data { get; set; }
    }
    public class MaterialconsumptionObjects
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public Materialconsumption Data { get; set; }
    }
}