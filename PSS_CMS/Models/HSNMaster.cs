using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class HSNMaster
    {
        public int HM_RECID { get; set; }
        public int SerialNumber { get; set; }


        [DisplayName("HSN Code")]
        public string HM_CODE { get; set; }


        [DisplayName("Description")]
        [Required(ErrorMessage = "* Mandatory")]
        public string HM_DESCRIPTION { get; set; }


        public string HSNCRECID { get; set; }

        [DisplayName("IGST (%)")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal HM_IGST { get; set; }



        [DisplayName("CGST (%)")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "* Mandatory")]
        public decimal HM_CGST { get; set; }

        [DisplayName("SGST (%)")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]

        [Required(ErrorMessage = "* Mandatory")]
        public decimal HM_SGST { get; set; }

        [DisplayName("Sort Order")]

        public int HM_SORTORDER { get; set; }

        [DisplayName("Disable")]
        public bool IsDisabled
        {
            get => HM_DISABLE == "Y";
            set => HM_DISABLE = value ? "Y" : "N";
        }

        // This field directly maps to the JSON property
        [JsonProperty("HM_DISABLE")]

        private string HM_DISABLE { get; set; }

    }
    public class HSNRootObjects
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<HSNMaster> Data { get; set; } // Changed from List<HSNMaster> to a single LineItems object
        public HSNMaster Datas { get; set; } // Changed from List<HSNMaster> to a single LineItems object

    }


}