using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class ItemGroup
    {


        [DisplayName("RecID")]
        public int IG_RECID { get; set; }
        public int SerialNumber { get; set; }


        [Required(ErrorMessage = "* Mandatory")]
        [DisplayName("Code")]
        [StringLength(7, MinimumLength = 4, ErrorMessage = "Code must be between 4 to 7 characters.")]
        [RegularExpression(@"^[A-Za-z1-9][A-Za-z0-9]{3,6}$")]

        public string IG_CODE { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "* Mandatory")]

        public string IG_DESCRIPTION { get; set; }

        [DisplayName("Sort Order")]

        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int IG_SORTORDER { get; set; }

        [DisplayName("Company Recid")]
        public string IG_CRECID { get; set; }

        [DisplayName("DateTime")]
        public string IG_DATETIME { get; set; }

        [DisplayName("Disable")]
        public bool IsDisabled
        {
            get => IG_DISABLE == "Y";
            set => IG_DISABLE = value ? "Y" : "N";
        }

        // This field directly maps to the JSON property
        [JsonProperty("IG_DISABLE")]

        private string IG_DISABLE { get; set; }


        [DisplayName("Purchase Items")]
        public bool PurchaseDisabled
        {
            get => IG_PURCHASE == "Y";
            set => IG_PURCHASE = value ? "Y" : "N";
        }

        // This field directly maps to the JSON property
        [JsonProperty("IG_PURCHASEFLAG")]

        private string IG_PURCHASE { get; set; }

        [DisplayName("Jobworkin Items")]
        public bool JobworkinDisabled
        {
            get => IG_JOBWORKIN == "Y";
            set => IG_JOBWORKIN = value ? "Y" : "N";
        }

        // This field directly maps to the JSON property
        [JsonProperty("IG_JOBWORKINFLAG")]

        private string IG_JOBWORKIN { get; set; }

        [DisplayName("Jobworkout Items")]
        public bool JobworkoutDisabled
        {
            get => IG_JOBWORKOUT == "Y";
            set => IG_JOBWORKOUT = value ? "Y" : "N";
        }

        // This field directly maps to the JSON property
        [JsonProperty("IG_JOBWORKOUTFLAG")]

        private string IG_JOBWORKOUT { get; set; }



        [DisplayName("Sales Items")]
        public bool SalesDisabled
        {
            get => IG_SALES == "Y";
            set => IG_SALES = value ? "Y" : "N";
        }

        // This field directly maps to the JSON property
        [JsonProperty("IG_SALESFLAG")]

        private string IG_SALES { get; set; }
    }

    public class IGRootObjects
    {
        public string Status { get; set; }
        public List<ItemGroup> Data { get; set; }

        public string Message { get; set; }
    }

}