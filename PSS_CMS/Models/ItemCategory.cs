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
    public class ItemCategory
    {
        [DisplayName("RecID")]
        public int IC_RECID { get; set; }
        public int SerialNumber { get; set; }


        [Required(ErrorMessage = "* Mandatory")]
        [DisplayName("Code")]
        [StringLength(7, MinimumLength = 4, ErrorMessage = "Code must be between 4 to 7 characters.")]
        [RegularExpression(@"^[A-Za-z1-9][A-Za-z0-9]{3,6}$")]
        public string IC_CODE { get; set; }
        [DisplayName("HSN Code")]

        public string IC_HSNCODE { get; set; }


        [DisplayName("Description")]
        [Required(ErrorMessage = "* Mandatory")]
        public string IC_DESCRIPTION { get; set; }


        [DisplayName("DateTime")]
        public string IC_DATETIME { get; set; }


        [DisplayName("Sort Order")]
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int IC_SORTORDER { get; set; }


        [DisplayName("ItemGroup RecID")]
        public int IC_IGRECID { get; set; }


        [DisplayName("Company RecID")]
        public int IC_CRECID { get; set; }


        [DisplayName("Image")]
        public string IC_IMAGE { get; set; }


        [DisplayName("Disable")]
        public bool IsDisabled
        {
            get => _IC_Disable == "Y";
            set => _IC_Disable = value ? "Y" : "N";
        }
        // This field directly maps to the JSON property
        [JsonProperty("IC_Disable")]

        private string _IC_Disable { get; set; }
        public List<SelectListItem> EnumType { get; set; }

    }

    public class ItemconetntObject
    {
        public string Status { get; set; }

        public List<ItemCategory> Data { get; set; }
        public ItemCategory Datas { get; set; }

        public string Message { get; set; }
        public string IC_IGRECID { get; set; }
    }

    public class ItemidObject
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<ItemCategory> Datas { get; set; }

        public ItemCategory Data { get; set; }
    }

}