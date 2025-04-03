using Newtonsoft.Json;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    public class LearningAndDevelopmentController : Controller
    {
        // GET: Learning
        public async Task<ActionResult> Index()
        {
            var HomeController = new HomeController();

            var data = await HomeController.Index() as ViewResult;
            // If you absolutely must do this, extract the ViewBag data like this
            if (data != null)
            {
                ViewBag.Menus = data.ViewBag.Menus;
                ViewBag.SubMenusContact = data.ViewBag.SubMenusContact;
                ViewBag.SubMenusService = data.ViewBag.SubMenusService;
                ViewBag.SubMenusClient = data.ViewBag.SubMenusClient;
                ViewBag.SubMenusHRMS = data.ViewBag.SubMenusHRMS;
                ViewBag.HOMECONTACTUS = data.ViewBag.HOMECONTACTUS;
                // Transfer other ViewBag data as needed
            }
            await LearningAndDevelopmentMainSection();
            await LearninganddevelopmentAboutUs();
            await LearningandDevelopmentWhyUs();
            await LearningAndDevelopmentSkillsSection();
            return View(data);
        }
        private string GetImageMimeType(string base64Image)
        {
            if (base64Image.Contains("data:image/jpeg;base64,"))
                return "image/jpeg";
            if (base64Image.Contains("data:image/png;base64,"))
                return "image/png";
            if (base64Image.Contains("data:image/gif;base64,"))
                return "image/gif";
            if (base64Image.Contains("data:image/bmp;base64,"))
                return "image/bmp";
            // Default to JPEG if not found
            return "image/jpeg";
        }


        public async Task<ActionResult>LearningAndDevelopmentMainSection()
        {
           

                string Weburl = ConfigurationManager.AppSettings["GetApi"];
                string AuthKey = ConfigurationManager.AppSettings["Authkey"];

                string APIKey = ConfigurationManager.AppSettings["APIKey"];



                List<LearningandDevelopment> learninganddevelopmentList = new List<LearningandDevelopment>();
                string strparams = "strACCESSID=" + "PS00LD" + "&strUNICID=" + "PS00LD_HRMS_LearningAndDevelopment" + "&strGroupID=" + "MainSection_LearningAndDevelopment";
                string learninganddevelopmentmainurl = Weburl + "?" + strparams;

                try
                {
                    using (HttpClientHandler handler = new HttpClientHandler())
                    {
                        handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                        using (HttpClient client = new HttpClient(handler))
                        {
                            client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                            client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                            var response = await client.GetAsync(learninganddevelopmentmainurl);

                            if (response.IsSuccessStatusCode)
                            {
                                var jsonString = await response.Content.ReadAsStringAsync();

                                ApiResponseLearningAndDevelopmentMain Learninganddevelopmentmain = JsonConvert.DeserializeObject<ApiResponseLearningAndDevelopmentMain>(jsonString);
                                Session["PS_T_LearningAndDevelopmentMAIN_RECID"] = Learninganddevelopmentmain.Data[0].PS_RECID;

                                Session["PS_T_LearningAndDevelopmentMAIN_ACCESSID"] = Learninganddevelopmentmain.Data[0].PS_ACCESSID;

                                Session["PS_T_LearningAndDevelopmentMAIN_PAGENAME"] = Learninganddevelopmentmain.Data[0].PS_PAGENAME;

                                Session["PS_T_LearningAndDevelopmentMAIN_CONTENTTYPE"] = Learninganddevelopmentmain.Data[0].PS_CONTENTTYPE;

                                Session["PS_T_LearningAndDevelopmentMAIN_PARENT"] = Learninganddevelopmentmain.Data[0].PS_PARENT;

                                Session["PS_T_LearningAndDevelopmentMAIN_NAME"] = Learninganddevelopmentmain.Data[0].PS_NAME;

                                Session["PS_T_LearningAndDevelopmentMAIN_ID"] = Learninganddevelopmentmain.Data[0].PS_ID;

                                Session["PS_T_LearningAndDevelopmentMAIN_LASTDATETIME"] = Learninganddevelopmentmain.Data[0].PS_LASTDATETIME;

                                Session["PS_T_LearningAndDevelopmentMAIN_TYPE"] = Learninganddevelopmentmain.Data[0].PS_TYPE;

                                List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(Learninganddevelopmentmain.Data[0].PS_VALUES);
                            string psValues = Learninganddevelopmentmain.Data[1].PS_VALUES.ToString();
                            string base64Image = psValues;


                            if (!string.IsNullOrEmpty(base64Image))
                            {

                                string mimeType = GetImageMimeType(base64Image);

                                ViewBag.LEARNINGANDDEVELOPMENTMAINIMAGE = base64Image;
                                ViewBag.MAINMIMEType = mimeType;
                                Session["learninganddevelopment_content_Images"] = ViewBag.LEARNINGANDDEVELOPMENTMAINIMAGE;
                            }

                            ViewBag.LearningAndDevelopmentMainText = rootObjects;
                            }

                            else
                            {
                                // Handle the error response here
                                ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., logging)
                    ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
                }

                return View();
            
            
        }
        public async Task<ActionResult> LearninganddevelopmentAboutUs()
        {

                string Weburl = ConfigurationManager.AppSettings["GetApi"];
                string AuthKey = ConfigurationManager.AppSettings["Authkey"];

                string APIKey = ConfigurationManager.AppSettings["APIKey"];



                List<HomeContentMainAboutus> aboutuscontentList = new List<HomeContentMainAboutus>();
                string strparams = "strACCESSID=" + "PS00LD" + "&strUNICID=" + "PS00LD_HRMS_LearningAndDevelopment_About" + "&strGroupID=" + "AboutSection_LearningAndDevelopment";
                string LearninganddevelopmentContentMainaboutusurl = Weburl + "?" + strparams;


                try
                {
                    using (HttpClientHandler handler = new HttpClientHandler())
                    {
                        handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                        using (HttpClient client = new HttpClient(handler))
                        {
                            client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                            client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                            var response = await client.GetAsync(LearninganddevelopmentContentMainaboutusurl);

                            if (response.IsSuccessStatusCode)
                            {
                                var jsonString = await response.Content.ReadAsStringAsync();

                                ApiResponseLearningAndDevelopmentMainaboutus learninganddevelopmentmaincontentaboutus = JsonConvert.DeserializeObject<ApiResponseLearningAndDevelopmentMainaboutus>(jsonString);
                                Session["PS_LT_ABOUT_RECID"] = learninganddevelopmentmaincontentaboutus.Data[0].PS_RECID;
                                Session["PS_LT_ABOUT_ACCESSID"] = learninganddevelopmentmaincontentaboutus.Data[0].PS_ACCESSID;
                                Session["PS_LT_ABOUT_PAGENAME"] = learninganddevelopmentmaincontentaboutus.Data[0].PS_PAGENAME;
                                Session["PS_LT_ABOUT_CONTENTTYPE"] = learninganddevelopmentmaincontentaboutus.Data[0].PS_CONTENTTYPE;
                                Session["PS_LT_ABOUT_PARENT"] = learninganddevelopmentmaincontentaboutus.Data[0].PS_PARENT;
                                Session["PS_LT_ABOUT_NAME"] = learninganddevelopmentmaincontentaboutus.Data[0].PS_NAME;
                                Session["PS_LT_ABOUT_ID"] = learninganddevelopmentmaincontentaboutus.Data[0].PS_ID;
                                Session["PS_LT_ABOUT_LASTDATETIME"] = learninganddevelopmentmaincontentaboutus.Data[0].PS_LASTDATETIME;
                                Session["PS_LT_ABOUT_TYPE"] = learninganddevelopmentmaincontentaboutus.Data[0].PS_TYPE;
                                //mainMenuList = rootObjects.Data;
                                List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(learninganddevelopmentmaincontentaboutus.Data[0].PS_VALUES);
                                // mainMenuList = rootObjects.Data;  // Assuming Data is a List<string>
                                ViewBag.LEARNINGANDDEVELOPMENTCONTENTMAINABOUTUS = rootObjects;
                            }
                            else
                            {
                                // Handle the error response here
                                ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                            }

                        }

                    }

                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., logging)
                    ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
                }
                return View();
            
           

        }

        public async Task<ActionResult> LearningandDevelopmentWhyUs()
        {
           

                string Weburl = ConfigurationManager.AppSettings["GetApi"];
                string AuthKey = ConfigurationManager.AppSettings["Authkey"];

                string APIKey = ConfigurationManager.AppSettings["APIKey"];



                List<LearningandDevelopment> learninganddevelopmentWhyusList = new List<LearningandDevelopment>();
                string strparams = "strACCESSID=" + "PS00LD" + "&strUNICID=" + "PS00LD_HRMS_LearningAndDevelopment_WhyUs" + "&strGroupID=" + "WhyUsSection_LearningAndDevelopment";
                string learninganddevelopmentwhyusurl = Weburl + "?" + strparams;

                try
                {
                    using (HttpClientHandler handler = new HttpClientHandler())
                    {
                        handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                        using (HttpClient client = new HttpClient(handler))
                        {
                            client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                            client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                            var response = await client.GetAsync(learninganddevelopmentwhyusurl);

                            if (response.IsSuccessStatusCode)
                            {
                                var jsonString = await response.Content.ReadAsStringAsync();

                                ApiResponseLearningAndDevelopmentWhyUS Learninganddevelopmentwhyus = JsonConvert.DeserializeObject<ApiResponseLearningAndDevelopmentWhyUS>(jsonString);
                                Session["PS_T_LearningAndDevelopmentWHYUS_RECID"] = Learninganddevelopmentwhyus.Data[0].PS_RECID;
                              
                                Session["PS_T_LearningAndDevelopmentWHYUS_ACCESSID"] = Learninganddevelopmentwhyus.Data[0].PS_ACCESSID;
                             

                                Session["PS_T_LearningAndDevelopmentWHYUS_PAGENAME"] = Learninganddevelopmentwhyus.Data[0].PS_PAGENAME;
                               

                                Session["PS_T_LearningAndDevelopmentWHYUS_CONTENTTYPE"] = Learninganddevelopmentwhyus.Data[0].PS_CONTENTTYPE;
                               

                                Session["PS_T_LearningAndDevelopmentWHYUS_PARENT"] = Learninganddevelopmentwhyus.Data[0].PS_PARENT;
                               

                                Session["PS_T_LearningAndDevelopmentWHYUS_NAME"] = Learninganddevelopmentwhyus.Data[0].PS_NAME;
                               

                                Session["PS_T_LearningAndDevelopmentWHYUS_ID"] = Learninganddevelopmentwhyus.Data[0].PS_ID;
                              

                                Session["PS_T_LearningAndDevelopmentWHYUS_LASTDATETIME"] = Learninganddevelopmentwhyus.Data[0].PS_LASTDATETIME;
                              

                                Session["PS_T_LearningAndDevelopmentWHYUS_TYPE"] = Learninganddevelopmentwhyus.Data[0].PS_TYPE;


                                List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(Learninganddevelopmentwhyus.Data[0].PS_VALUES);
                            string psValues = Learninganddevelopmentwhyus.Data[1].PS_VALUES.ToString();
                            string base64Image = psValues;


                            if (!string.IsNullOrEmpty(base64Image))
                            {

                                string mimeType = GetImageMimeType(base64Image);

                                ViewBag.LEARNINGANDDEVELOPMENTWHYUSIMAGE = base64Image;
                                ViewBag.WHYUSMIMEType = mimeType;
                                Session["learninganddevelopment_whyus_Images"] = ViewBag.LEARNINGANDDEVELOPMENTWHYUSIMAGE;
                            }

                            ViewBag.LearningAndDevelopmentWhyUSText = rootObjects;
                            }

                            else
                            {
                                // Handle the error response here
                                ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., logging)
                    ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
                }

                return View();
            
        }

        public async Task<ActionResult> LearningAndDevelopmentSkillsSection()
        {
            string Weburl = ConfigurationManager.AppSettings["GetApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];



            List<LearningandDevelopmentSkills> learninganddevelopmentskillList = new List<LearningandDevelopmentSkills>();
            string strparams = "strACCESSID=" + "PS00LD" + "&strUNICID=" + "PS00LD_HRMS_LearningAndDevelopment_Skill" + "&strGroupID=" + "SkillSection_LearningAndDevelopment";
            string learninganddevelopmentskillurl = Weburl + "?" + strparams;

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                        client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                        var response = await client.GetAsync(learninganddevelopmentskillurl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();

                            ApiResponseLearningAndDevelopmentSkill Learninganddevelopmentskill = JsonConvert.DeserializeObject<ApiResponseLearningAndDevelopmentSkill>(jsonString);
                            Session["PS_T_LearningAndDevelopmentSKILL_RECID"] = Learninganddevelopmentskill.Data[0].PS_RECID;

                            Session["PS_T_LearningAndDevelopmentSKILL_ACCESSID"] = Learninganddevelopmentskill.Data[0].PS_ACCESSID;

                            Session["PS_T_LearningAndDevelopmentSKILL_PAGENAME"] = Learninganddevelopmentskill.Data[0].PS_PAGENAME;

                            Session["PS_T_LearningAndDevelopmentSKILL_CONTENTTYPE"] = Learninganddevelopmentskill.Data[0].PS_CONTENTTYPE;

                            Session["PS_T_LearningAndDevelopmentSKILL_PARENT"] = Learninganddevelopmentskill.Data[0].PS_PARENT;

                            Session["PS_T_LearningAndDevelopmentSKILL_NAME"] = Learninganddevelopmentskill.Data[0].PS_NAME;

                            Session["PS_T_LearningAndDevelopmentSKILL_ID"] = Learninganddevelopmentskill.Data[0].PS_ID;

                            Session["PS_T_LearningAndDevelopmentSKILL_LASTDATETIME"] = Learninganddevelopmentskill.Data[0].PS_LASTDATETIME;

                            Session["PS_T_LearningAndDevelopmentSKILL_TYPE"] = Learninganddevelopmentskill.Data[0].PS_TYPE;

                            List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(Learninganddevelopmentskill.Data[0].PS_VALUES);
                            string psValues = Learninganddevelopmentskill.Data[1].PS_VALUES.ToString();
                            string base64Image = psValues;


                            if (!string.IsNullOrEmpty(base64Image))
                            {

                                string mimeType = GetImageMimeType(base64Image);

                                ViewBag.LEARNINGANDDEVELOPMENTSKILLIMAGE = base64Image;
                                ViewBag.SKILLSMIMEType = mimeType;
                                Session["learninganddevelopment_skill_Images"] = ViewBag.LEARNINGANDDEVELOPMENTSKILLIMAGE;
                            }

                            ViewBag.LearningAndDevelopmentSkillText = rootObjects;
                        }

                        else
                        {
                            // Handle the error response here
                            ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., logging)
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            return View();
        }
    }
}