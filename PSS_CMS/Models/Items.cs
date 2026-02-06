using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Models
{
    public class Items
    {
        [DisplayName("RecID")]
        public int I_RECID { get; set; }

        public int SerialNumber { get; set; }

        [DisplayName("Code")]
        //[Required(ErrorMessage = "* Mandatory")]

        public string I_CODE { get; set; }

        [DisplayName("Description")]
        //[Required(ErrorMessage = "* Mandatory")]

        public string I_DESCRIPTION { get; set; }


        [DisplayName("Price")]
        //[Required(ErrorMessage = "* Mandatory")]

        public decimal I_PRICE { get; set; }

        [DisplayName("Quantity")]
        //[Required(ErrorMessage = "* Mandatory")]
        public int I_BOXQUANTITY { get; set; }

        [DisplayName("Piece Quantity")]
        //[Required(ErrorMessage = "* Mandatory")]
        public int I_PIECEQUANTITY { get; set; }


        [DisplayName("Conversion Quantity")]
        public int I_CONVERSIONQUANTITY { get; set; }

        [DisplayName("HSN CODE")]
        //[Required(ErrorMessage = "* Mandatory")]
        public int I_HSNRECID { get; set; }
        [DisplayName("HSN CODE")]
        public string I_HSNCODE { get; set; }
        [DisplayName("HSN CODE")]

        public string I_HSN { get; set; }

        [DisplayName("SGST(%)")]
        //[Required(ErrorMessage = "* Mandatory")]
        public decimal I_SGST { get; set; }

        [DisplayName("CGST(%)")]
        //[Required(ErrorMessage = "* Mandatory")]
        public decimal I_CGST { get; set; }


        [DisplayName("DateTime")]
        public string I_DATETIME { get; set; }
        [DisplayName("Image")]
        public string I_IMAGE { get; set; }
        [DisplayName("Sort Order")]
        public int I_SORTORDER { get; set; }
        [DisplayName("Company RecID")]
        public string I_CRECID { get; set; }
        [DisplayName("Item Category RecID")]
        public string I_ICRECID { get; set; }
        [DisplayName("Expiry Date")]
        public int I_EXPIRYDATE { get; set; }
        //[DisplayName("If Applicable")]
        //public string I_IFEXPIRYAPPLICABLE { get; set; }
        [DisplayName("Shelves")]
        public int I_SHRECID { get; set; }
        [DisplayName("Bins")]
        public int I_BINRECID { get; set; }
        [DisplayName("Outlet")]
        public int I_SPRECID { get; set; }
        [DisplayName("Purchase UOM")]
        public string I_PUOM { get; set; }
        [DisplayName("UOM")]
        public string I_CUOM { get; set; }

        [DisplayName("Date of Sales")]
        public string I_DOS { get; set; }

        [DisplayName("Sales Invoice No")]
        public string I_SINO { get; set; }

        [DisplayName("Item Name")]
        public string I_ITEMNAME { get; set; }

        [DisplayName("Warrenty Period (In Months)")]
        public string I_TOTALWARRENTYPERIOD { get; set; }

        [DisplayName("Warrenty End Period")]
        public string I_WARRENTYENDPERIOD { get; set; }

        //[DisplayName("Scheduled Servicee ")]
        //public string I_SCHEDULEDSERVICE  { get; set; }

        [DisplayName("Extended Warranty (In Months)")]
        public string I_EXTENDEDWARRENTYPERIOD { get; set; }

        [DisplayName("Extended Warranty (In Months)")]
        public string I_GRACEPERIOD { get; set; }


        [DisplayName("Extended Warranty End (In Months)")]
        public string I_EXTENDEDWARRENTYENDPERIOD { get; set; }

        [DisplayName("Extended Warrenty (In Months)")]
        public string I_SERVICEDESCCRIPTION { get; set; }

        //[DisplayName("Service and Maintance")]
        //public string I_SERVICEANDMAINTANANCE { get; set; }


        [DisplayName("Spec Required")]
        public bool SPECREQUIRED

        {
            get => I_SPECREQUIRED == "Y";
            set => I_SPECREQUIRED = value ? "Y" : "N";
        }


        [JsonProperty("I_SPECREQUIRED")]

        public string I_SPECREQUIRED { get; set; }


        [DisplayName("Design Image Required")]
        public bool DESIGNIMAGEREQUIRED

        {
            get => I_DESIGNIMAGEREQUIRED == "Y";
            set => I_DESIGNIMAGEREQUIRED = value ? "Y" : "N";
        }


        [JsonProperty("I_DESIGNIMAGEREQUIRED")]

        public string I_DESIGNIMAGEREQUIRED { get; set; }

        [DisplayName("Expiry Applicable")]
        public bool EXPIRYAPPLICABLE

        {
            get => I_EXPIRYAPPLICABLE == "Y";
            set => I_EXPIRYAPPLICABLE = value ? "Y" : "N";
        }


        [JsonProperty("I_IFEXPIRYAPPLICABLE")]

        public string I_EXPIRYAPPLICABLE { get; set; }




        [DisplayName("Service and Maintance")]
        public bool ServiceAndMaintance

        {
            get => I_SERVICEANDMAINTANANCE == "Y";
            set => I_SERVICEANDMAINTANANCE = value ? "Y" : "N";
        }

        [JsonProperty("I_SERVICEANDMAINTANANCE")]
        public string I_SERVICEANDMAINTANANCE { get; set; }

        [DisplayName("On Demand Basis")]
        public bool ONDEMAND

        {
            get => I_ONDEMAND == "Y";
            set => I_ONDEMAND = value ? "Y" : "N";
        }

        [JsonProperty("I_ONDEMAND")]
        public string I_ONDEMAND { get; set; }




        [DisplayName("Extended Warrenty Applicable")]
        public bool EXTENDEDWARRENTYAPPLICABLE

        {
            get => I_EXTENDEDWARRENTYAPPLICABLE == "Y";
            set => I_EXTENDEDWARRENTYAPPLICABLE = value ? "Y" : "N";
        }

        [JsonProperty("I_EXTENDEDWARRENTYAPPLICABLE")]
        public string I_EXTENDEDWARRENTYAPPLICABLE { get; set; }




        [DisplayName("On Schedule")]
        public bool SCHEDULEDSERVICE

        {
            get => I_SCHEDULEDSERVICE == "Y";
            set => I_SCHEDULEDSERVICE = value ? "Y" : "N";
        }

        [JsonProperty("I_SCHEDULEDSERVICE")]
        public string I_SCHEDULEDSERVICE { get; set; }





        [DisplayName("Disable")]
        public bool IsDisabled
        {
            get => _I_Disable == "Y";
            set => _I_Disable = value ? "Y" : "N";
        }
        // This field directly maps to the JSON property
        [JsonProperty("I_Disable")]

        public string _I_Disable { get; set; }


        [DisplayName("Tradable")]
        public bool TRADABLE

        {
            get => I_TRADABLE == "Y";
            set => I_TRADABLE = value ? "Y" : "N";
        }


        [JsonProperty("I_TRADABLE")]

        public string I_TRADABLE { get; set; }


        [DisplayName("Under Employee Custody")]
        public bool UNDER_EMPLOYEE_CUSTODY

        {
            get => I_UNDEREMPLOYEECUSTODY == "Y";
            set => I_UNDEREMPLOYEECUSTODY = value ? "Y" : "N";
        }


        [JsonProperty("I_UNDEREMPLOYEECUSTODY")]

        public string I_UNDEREMPLOYEECUSTODY { get; set; }

        [DisplayName("By Product [Job Work Component]")]
        public bool BYPRODUCT

        {
            get => I_BYPRODUCT == "Y";
            set => I_BYPRODUCT = value ? "Y" : "N";
        }


        [JsonProperty("I_BYPRODUCT")]

        public string I_BYPRODUCT { get; set; }

        public string I_VENDORMAJORUOM { get; set; }
        public int I_VENDORCONVERSIONQUANTITY { get; set; }
        public int I_VENDORMAJORQUANTITY { get; set; }
        public string I_VENDORMINORUOM { get; set; }
        public decimal I_VENDORPRICE { get; set; }
        public int I_VENDORMINORQUANTITY { get; set; }

        public string I_CUSTOMERMAJORUOM { get; set; }
        public int I_CUSTOMERCONVERSIONQUANTITY { get; set; }
        public int I_CUSTOMERMAJORQUANTITY { get; set; }
        public int I_CUSTOMERMINORQUANTITY { get; set; }
        public string I_CUSTOMERMINORUOM { get; set; }
        public decimal I_CUSTOMERPRICE { get; set; }


        //SERVICE MANAGEMENT
        [DisplayName("Quantity")]
        public decimal I_SMQUANTITY { get; set; }
        [DisplayName("Net Amount")]
        public decimal I_SMNETAMOUNT { get; set; }
        [DisplayName("Total Amount")]
        public decimal I_SMTOTALAMOUNT { get; set; }
        [DisplayName("Discount")]
        public int I_SMDISCOUNT { get; set; }

        



        public List<SelectListItem> Outlets { get; set; }
        public List<SelectListItem> Bins { get; set; }
        public List<SelectListItem> Shelves { get; set; }
        public List<anlyticsComboBoxItem> Users { get; set; }

    }

    public class ComboBoxItem
    {
        public int ComboId { get; set; }
        public string ComboName { get; set; }
    }
    public class anlyticsComboBoxItem
    {
        public string RECORDID { get; set; }
        public string NAME { get; set; }
        public string COMPANYID { get; set; }

    }

    public class ApiResponses
    {
        public string status { get; set; }
        public List<ComboBoxItem> Outlet { get; set; }
        public List<anlyticsComboBoxItem> Users { get; set; }

        public List<ComboBoxItem> Bins { get; set; }
        //[JsonConverter(typeof(Shelves))]
        public List<ComboBoxItem> Shelves { get; set; }
        public string message { get; set; }

    }


    public class IRootObjects
    {
        public string Status { get; set; }
        public List<Items> Data { get; set; }

        public string Message { get; set; }
        public string ItemCode { get; set; }
    }
    public class ICreateRootObjects
    {
        public string Status { get; set; }
        public Items Data { get; set; }

        public string Message { get; set; }
    }

}