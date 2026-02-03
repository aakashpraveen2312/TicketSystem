using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class HSNCategory
    {

        public int SerialNumber { get; set; }
        public int HSNC_RECID { get; set; }
        public string HSNC_CRECID { get; set; }


        [DisplayName("Category Code")]
        [Required(ErrorMessage = "* Mandatory")]
        [StringLength(7, MinimumLength = 4, ErrorMessage = "Code must be between 4 to 7 characters.")]
        [RegularExpression(@"^[A-Za-z1-9][A-Za-z0-9]{3,6}$")]
        public string HSNC_CODE { get; set; }


        [DisplayName("Category")]
        [Required(ErrorMessage = "* Mandatory")]
        public string HSNC_CATEGORYNAME { get; set; }


        [DisplayName("Sort Order")]
        //[Required(ErrorMessage = "* Mandatory")]
        //[Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int HSNC_SORTORDER { get; set; }

        [DisplayName("Disable")]
        public bool IsDisabled
        {
            get => _HSNC_disable == "Y";
            set => _HSNC_disable = value ? "Y" : "N";
        }
        // This field directly maps to the JSON property
        [JsonProperty("HSNC_DISABLE")]
        private string _HSNC_disable { get; set; }

    }

    public class HSNCRootObjects
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<HSNCategory> Data { get; set; } // Changed from List<HSNMaster> to a single LineItems object
        public HSNMaster Datas { get; set; } // Changed from List<HSNMaster> to a single LineItems object

    }

}