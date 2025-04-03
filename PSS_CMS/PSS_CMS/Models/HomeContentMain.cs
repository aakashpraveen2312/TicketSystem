using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class HomeContentMain
    {
       
        public string PS_RECID { get; set; }
            public string PS_ACCESSID { get; set; }
            public string PS_PAGENAME { get; set; }
            public string PS_CONTENTTYPE { get; set; }
            public string PS_PARENT { get; set; }
            public string PS_NAME { get; set; }
            public string PS_ID { get; set; }
        public string PS_VALUES { get; set; }  // This will now store the list of values
        public string PS_LASTDATETIME { get; set; }
        public string PS_TYPE { get; set; }
    }
    public class HomeContentMainImages
    {

        public string PS_RECID { get; set; }
        public string PS_ACCESSID { get; set; }
        public string PS_PAGENAME { get; set; }
        public string PS_CONTENTTYPE { get; set; }
        public string PS_PARENT { get; set; }
        public string PS_NAME { get; set; }
        public string PS_ID { get; set; }
        public string PS_VALUES { get; set; }  // This will now store the list of values
        public string PS_LASTDATETIME { get; set; }
        public string PS_TYPE { get; set; }
    }
    public class HomeContentMainAboutus
    {

        public string PS_RECID { get; set; }
        public string PS_ACCESSID { get; set; }
        public string PS_PAGENAME { get; set; }
        public string PS_CONTENTTYPE { get; set; }
        public string PS_PARENT { get; set; }
        public string PS_NAME { get; set; }
        public string PS_ID { get; set; }
        public string PS_VALUES { get; set; }  // This will now store the list of values
        public string PS_LASTDATETIME { get; set; }
        public string PS_TYPE { get; set; }
    }
    public class HomeContentCoreactivity
    {

        public string PS_RECID { get; set; }
        public string PS_ACCESSID { get; set; }
        public string PS_PAGENAME { get; set; }
        public string PS_CONTENTTYPE { get; set; }
        public string PS_PARENT { get; set; }
        public string PS_NAME { get; set; }
        public string PS_ID { get; set; }
        public string PS_VALUES { get; set; }  
        public string PS_LASTDATETIME { get; set; }
        public string PS_TYPE { get; set; }
    }
    
    public class HomeContentSkillText
    {

        public string PS_RECID { get; set; }
        public string PS_ACCESSID { get; set; }
        public string PS_PAGENAME { get; set; }
        public string PS_CONTENTTYPE { get; set; }
        public string PS_PARENT { get; set; }
        public string PS_NAME { get; set; }
        public string PS_ID { get; set; }
        public string PS_VALUES { get; set; }  
        public string PS_LASTDATETIME { get; set; }
        public string PS_TYPE { get; set; }
    }
    public class HomeContentCoreactivityimage
    {

        public string PS_RECID { get; set; }
        public string PS_ACCESSID { get; set; }
        public string PS_PAGENAME { get; set; }
        public string PS_CONTENTTYPE { get; set; }
        public string PS_PARENT { get; set; }
        public string PS_NAME { get; set; }
        public string PS_ID { get; set; }
        public string PS_VALUES { get; set; }  // This will now store the list of values
        public string PS_LASTDATETIME { get; set; }
        public string PS_TYPE { get; set; }
    }
    public class HomeSkillimage
    {

        public string PS_RECID { get; set; }
        public string PS_ACCESSID { get; set; }
        public string PS_PAGENAME { get; set; }
        public string PS_CONTENTTYPE { get; set; }
        public string PS_PARENT { get; set; }
        public string PS_NAME { get; set; }
        public string PS_ID { get; set; }
        public string PS_VALUES { get; set; }  // This will now store the list of values
        public string PS_LASTDATETIME { get; set; }
        public string PS_TYPE { get; set; }
    }

    public class Homecontactus
    {

        public string PS_RECID { get; set; }
        public string PS_ACCESSID { get; set; }
        public string PS_PAGENAME { get; set; }
        public string PS_CONTENTTYPE { get; set; }
        public string PS_PARENT { get; set; }
        public string PS_NAME { get; set; }
        public string PS_ID { get; set; }
        public string PS_VALUES { get; set; }  // This will now store the list of values
        public string PS_LASTDATETIME { get; set; }
        public string PS_TYPE { get; set; }
    }
    public class HeroVideolink
    {

        public string PS_RECID { get; set; }
        public string PS_ACCESSID { get; set; }
        public string PS_PAGENAME { get; set; }
        public string PS_CONTENTTYPE { get; set; }
        public string PS_PARENT { get; set; }
        public string PS_NAME { get; set; }
        public string PS_ID { get; set; }
        public string PS_VALUES { get; set; }  // This will now store the list of values
        public string PS_LASTDATETIME { get; set; }
        public string PS_TYPE { get; set; }
    }
    public class ApiHomeContentResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<HomeContentMain> Data { get; set; }
    } 
    

    public class ApiHomeContentAboutusResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<HomeContentMainAboutus> Data { get; set; }
    }
    public class ApiHomeContentImageResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<HomeContentMainImages> Data { get; set; }
    }
    
    public class ApiHomeContentCoreactivityResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<HomeContentCoreactivity> Data { get; set; }
    }  
    
    public class ApiHomeSkillTextResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<HomeContentSkillText> Data { get; set; }
    }      
    
    public class ApiHomeContentCoreactivityImageResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<HomeContentCoreactivityimage> Data { get; set; }
    }  
    
    public class ApiHomeSkillImageResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<HomeSkillimage> Data { get; set; }
    }    
    
    public class ApiHomeContactus
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<Homecontactus> Data { get; set; }
    }

    public class ApiHeroVideoLink
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<HeroVideolink> Data { get; set; }
    }
}