using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Materialconsumption
    {
        public int tM_TCRECID { get; set; }
        public int tM_CRECID { get; set; }
        public int tM_MCRECID { get; set; }
        public int tM_MRECID { get; set; }
        public int tM_RECID { get; set; }

        [DisplayName("Billable")]
        public bool tM_BILLABLE
        {
            get => tM_BILLABLED == "Y";
            set => tM_BILLABLED = value ? "Y" : "N";
        }
        [JsonProperty("tM_BILLABLED")]

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
        [DisplayName("Discount")]
        public int tM_DISCOUNT { get; set; }
        [DisplayName("Total Amount")]
        public decimal tM_TOTALAMOUNT { get; set; }
        [DisplayName("CGST")]
        public int tM_CGST { get; set; }
        [DisplayName("SGST")]
        public int tM_SGST { get; set; }
        [DisplayName("Net Amount")]

        public decimal tM_NETAMOUNT { get; set; }
        [DisplayName("Item Category")]
        public string MATERIALCATEGORY { get; set; }
        [DisplayName("Line Item")]
        public string MATERIAL { get; set; }
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
        public string M_TYPE { get; set; }
        public decimal M_QUANTITY { get; set; }
        public decimal M_PRICE { get; set; }
        public decimal M_DISCOUNT { get; set; }
        public decimal M_TOTALAMOUNT { get; set; }
        public decimal M_CGST { get; set; }
        public decimal M_SGST { get; set; }
        public decimal M_NETAMOUNT { get; set; }
        public int M_SORTORDER { get; set; }
        public string M_DISABLE { get; set; }

        public int M_CRECID { get; set; }

        public string SelectedCategory { get; set; }
        public string SelectedMaterial { get; set; }



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