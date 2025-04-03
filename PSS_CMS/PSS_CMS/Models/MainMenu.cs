using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class MainMenu
    {
        [Key]
        public string PS_RECID { get; set; }
        public string PS_ACCESSID { get; set; }
        public string PS_PAGENAME { get; set; }
        public string PS_CONTENTTYPE { get; set; }
        public string PS_PARENT { get; set; }
        public string PS_ID { get; set; }

        public string PS_NAME { get; set; }


        // Change PS_VALUES from string to List<string>
        public string PS_VALUES { get; set; }  // This will now store the list of values
        public string PS_LASTDATETIME { get; set; }
        public string PS_TYPE { get; set; }

    }

    public class SubMenuContact
    {
        [Key]
        public string PS_RECID { get; set; }
        public string PS_ACCESSID { get; set; }
        public string PS_PAGENAME { get; set; }
        public string PS_CONTENTTYPE { get; set; }
        public string PS_PARENT { get; set; }
        public string PS_ID { get; set; }

        public string PS_NAME { get; set; }


        // Change PS_VALUES from string to List<string>
        public List<string> PS_VALUES { get; set; }  // This will now store the list of values
        public string PS_VALUE { get; set; }  // This will now store the list of values
    }


    public class SubMenuServices
    {
        [Key]
        public string PS_RECID { get; set; }
        public string PS_ACCESSID { get; set; }
        public string PS_PAGENAME { get; set; }
        public string PS_CONTENTTYPE { get; set; }
        public string PS_PARENT { get; set; }
        public string PS_ID { get; set; }

        public string PS_NAME { get; set; }


        // Change PS_VALUES from string to List<string>
        public string PS_VALUES { get; set; }  // This will now store the list of values
        public string PS_LASTDATETIME { get; set; }
        public string PS_TYPE { get; set; }


    }


    public class SubMenuClients
    {
        [Key]
        public string PS_RECID { get; set; }
        public string PS_ACCESSID { get; set; }
        public string PS_PAGENAME { get; set; }
        public string PS_CONTENTTYPE { get; set; }
        public string PS_PARENT { get; set; }
        public string PS_ID { get; set; }

        public string PS_NAME { get; set; }


        // Change PS_VALUES from string to List<string>
        public List<string> PS_VALUES { get; set; }  // This will now store the list of values
        public string PS_VALUE { get; set; }  // This will now store the list of values

       
    }


    public class SubMenuHRMS
    {
        [Key]
        public string PS_RECID { get; set; }
        public string PS_ACCESSID { get; set; }
        public string PS_PAGENAME { get; set; }
        public string PS_CONTENTTYPE { get; set; }
        public string PS_PARENT { get; set; }
        public string PS_ID { get; set; }

        public string PS_NAME { get; set; }


        // Change PS_VALUES from string to List<string>
        public string PS_VALUES { get; set; }  // This will now store the list of values
        public string PS_LASTDATETIME { get; set; }
        public string PS_TYPE { get; set; }


    }

    public class ApiResponseMainmenu
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<MainMenu> Data { get; set; }
        
    }   
    
    public class ApiResponseService
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<SubMenuServices> Data { get; set; }
        
    }  
    
    
    public class ApiResponseHRMS
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<SubMenuHRMS> Data { get; set; }
        
    }

}