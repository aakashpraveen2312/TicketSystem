using Newtonsoft.Json;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace PSS_CMS.Controllers
{
    public class HomeController : Controller
    {
       

        //Over all mainmenu list
        public async Task<ActionResult> Index()
        {
            string Weburl = ConfigurationManager.AppSettings["GetApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];



            List<MainMenu> mainMenuList = new List<MainMenu>();
            string strparams = "strACCESSID=" + "PS001" + "&strUNICID=" + "PS001_Root_Menu" + "&strGroupID=" + "0";
            string finalurl = Weburl + "?" + strparams;

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


                        var response = await client.GetAsync(finalurl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();

                            ApiResponseMainmenu Mainmenucontent = JsonConvert.DeserializeObject<ApiResponseMainmenu>(jsonString);
                            //mainMenuList = rootObjects.Data;
                            List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(Mainmenucontent.Data[0].PS_VALUES);
                            // mainMenuList = rootObjects.Data;  // Assuming Data is a List<string>
                            ViewBag.Menus = rootObjects;


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


           
            await SubmenuForServices();

            // Return the view with the menu data
            return View();
        }

        public bool IsValidJsonArray(string strInput)
        {
            if (string.IsNullOrEmpty(strInput)) return false;

            strInput = strInput.Trim();

            // Check if the string starts and ends with square brackets (i.e., it's an array)
            if (strInput.StartsWith("[") && strInput.EndsWith("]"))
            {
                try
                {
                    // Try to deserialize the string to a List
                    JsonConvert.DeserializeObject<List<object>>(strInput);
                    return true;
                }
                catch (JsonException)
                {
                    // If deserialization fails, it's not a valid JSON array
                    return false;
                }
            }

            return false;
        }

        //Services submenu list
        public async Task<ActionResult> SubmenuForServices()
        {
            string Weburl = ConfigurationManager.AppSettings["GetApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];



            List<SubMenuServices> submenuserviceList = new List<SubMenuServices>();
            string strparams = "strACCESSID=" + "PS001" + "&strUNICID=" + "PS001_Menu_Services" + "&strGroupID=" + "0";
            string serviceurl = Weburl + "?" + strparams;

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


                        var response = await client.GetAsync(serviceurl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();

                            ApiResponseService Servicecontent = JsonConvert.DeserializeObject<ApiResponseService>(jsonString);
                            //mainMenuList = rootObjects.Data;
                            List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(Servicecontent.Data[0].PS_VALUES);
                            // mainMenuList = rootObjects.Data;  // Assuming Data is a List<string>
                            ViewBag.SubMenusService = rootObjects;
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
                        
           
            await SubmenuForHRMS();
            return View();
        }
        //Services submenu list save

        public async Task<ActionResult> SubmenuForHRMS()
        {
            string Weburl = ConfigurationManager.AppSettings["GetApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];



            List<SubMenuHRMS> submenuhrmsList = new List<SubMenuHRMS>();
            string strparams = "strACCESSID=" + "PS001" + "&strUNICID=" + "PS001_Services_HRMS" + "&strGroupID=" + "HRMSDropDown";
            string hrmsurl = Weburl + "?" + strparams;

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


                        var response = await client.GetAsync(hrmsurl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();

                            ApiResponseHRMS HRMScontent = JsonConvert.DeserializeObject<ApiResponseHRMS>(jsonString);
                            //mainMenuList = rootObjects.Data;
                            List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(HRMScontent.Data[0].PS_VALUES);
                            // mainMenuList = rootObjects.Data;  // Assuming Data is a List<string>
                            ViewBag.SubMenusHRMS = rootObjects;
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

           await HomeMainContent();
            // Return the view with the menu data
            return View();
        }

        public async Task<ActionResult> HomeMainContent()
        {


            string Weburl = ConfigurationManager.AppSettings["GetApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];



            List<HomeContentMain> homecontentList = new List<HomeContentMain>();
            string strparams = "strACCESSID=" + "PS002" + "&strUNICID=" + "PS002_Home_MainSection" + "&strGroupID=" + "MainSection";
            string hrmsurl = Weburl + "?" + strparams;

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


                        var response = await client.GetAsync(hrmsurl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();

                            ApiHomeContentResponse homemaincontent = JsonConvert.DeserializeObject<ApiHomeContentResponse>(jsonString);
                            //mainMenuList = rootObjects.Data;
                            List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(homemaincontent.Data[0].PS_VALUES);
                            string psValues = homemaincontent.Data[1].PS_VALUES.ToString();



                            // mainMenuList = rootObjects.Data;  // Assuming Data is a List<string>
                            // Get base64 string of the image if it exists
                            //string base64Image = rootObjects.Data[2].PS_VALUES;
                            string base64Image = psValues;

                            // Check if base64 image exists and assign it to ViewBag and Session
                            if (!string.IsNullOrEmpty(base64Image))
                            {
                                // Determine MIME type based on the base64 content
                                string mimeType = GetImageMimeType(base64Image);  // Implement this method to detect MIME type based on base64 image content

                                // Save the base64 image and MIME type to ViewBag and Session
                                ViewBag.HOMECONTENTMAINIMAGE = base64Image;
                                ViewBag.MIMEType = mimeType;  // Pass MIME type to the view
                                Session["Main_content_Images"] = ViewBag.HOMECONTENTMAINIMAGE;
                            }

                            ViewBag.HOMECONTENTMAIN = rootObjects;
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

            await HomeMainContentAboutUs();

            // Return the view with the menu data
            return View();
        }

        //public async Task<ActionResult> HomeMainContentImage()
        //{


        //    string Weburl = ConfigurationManager.AppSettings["GetApi"];
        //    string AuthKey = ConfigurationManager.AppSettings["Authkey"];

        //    string APIKey = ConfigurationManager.AppSettings["APIKey"];



        //    List<HomeContentMainImages> homecontentList = new List<HomeContentMainImages>();
        //    string strparams = "strACCESSID=" + "PS002" + "&strUNICID=" + "PS002_Home_MainSection_Images";
        //    string hrmsurl = Weburl + "?" + strparams;

        //    try
        //    {
        //        using (HttpClientHandler handler = new HttpClientHandler())
        //        {
        //            handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

        //            using (HttpClient client = new HttpClient(handler))
        //            {
        //                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
        //                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
        //                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


        //                var response = await client.GetAsync(hrmsurl);

        //                if (response.IsSuccessStatusCode)
        //                {
        //                    var jsonString = await response.Content.ReadAsStringAsync();

        //                    ApiHomeContentImageResponse homemainimagecontent = JsonConvert.DeserializeObject<ApiHomeContentImageResponse>(jsonString);

        //                    // Get base64 string of the image if it exists
        //                    string base64Image = homemainimagecontent.Data[0].PS_VALUES;

        //                    // Check if base64 image exists and assign it to ViewBag and Session
        //                    if (!string.IsNullOrEmpty(base64Image))
        //                    {
        //                        // Determine MIME type based on the base64 content
        //                        string mimeType = GetImageMimeType(base64Image);  // Implement this method to detect MIME type based on base64 image content

        //                        // Save the base64 image and MIME type to ViewBag and Session
        //                        ViewBag.HOMECONTENTMAINIMAGE = base64Image;
        //                        ViewBag.MIMEType = mimeType;  // Pass MIME type to the view
        //                        Session["Main_content_Images"] = ViewBag.HOMECONTENTMAINIMAGE;
        //                    }
        //                    else
        //                    {
        //                        // If no image found, set it as null
        //                        ViewBag.HOMECONTENTMAINIMAGE = null;
        //                        Session["Main_content_Images"] = null;
        //                    }
        //                }

        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions (e.g., logging)
        //        ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
        //    }
        //    await HomeMainContentAboutUs();
        //    // Return the view with the menu data
        //    return View();
        //}
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
        public async Task<ActionResult> HomeMainContentAboutUs()
        {


            string Weburl = ConfigurationManager.AppSettings["GetApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];



            List<HomeContentMainAboutus> homecontentList = new List<HomeContentMainAboutus>();
            string strparams = "strACCESSID=" + "PS002" + "&strUNICID=" + "PS002_Home_MainSection_AboutUs" + "&strGroupID=" + "AboutUsSection";
            string HomeContentMainaboutusurl = Weburl + "?" + strparams;

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


                        var response = await client.GetAsync(HomeContentMainaboutusurl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();

                            ApiHomeContentAboutusResponse homemaincontentaboutus = JsonConvert.DeserializeObject<ApiHomeContentAboutusResponse>(jsonString);
                            Session["PS_ABOUT_O_RECID"] = homemaincontentaboutus.Data[0].PS_RECID;
                            Session["PS_ABOUT_O_ACCESSID"] = homemaincontentaboutus.Data[0].PS_ACCESSID;
                            Session["PS_ABOUT_O_PAGENAME"] = homemaincontentaboutus.Data[0].PS_PAGENAME;
                            Session["PS_ABOUT_O_CONTENTTYPE"] = homemaincontentaboutus.Data[0].PS_CONTENTTYPE;
                            Session["PS_ABOUT_O_PARENT"] = homemaincontentaboutus.Data[0].PS_PARENT;
                            Session["PS_ABOUT_O_NAME"] = homemaincontentaboutus.Data[0].PS_NAME;
                            Session["PS_ABOUT_O_ID"] = homemaincontentaboutus.Data[0].PS_ID;
                            Session["PS_ABOUT_O_LASTDATETIME"] = homemaincontentaboutus.Data[0].PS_LASTDATETIME;
                            Session["PS_ABOUT_O_TYPE"] = homemaincontentaboutus.Data[0].PS_TYPE;
                            //mainMenuList = rootObjects.Data;
                            List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(homemaincontentaboutus.Data[0].PS_VALUES);
                            // mainMenuList = rootObjects.Data;  // Assuming Data is a List<string>
                            ViewBag.HOMECONTENTMAINABOUTUS = rootObjects;
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
            await HomeMainContentCoreActivities();
            return View();
        }
        public async Task<ActionResult> HomeMainContentCoreActivities()
        {


            string Weburl = ConfigurationManager.AppSettings["GetApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];



            List<HomeContentCoreactivity> homecontentList = new List<HomeContentCoreactivity>();
            string strparams = "strACCESSID=" + "PS002" + "&strUNICID=" + "PS002_Home_MainSection_Core" + "&strGroupID=" + "CoreSection";
            string HomeContentMainaboutusurl = Weburl + "?" + strparams;

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


                        var response = await client.GetAsync(HomeContentMainaboutusurl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();

                            ApiHomeContentCoreactivityResponse coreactivitytext = JsonConvert.DeserializeObject<ApiHomeContentCoreactivityResponse>(jsonString);
                            Session["PS_CORE_O_RECID"] = coreactivitytext.Data[0].PS_RECID;
                            Session["PS_CORE_O_ACCESSID"] = coreactivitytext.Data[0].PS_ACCESSID;
                            Session["PS_CORE_O_PAGENAME"] = coreactivitytext.Data[0].PS_PAGENAME;
                            Session["PS_CORE_O_CONTENTTYPE"] = coreactivitytext.Data[0].PS_CONTENTTYPE;
                            Session["PS_CORE_O_PARENT"] = coreactivitytext.Data[0].PS_PARENT;
                            Session["PS_CORE_O_NAME"] = coreactivitytext.Data[0].PS_NAME;
                            Session["PS_CORE_O_ID"] = coreactivitytext.Data[0].PS_ID;
                            Session["PS_CORE_O_LASTDATETIME"] = coreactivitytext.Data[0].PS_LASTDATETIME;
                            Session["PS_CORE_O_TYPE"] = coreactivitytext.Data[0].PS_TYPE;
                            //mainMenuList = rootObjects.Data;
                            List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(coreactivitytext.Data[0].PS_VALUES);
                            // mainMenuList = rootObjects.Data;  // Assuming Data is a List<string>
                            ViewBag.HOMECORETEXT = rootObjects;
                            string psValuescore = coreactivitytext.Data[1].PS_VALUES.ToString();


                            string base64Image = psValuescore;
                            // Check if base64 image exists and assign it to ViewBag and Session
                            if (!string.IsNullOrEmpty(base64Image))
                            {
                                // Determine MIME type based on the base64 content
                                string mimeType = GetImageMimeType(base64Image);  // Implement this method to detect MIME type based on base64 image content

                                // Save the base64 image and MIME type to ViewBag and Session
                                ViewBag.HOMECONTENTCOREIMAGE = base64Image;
                                ViewBag.CoreMimeType = mimeType;  // Pass MIME type to the view
                                Session["Core_Activity_Images"] = ViewBag.HOMECONTENTCOREIMAGE;
                            }
                            else
                            {
                                // If no image found, set it as null
                                ViewBag.HOMECONTENTCOREIMAGE = null;
                                Session["Core_Activity_Images"] = null;
                            }
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

            await HomeSkillText();
            // Return the view with the menu data
            return View();
        }
        //public async Task<ActionResult> HomeMainContentCoreImage()
        //{


        //    string Weburl = ConfigurationManager.AppSettings["GetApi"];
        //    string AuthKey = ConfigurationManager.AppSettings["Authkey"];

        //    string APIKey = ConfigurationManager.AppSettings["APIKey"];



        //    List<HomeContentMainImages> homecontentList = new List<HomeContentMainImages>();
        //    string strparams = "strACCESSID=" + "PS002" + "&strUNICID=" + "PS002_Home_MainSection_CoreImage" + "&strGroupID=" + "CoreSection";
        //    string HomeContentMainImageurl = Weburl + "?" + strparams;

        //    try
        //    {
        //        using (HttpClientHandler handler = new HttpClientHandler())
        //        {
        //            handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

        //            using (HttpClient client = new HttpClient(handler))
        //            {
        //                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
        //                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
        //                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


        //                var response = await client.GetAsync(HomeContentMainImageurl);

        //                if (response.IsSuccessStatusCode)
        //                {
        //                    var jsonString = await response.Content.ReadAsStringAsync();

        //                    ApiHomeContentCoreactivityImageResponse coreimagecontent = JsonConvert.DeserializeObject<ApiHomeContentCoreactivityImageResponse>(jsonString);
        //                    // Get base64 string of the image if it exists
        //                    string base64Image = coreimagecontent.Data[0].PS_VALUES;

        //                    // Check if base64 image exists and assign it to ViewBag and Session
        //                    if (!string.IsNullOrEmpty(base64Image))
        //                    {
        //                        // Determine MIME type based on the base64 content
        //                        string mimeType = GetImageMimeType(base64Image);  // Implement this method to detect MIME type based on base64 image content

        //                        // Save the base64 image and MIME type to ViewBag and Session
        //                        ViewBag.HOMECONTENTCOREIMAGE = base64Image;
        //                        ViewBag.CoreMimeType = mimeType;  // Pass MIME type to the view
        //                        Session["Core_Activity_Images"] = ViewBag.HOMECONTENTCOREIMAGE;
        //                    }
        //                    else
        //                    {
        //                        // If no image found, set it as null
        //                        ViewBag.HOMECONTENTCOREIMAGE = null;
        //                        Session["Core_Activity_Images"] = null;
        //                    }
        //                }

        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions (e.g., logging)
        //        ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
        //    }
        //    await HomeSkillText();
        //    return View();
        //}

        public async Task<ActionResult> HomeSkillText()
        {


            string Weburl = ConfigurationManager.AppSettings["GetApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];



            List<HomeContentSkillText> SkillTextList = new List<HomeContentSkillText>();
            string strparams = "strACCESSID=" + "PS002" + "&strUNICID=" + "PS002_Home_MainSection_Skill" + "&strGroupID=" + "SkillSection";
            string HomeSkilltextgeturl = Weburl + "?" + strparams;

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


                        var response = await client.GetAsync(HomeSkilltextgeturl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();

                            ApiHomeSkillTextResponse Skilltext = JsonConvert.DeserializeObject<ApiHomeSkillTextResponse>(jsonString);
                            Session["PS_ST_RECID"] = Skilltext.Data[1].PS_RECID;
                            Session["PS_ST_ACCESSID"] = Skilltext.Data[1].PS_ACCESSID;
                            Session["PS_ST_PAGENAME"] = Skilltext.Data[1].PS_PAGENAME;
                            Session["PS_ST_CONTENTTYPE"] = Skilltext.Data[1].PS_CONTENTTYPE;
                            Session["PS_ST_PARENT"] = Skilltext.Data[1].PS_PARENT;
                            Session["PS_ST_NAME"] = Skilltext.Data[1].PS_NAME;
                            Session["PS_ST_ID"] = Skilltext.Data[1].PS_ID;
                            Session["PS_ST_LASTDATETIME"] = Skilltext.Data[1].PS_LASTDATETIME;
                            Session["PS_ST_TYPE"] = Skilltext.Data[1].PS_TYPE;
                            //mainMenuList = rootObjects.Data;
                            List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(Skilltext.Data[1].PS_VALUES);
                            // mainMenuList = rootObjects.Data;  // Assuming Data is a List<string>
                            ViewBag.HOMESKILLTEXT = rootObjects;
                            string psValuesskill = Skilltext.Data[0].PS_VALUES.ToString();
                            string base64Image = psValuesskill;
                           
                            // Check if base64 image exists and assign it to ViewBag and Session
                            if (!string.IsNullOrEmpty(base64Image))
                            {
                                // Determine MIME type based on the base64 content
                                string mimeType = GetImageMimeType(base64Image);  // Implement this method to detect MIME type based on base64 image content

                                // Save the base64 image and MIME type to ViewBag and Session
                                ViewBag.HOMESKILLIMAGE = base64Image;
                                ViewBag.SkillMimeType = mimeType;  // Pass MIME type to the view
                                Session["Skill_Images"] = ViewBag.HOMESKILLIMAGE;
                            }
                            else
                            {
                                // If no image found, set it as null
                                ViewBag.HOMESKILLIMAGE = null;
                                Session["Skill_Images"] = null;
                            }
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
            await HomeContactUs();
            return View();
        }
        //public async Task<ActionResult> HomeSkillImage()
        //{


        //    string Weburl = ConfigurationManager.AppSettings["GetApi"];
        //    string AuthKey = ConfigurationManager.AppSettings["Authkey"];

        //    string APIKey = ConfigurationManager.AppSettings["APIKey"];



        //    List<HomeContentMainImages> homecontentList = new List<HomeContentMainImages>();
        //    string strparams = "strACCESSID=" + "PS002" + "&strUNICID=" + "PS002_Home_MainSection_SkillImage" + "&strGroupID=" + "SkillSection";
        //    string Homeskillimagegeturl = Weburl + "?" + strparams;

        //    try
        //    {
        //        using (HttpClientHandler handler = new HttpClientHandler())
        //        {
        //            handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

        //            using (HttpClient client = new HttpClient(handler))
        //            {
        //                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
        //                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
        //                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


        //                var response = await client.GetAsync(Homeskillimagegeturl);

        //                if (response.IsSuccessStatusCode)
        //                {
        //                    var jsonString = await response.Content.ReadAsStringAsync();

        //                    ApiHomeSkillImageResponse skillimagecontent = JsonConvert.DeserializeObject<ApiHomeSkillImageResponse>(jsonString);
        //                    Session["PS_SI_RECID"] = skillimagecontent.Data[0].PS_RECID;
        //                    Session["PS_SI_ACCESSID"] = skillimagecontent.Data[0].PS_ACCESSID;
        //                    Session["PS_SI_PAGENAME"] = skillimagecontent.Data[0].PS_PAGENAME;
        //                    Session["PS_SI_CONTENTTYPE"] = skillimagecontent.Data[0].PS_CONTENTTYPE;
        //                    Session["PS_SI_PARENT"] = skillimagecontent.Data[0].PS_PARENT;
        //                    Session["PS_SI_NAME"] = skillimagecontent.Data[0].PS_NAME;
        //                    Session["PS_SI_ID"] = skillimagecontent.Data[0].PS_ID;
        //                    Session["PS_SI_LASTDATETIME"] = skillimagecontent.Data[0].PS_LASTDATETIME;
        //                    Session["PS_SI_TYPE"] = skillimagecontent.Data[0].PS_TYPE;
        //                    // Get base64 string of the image if it exists
        //                    string base64Image = skillimagecontent.Data[0].PS_VALUES;

        //                    // Check if base64 image exists and assign it to ViewBag and Session
        //                    if (!string.IsNullOrEmpty(base64Image))
        //                    {
        //                        // Determine MIME type based on the base64 content
        //                        string mimeType = GetImageMimeType(base64Image);  // Implement this method to detect MIME type based on base64 image content

        //                        // Save the base64 image and MIME type to ViewBag and Session
        //                        ViewBag.HOMESKILLIMAGE = base64Image;
        //                        ViewBag.SkillMimeType = mimeType;  // Pass MIME type to the view
        //                        Session["Skill_Images"] = ViewBag.HOMESKILLIMAGE;
        //                    }
        //                    else
        //                    {
        //                        // If no image found, set it as null
        //                        ViewBag.HOMESKILLIMAGE = null;
        //                        Session["Skill_Images"] = null;
        //                    }
        //                }

        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions (e.g., logging)
        //        ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
        //    }
        //    await HomeContactUs();
        //    return View();
        //}
        public async Task<ActionResult> HomeContactUs()
        {


            string Weburl = ConfigurationManager.AppSettings["GetApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];



            List<Homecontactus> contactusList = new List<Homecontactus>();
            string strparams = "strACCESSID=" + "PS002" + "&strUNICID=" + "PS002_Home_ContactUs" + "&strGroupID=" + "0";
            string Contactusgeturl = Weburl + "?" + strparams;

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


                        var response = await client.GetAsync(Contactusgeturl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();

                            ApiHomeContactus Contactuscontent = JsonConvert.DeserializeObject<ApiHomeContactus>(jsonString);
                            //mainMenuList = rootObjects.Data;
                            List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(Contactuscontent.Data[0].PS_VALUES);
                            // mainMenuList = rootObjects.Data;  // Assuming Data is a List<string>
                            ViewBag.HOMECONTACTUS = rootObjects;
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

            await ClientImageGet();
            return View();
        }

        public async Task<ActionResult> ClientImageGet()
        {
           

                string Weburl = ConfigurationManager.AppSettings["GetApi"];
                string AuthKey = ConfigurationManager.AppSettings["Authkey"];
                string APIKey = ConfigurationManager.AppSettings["APIKey"];

                List<clientimage> homecontentList = new List<clientimage>();
                string strparams = "strACCESSID=" + "PS002" + "&strUNICID=" + "PS002_Home_Client1" + "&strGroupID=" + "CAROUSEL";
                string clienturl = Weburl + "?" + strparams;

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

                            var response = await client.GetAsync(clienturl);

                            if (response.IsSuccessStatusCode)
                            {
                                var jsonString = await response.Content.ReadAsStringAsync();
                                ApiClientimage ClientImagescontent = JsonConvert.DeserializeObject<ApiClientimage>(jsonString);

                                if (ClientImagescontent.Data != null && ClientImagescontent.Data.Count > 0)
                                {
                                    Session["PS_CLIENT_RECID"] = ClientImagescontent.Data[0].PS_RECID;
                                    Session["PS_CLIENT_ACCESSID"] = ClientImagescontent.Data[0].PS_ACCESSID;
                                    Session["PS_CLIENT_PAGENAME"] = ClientImagescontent.Data[0].PS_PAGENAME;
                                    Session["PS_CLIENT_CONTENTTYPE"] = ClientImagescontent.Data[0].PS_CONTENTTYPE;
                                    Session["PS_CLIENT_PARENT"] = ClientImagescontent.Data[0].PS_PARENT;
                                    Session["PS_CLIENT_NAME"] = ClientImagescontent.Data[0].PS_NAME;
                                    Session["PS_CLIENT_ID"] = ClientImagescontent.Data[0].PS_ID;
                                    Session["PS_CLIENT_GROUPID"] = ClientImagescontent.Data[0].PS_GROUPID;
                                    // Save hero type in session
                                    Session["PS_CLIENT_TYPE"] = ClientImagescontent.Data[0].PS_TYPE;

                                    // Pass it to the view via ViewBag
                                    ViewBag.CLIENT = ClientImagescontent.Data[0].PS_TYPE;

                                    // Handle images
                                    List<string> CLIENTImages = new List<string>();
                                    List<string> CLIENTImagesPS_ID = new List<string>();
                                    foreach (var content in ClientImagescontent.Data)
                                    {
                                        string psValue = content.PS_VALUES;
                                        string psID = content.PS_ID;
                                        if (!string.IsNullOrEmpty(psValue))
                                        {
                                            CLIENTImages.Add(psValue);
                                            CLIENTImagesPS_ID.Add(psID);
                                        }
                                    }

                                    // Save the images in session and pass them to ViewBag
                                    Session["MainContentImages"] = CLIENTImages;
                                    Session["MainContentPS_ID"] = CLIENTImagesPS_ID;
                                    ViewBag.MainContentImages = CLIENTImages;
                                }
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
                }
            await HeroSectionVideo();
                return View();
            
        }
        public async Task<ActionResult> HeroSectionVideo()
        {
            string Weburl = ConfigurationManager.AppSettings["GetApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];



            List<HeroVideolink> contactusList = new List<HeroVideolink>();
            string strparams = "strACCESSID=" + "PS002" + "&strUNICID=" + "PS002_Home_MainSection_Video" + "&strGroupID=" + "0";
            string Videourl = Weburl + "?" + strparams;

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


                        var response = await client.GetAsync(Videourl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();

                            ApiHeroVideoLink VIDEOcontent = JsonConvert.DeserializeObject<ApiHeroVideoLink>(jsonString);
                            Session["PS_H_VIDEO_RECID"] = VIDEOcontent.Data[0].PS_RECID;
                            Session["PS_H_VIDEO_ACCESSID"] = VIDEOcontent.Data[0].PS_ACCESSID;
                            Session["PS_H_VIDEO_PAGENAME"] = VIDEOcontent.Data[0].PS_PAGENAME;
                            Session["PS_H_VIDEO_CONTENTTYPE"] = VIDEOcontent.Data[0].PS_CONTENTTYPE;
                            Session["PS_H_VIDEO_PARENT"] = VIDEOcontent.Data[0].PS_PARENT;
                            Session["PS_H_VIDEO_NAME"] = VIDEOcontent.Data[0].PS_NAME;
                            Session["PS_H_VIDEO_ID"] = VIDEOcontent.Data[0].PS_ID;
                            Session["PS_H_VIDEO_LASTDATETIME"] = VIDEOcontent.Data[0].PS_LASTDATETIME;
                            Session["PS_H_VIDEO_TYPE"] = VIDEOcontent.Data[0].PS_TYPE;

                            List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(VIDEOcontent.Data[0].PS_VALUES);
                            // mainMenuList = rootObjects.Data;  // Assuming Data is a List<string>
                            ViewBag.HOMEVIDEO = rootObjects;
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

