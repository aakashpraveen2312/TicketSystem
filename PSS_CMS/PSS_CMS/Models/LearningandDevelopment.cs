using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PSS_CMS.Models
{
    public class LearningandDevelopment
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
    public class ApiResponseLearningAndDevelopmentMain
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<LearningandDevelopment> Data { get; set; }
    }

    public class LearningandDevelopmentImages
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
    public class ApiResponseLearningAndDevelopmentImageResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<LearningandDevelopmentImages> Data { get; set; }
    }

    public class LearningandDevelopmentaboutus
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
    public class ApiResponseLearningAndDevelopmentMainaboutus
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<LearningandDevelopmentaboutus> Data { get; set; }
    }

    public class LearningandDevelopmentWhyUS
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
    public class ApiResponseLearningAndDevelopmentWhyUS
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<LearningandDevelopmentWhyUS> Data { get; set; }
    }

    public class LearningandDevelopmentWhyUSText
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
    public class ApiResponseLearningAndDevelopmentWhyUSText
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<LearningandDevelopmentWhyUSText> Data { get; set; }
    }

    public class LearningandDevelopmentWhyUSImage
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
    public class ApiResponseLearningAndDevelopmentWhyUSImage
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<LearningandDevelopmentWhyUSImage> Data { get; set; }
    }
    public class LearningandDevelopmentSkills
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
    public class ApiResponseLearningAndDevelopmentSkill
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<LearningandDevelopmentSkills> Data { get; set; }
    }
    public class LearningandDevelopmentSkillsText
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
    public class ApiResponseLearningAndDevelopmentSkillText
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<LearningandDevelopmentSkillsText> Data { get; set; }
    }
    public class LearningandDevelopmentSkillsImg
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
    public class ApiResponseLearningAndDevelopmentSkillImg
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public List<LearningandDevelopmentSkillsImg> Data { get; set; }
    }
}