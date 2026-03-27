using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class Party
    {
        public bool IsSelecteds { get; set; }
        public string P_TYPE { get; set; }
        public int SerialNumber { get; set; }

        [DisplayName("Sort")]
        public int P_SORTORDER { get; set; }

        public int P_RECID { get; set; }



        [DisplayName("Code")]
        public string P_CODE { get; set; }

        public string P_NAME { get; set; }

        public string P_ADDRESS1 { get; set; }


        public string P_POSTALCODE { get; set; }

        public string P_MOBILE { get; set; }

        public string P_ALTERNATEMOBILE { get; set; }


        public string P_TELEPHONE { get; set; }

        public string P_EMAIL { get; set; }

        public string P_ALTERNATEMAIL { get; set; }


        public string P_GSTNUMBER { get; set; }

        public string P_GSTATTACHMENT { get; set; }

        public string P_IGSTNUMBER { get; set; }

        public string P_IGSTATTACHMENT { get; set; }

        public string P_PAN { get; set; }

        public string P_PANATTACHMENT { get; set; }


        public string P_FAXNUMBER { get; set; }


        public string P_CONTACTPERSON1 { get; set; }

  
        [DisplayName("Mobile Number")]

        public string P_CONTACTPERSON1MOBILE { get; set; }


 
        public string P_CONTACTPERSON2 { get; set; }

        [DisplayName("Mobile Number")]

        public string P_CONTACTPERSON2MOBILE { get; set; }

        public string P_CUSTOMERAPPLICABLE { get; set; }

        public string P_SUPPLIERAPPLICABLE { get; set; }

        public string P_CONTACTPERSON1EMAIL { get; set; }

        public string P_CONTACTPERSON2EMAIL { get; set; }


        public string P_TINNUMBER { get; set; }

        public string P_TINATTACHMENT { get; set; }


        public string P_AUTHSIGNNAME { get; set; }


        public string P_AUTHPOSITION { get; set; }
        public string P_AUTHSIGNATTACHMENT { get; set; }
        public string P_HEADERIMGATTACHMENT { get; set; }
        public string P_FOOTERIMGATTACHMENT { get; set; }



        [DisplayName("Customer")]
        public bool IscustomerDisabled
        {
            get => _C_disable == "Y";
            set => _C_disable = value ? "Y" : "N";
        }

        // This field directly maps to the JSON property
        [JsonProperty("P_FLGCUSTOMER")]

        private string _C_disable { get; set; }



        [DisplayName("Supplier")]
        public bool IsSupplierDisabled
        {
            get => _S_disable == "Y";
            set => _S_disable = value ? "Y" : "N";
        }

        // This field directly maps to the JSON property
        [JsonProperty("P_FLGSUPPLIER")]

        private string _S_disable { get; set; }



        [DisplayName("Vendor")]
        public bool IsVendorDisabled
        {
            get => _V_disable == "Y";
            set => _V_disable = value ? "Y" : "N";
        }

  
        [JsonProperty("P_FLGVENDOR")]

        private string _V_disable { get; set; }


        [DisplayName("Distributor")]
        public bool IsDistributorDisabled
        {
            get => _D_disable == "Y";
            set => _D_disable = value ? "Y" : "N";
        }


        [JsonProperty("P_FLGDISTRIBUTOR")]

        private string _D_disable { get; set; }


        [DisplayName("Verify Whatsapp")]
        public bool IsVERIFIEDWHATSAPPDisabled
        {
            get => p_VERIFIEDWHATSAPP == "Y";
            set => p_VERIFIEDWHATSAPP = value ? "Y" : "N";
        }

   
        [JsonProperty("P_VERIFIEDWHATSAPP")]

        private string p_VERIFIEDWHATSAPP { get; set; }

        [DisplayName("Verify Email")]
        public bool IsVERIFIEDEMAILDisabled
        {
            get => p_VERIFIEDEMAIL == "Y";
            set => p_VERIFIEDEMAIL = value ? "Y" : "N";
        }

     
        [JsonProperty("P_VERIFIEDEMAIL")]

        private string p_VERIFIEDEMAIL { get; set; }


        [DisplayName("Employee")]
        public bool IsEmployeeDisabled
        {
            get => _E_disable == "Y";
            set => _E_disable = value ? "Y" : "N";
        }

        [JsonProperty("P_FLGEMPLOYEE")]

        private string _E_disable { get; set; }


        [DisplayName("Disable")]
        public bool IsDisabled
        {
            get => P_DISABLE == "Y";
            set => P_DISABLE = value ? "Y" : "N";
        }


        public string P_DISABLE { get; set; }

        public int P_CORECID { get; set; }


        public string P_BANKNAME { get; set; }


        public string P_BRANCHNAME { get; set; }


        public string P_ACCOUNTNAME { get; set; }


        
        public string P_ACCOUNTNO { get; set; }

       
        public string P_ACCOUNTTYPE { get; set; }


        
      
        public string P_IFSCCODE { get; set; }



      
        public string P_BANKLOCATION { get; set; }


       
        public string P_BANKADDRESS { get; set; }

        //Flag Values
        public bool flgCustomer { get; set; }
        public bool flgSupplier { get; set; }
        public bool flgVendor { get; set; }
        public bool flgDistributor { get; set; }
        public bool flgEmployee { get; set; }
        public bool flgDisable { get; set; }



        public bool PIGSTEmail
        {
            get => P_IGST == "Y";
            set => P_IGST = value ? "Y" : "N";
        }

        // This field directly maps to the JSON property
        [JsonProperty("P_IGST")]

        public string P_IGST { get; set; }


        public bool PartyVerify
        {
            get => P_Verify == "Y";
            set => P_Verify = value ? "Y" : "N";
        }

        // This field directly maps to the JSON property
        [JsonProperty("P_Verify")]

        public string P_Verify { get; set; }

    }
    public class PartyObjects
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<Party> Data { get; set; }
        public int Recid { get; set; }
    } 
    public class PartyObject
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public Party Data { get; set; }
        public int Recid { get; set; }
    }
}