using Newtonsoft.Json;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    public class TemplateLayoutController : Controller
    {
        // GET: TemplateLayout
        public async Task<ActionResult> HomeHeroSection(string section)
        {
            // Clear unrelated session variables
            Session["PS_HT_ABOUT_TYPE"] = null;
            Session["PS_H_CORE_TYPE"] = null;
            Session["PS_SKILL_TYPE"] = null;
            Session["PS_H_CONTACT_TYPE"] = null;
            Session["PS_H_VIDEO_TYPE"] = null;

            if (section == "HeroSection")
            {
               Session["ContentName"]=section; 
               
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
                                    Session["PS_H_HERO_RECID"] = homemaincontent.Data[0].PS_RECID;
                                    Session["PS_H_HEROIMG_RECID"] = homemaincontent.Data[1].PS_RECID;
                                    Session["PS_H_HERO_ACCESSID"] = homemaincontent.Data[0].PS_ACCESSID;
                                    Session["PS_H_HEROIMG_ACCESSID"] = homemaincontent.Data[1].PS_ACCESSID;
                                    Session["PS_H_HERO_PAGENAME"] = homemaincontent.Data[0].PS_PAGENAME;
                                    Session["PS_H_HEROIMG_PAGENAME"] = homemaincontent.Data[1].PS_PAGENAME;
                                    Session["PS_H_HERO_CONTENTTYPE"] = homemaincontent.Data[0].PS_CONTENTTYPE;
                                    Session["PS_H_HEROIMG_CONTENTTYPE"] = homemaincontent.Data[1].PS_CONTENTTYPE;
                                    Session["PS_H_HERO_PARENT"] = homemaincontent.Data[0].PS_PARENT;
                                    Session["PS_H_HEROIMG_PARENT"] = homemaincontent.Data[1].PS_PARENT;
                                    Session["PS_H_HERO_NAME"] = homemaincontent.Data[0].PS_NAME;
                                    Session["PS_H_HEROIMG_NAME"] = homemaincontent.Data[1].PS_NAME;
                                    Session["PS_H_HERO_ID"] = homemaincontent.Data[0].PS_ID;
                                    Session["PS_H_HEROIMG_ID"] = homemaincontent.Data[1].PS_ID;
                                    Session["PS_H_HERO_LASTDATETIME"] = homemaincontent.Data[0].PS_LASTDATETIME;
                                    Session["PS_H_HEROIMG_LASTDATETIME"] = homemaincontent.Data[1].PS_LASTDATETIME;
                                    Session["PS_H_HERO_TYPE_TEXT"] = homemaincontent.Data[0].PS_TYPE;

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

                return View("HomePageHeroSection");
                }
                return RedirectToAction("HomeMainContent", "RecentTickets");
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
        [HttpPost]
        public async Task<ActionResult> HomeHeroSectionTextSave(List<string> psValues, HomeContentMain Objhomecontent, string Form)
        {
            string HomeContentMainurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_H_HERO_RECID = Session["PS_H_HERO_RECID"].ToString();
            string PS_H_HERO_ACCESSID = Session["PS_H_HERO_ACCESSID"].ToString();
            string PS_H_HERO_PAGENAME = Session["PS_H_HERO_PAGENAME"].ToString();
            string PS_H_HERO_CONTENTTYPE = Session["PS_H_HERO_CONTENTTYPE"].ToString();
            string PS_H_HERO_PARENT = Session["PS_H_HERO_PARENT"].ToString();
            string PS_H_HERO_NAME = Session["PS_H_HERO_NAME"].ToString();
            string PS_H_HERO_ID = Session["PS_H_HERO_ID"].ToString();
            string PS_H_HERO_LASTDATETIME = Session["PS_H_HERO_LASTDATETIME"].ToString();
            string PS_H_HERO_TYPE = Session["PS_H_HERO_TYPE_TEXT"].ToString();


            try
            {


                var escapedPsValues = psValues.Select(value =>
      value.Replace("\\", "\\\\")  // Escape backslashes
           .Replace("\"", "\\\"")  // Escape quotes
           .Replace("\n", "\\n")   // Escape newlines
           .Replace("\r", "\\r")); // Escape carriage returns

                // Create the desired format for pS_VALUES as a string
                string wrappedSerializedPsValues = "[" + string.Join(",", escapedPsValues.Select(value => $"\"{value}\"")) + "]";

                // Escape the quotes properly for a string value
                string escapedWrappedSerializedPsValues = wrappedSerializedPsValues.Replace("\"", "\\\"");

                // Construct the JSON payload
                var content = $@"{{
                        ""pS_RECID"": ""{PS_H_HERO_RECID}"",
                        ""pS_ACCESSID"": ""{PS_H_HERO_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_H_HERO_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_H_HERO_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_H_HERO_PARENT}"",
                        ""pS_NAME"": ""{PS_H_HERO_NAME}"",
                        ""pS_ID"": ""{PS_H_HERO_ID}"",
                        ""pS_VALUES"": ""{escapedWrappedSerializedPsValues}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_H_HERO_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{HomeContentMainurl}"), // Assuming the ID is used in the URL
                    Method = HttpMethod.Put, // Change from POST to PUT
                    Headers =
                            {
                                { "X-Version", "1" },
                                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
                            },
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler)
                {
                };
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiHomeContentResponse>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<ActionResult> HomeHeroSectionImageSave(HttpPostedFileBase file, HomeContentMainImages HomecontentMainimages, string ExistingImageMain)
        {

            string HomeContentMainImageurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_H_HEROIMG_RECID = Session["PS_H_HEROIMG_RECID"].ToString();
            string PS_H_HEROIMG_ACCESSID = Session["PS_H_HEROIMG_ACCESSID"].ToString();
            string PS_H_HEROIMG_PAGENAME = Session["PS_H_HEROIMG_PAGENAME"].ToString();
            string PS_H_HEROIMG_CONTENTTYPE = Session["PS_H_HEROIMG_CONTENTTYPE"].ToString();
            string PS_H_HEROIMG_PARENT = Session["PS_H_HEROIMG_PARENT"].ToString();
            string PS_H_HEROIMG_NAME = Session["PS_H_HEROIMG_NAME"].ToString();
            string PS_H_HEROIMG_ID = Session["PS_H_HEROIMG_ID"].ToString();
            string PS_H_HEROIMG_LASTDATETIME = Session["PS_H_HEROIMG_LASTDATETIME"].ToString();
            string PS_H_HEROIMG_TYPE = Session["PS_H_HERO_TYPE_TEXT"].ToString();


            string base64Image;                                                        

            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    // Convert the uploaded image to Base64
                    byte[] fileBytes = new byte[file.ContentLength];
                    file.InputStream.Read(fileBytes, 0, file.ContentLength);
                    base64Image = Convert.ToBase64String(fileBytes);
                    // Update session with new image
                    Session["Main_Image"] = base64Image;
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error processing the file: {ex.Message}";
                    return RedirectToAction("HomeMainContent", "RecentTickets"); // Redirect to the original page
                }
            }
            else if (!string.IsNullOrEmpty(ExistingImageMain))
            {
                // Use the existing image
                base64Image = ExistingImageMain;

                // Update the session with the existing image (to ensure it's consistent in the session)
                Session["Main_Image"] = base64Image;
            }
            else
            {
                TempData["ErrorMessage"] = "No file selected or existing image provided.";
                return RedirectToAction("HomeMainContent", "RecentTickets"); // Redirect to the original page
            }
            try
            {
                // Construct the JSON payload
                var content = $@"{{
                        ""pS_RECID"": ""{PS_H_HEROIMG_RECID}"",
                        ""pS_ACCESSID"": ""{PS_H_HEROIMG_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_H_HEROIMG_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_H_HEROIMG_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_H_HEROIMG_PARENT}"",
                        ""pS_NAME"": ""{PS_H_HEROIMG_NAME}"",
                        ""pS_ID"": ""{PS_H_HEROIMG_ID}"",
                        ""pS_VALUES"": ""{base64Image}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_H_HEROIMG_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{HomeContentMainImageurl}"), // Assuming the ID is used in the URL
                    Method = HttpMethod.Put, // Change from POST to PUT
                    Headers =
                            {
                                { "X-Version", "1" },
                                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
                            },
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler)
                {
                };
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiHomeContentImageResponse>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("HomeHeroSection", "TemplateLayout" , new { section = "HeroSection" });
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
            return RedirectToAction("Index", "Home");
        }
       
        public async Task<ActionResult> HomePageAboutUs(string section)
        {
            Session["PS_H_HERO_TYPE_TEXT"] = null;
            Session["PS_H_CORE_TYPE"] = null;
            Session["PS_SKILL_TYPE"] = null;
            Session["PS_H_CONTACT_TYPE"] = null;
            Session["PS_H_VIDEO_TYPE"] = null;

            if (section == "About Us")
            {
                Session["ContentName"] = section;

                string Weburl = ConfigurationManager.AppSettings["GetApi"];
                string AuthKey = ConfigurationManager.AppSettings["Authkey"];

                string APIKey = ConfigurationManager.AppSettings["APIKey"];



                List<HomeContentMainAboutus> homecontentList = new List<HomeContentMainAboutus>();
                string strparams = "strACCESSID=" + "PS002" + "&strUNICID=" + "PS002_Home_AboutUs" + "&strGroupID=" + "AboutUsSection";
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
                                Session["PS_HT_ABOUT_RECID"] = homemaincontentaboutus.Data[0].PS_RECID;
                                Session["PS_HT_ABOUT_ACCESSID"] = homemaincontentaboutus.Data[0].PS_ACCESSID;
                                Session["PS_HT_ABOUT_PAGENAME"] = homemaincontentaboutus.Data[0].PS_PAGENAME;
                                Session["PS_HT_ABOUT_CONTENTTYPE"] = homemaincontentaboutus.Data[0].PS_CONTENTTYPE;
                                Session["PS_HT_ABOUT_PARENT"] = homemaincontentaboutus.Data[0].PS_PARENT;
                                Session["PS_HT_ABOUT_NAME"] = homemaincontentaboutus.Data[0].PS_NAME;
                                Session["PS_HT_ABOUT_ID"] = homemaincontentaboutus.Data[0].PS_ID;
                                Session["PS_HT_ABOUT_LASTDATETIME"] = homemaincontentaboutus.Data[0].PS_LASTDATETIME;
                                Session["PS_HT_ABOUT_TYPE"] = homemaincontentaboutus.Data[0].PS_TYPE;
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
                return View("HomePageHeroSection");
            }
            return RedirectToAction("HomeMainContent","RecentTickets");
            
        }
        
        [HttpPost]
        public async Task<ActionResult> HomePageAboutUsSave(List<string> psValues, HomeContentMainAboutus Objaboutus, string Form)
        {
            string HomeContentAboutUSurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_HT_ABOUT_RECID = Session["PS_HT_ABOUT_RECID"].ToString();
            string PS_HT_ABOUT_ACCESSID = Session["PS_HT_ABOUT_ACCESSID"].ToString();
            string PS_HT_ABOUT_PAGENAME = Session["PS_HT_ABOUT_PAGENAME"].ToString();
            string PS_HT_ABOUT_CONTENTTYPE = Session["PS_HT_ABOUT_CONTENTTYPE"].ToString();
            string PS_HT_ABOUT_PARENT = Session["PS_HT_ABOUT_PARENT"].ToString();
            string PS_HT_ABOUT_NAME = Session["PS_HT_ABOUT_NAME"].ToString();
            string PS_HT_ABOUT_ID = Session["PS_HT_ABOUT_ID"].ToString();
            string PS_HT_ABOUT_LASTDATETIME = Session["PS_HT_ABOUT_LASTDATETIME"].ToString();
            string PS_HT_ABOUT_TYPE = Session["PS_HT_ABOUT_TYPE"].ToString();

            try
            {


                var escapedPsValues = psValues.Select(value =>
      value.Replace("\\", "\\\\")  // Escape backslashes
           .Replace("\"", "\\\"")  // Escape quotes
           .Replace("\n", "\\n")   // Escape newlines
           .Replace("\r", "\\r")); // Escape carriage returns

                // Create the desired format for pS_VALUES as a string
                string wrappedSerializedPsValues = "[" + string.Join(",", escapedPsValues.Select(value => $"\"{value}\"")) + "]";

                // Escape the quotes properly for a string value
                string escapedWrappedSerializedPsValues = wrappedSerializedPsValues.Replace("\"", "\\\"");

                // Construct the JSON payload
                var content = $@"{{
                        ""pS_RECID"": ""{PS_HT_ABOUT_RECID}"",
                        ""pS_ACCESSID"": ""{PS_HT_ABOUT_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_HT_ABOUT_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_HT_ABOUT_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_HT_ABOUT_PARENT}"",
                        ""pS_NAME"": ""{PS_HT_ABOUT_NAME}"",
                        ""pS_ID"": ""{PS_HT_ABOUT_ID}"",
                        ""pS_VALUES"": ""{escapedWrappedSerializedPsValues}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_HT_ABOUT_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{HomeContentAboutUSurl}"), // Assuming the ID is used in the URL
                    Method = HttpMethod.Put, // Change from POST to PUT
                    Headers =
                            {
                                { "X-Version", "1" },
                                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
                            },
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler)
                {
                };
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiHomeContentAboutusResponse>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> HomeCore(string section)
        {

            Session["PS_H_HERO_TYPE_TEXT"] = null;
            Session["PS_SKILL_TYPE"] = null;
            Session["PS_HT_ABOUT_TYPE"] = null;
            Session["PS_H_CONTACT_TYPE"] = null;
            Session["PS_H_VIDEO_TYPE"] = null;

            if (section == "Core Section")
            {
                Session["ContentName"] = section;
                string Weburl = ConfigurationManager.AppSettings["GetApi"];
                string AuthKey = ConfigurationManager.AppSettings["Authkey"];

                string APIKey = ConfigurationManager.AppSettings["APIKey"];



                List<HomeContentCoreactivity> homecontentList = new List<HomeContentCoreactivity>();
                string strparams = "strACCESSID=" + "PS002" + "&strUNICID=" + "PS002_Home_Core" + "&strGroupID=" + "CoreSection";
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
                                Session["PS_H_CORE_RECID"] = coreactivitytext.Data[0].PS_RECID;
                                Session["PS_H_COREIMG_RECID"] = coreactivitytext.Data[1].PS_RECID;
                                Session["PS_H_CORE_ACCESSID"] = coreactivitytext.Data[0].PS_ACCESSID;
                                Session["PS_H_COREIMG_ACCESSID"] = coreactivitytext.Data[1].PS_ACCESSID;
                                Session["PS_H_CORE_PAGENAME"] = coreactivitytext.Data[0].PS_PAGENAME;
                                Session["PS_H_COREIMG_PAGENAME"] = coreactivitytext.Data[1].PS_PAGENAME;
                                Session["PS_H_CORE_CONTENTTYPE"] = coreactivitytext.Data[0].PS_CONTENTTYPE;
                                Session["PS_H_COREIMG_CONTENTTYPE"] = coreactivitytext.Data[1].PS_CONTENTTYPE;
                                Session["PS_H_CORE_PARENT"] = coreactivitytext.Data[0].PS_PARENT;
                                Session["PS_H_COREIMG_PARENT"] = coreactivitytext.Data[1].PS_PARENT;
                                Session["PS_H_CORE_NAME"] = coreactivitytext.Data[0].PS_NAME;
                                Session["PS_H_COREIMG_NAME"] = coreactivitytext.Data[1].PS_NAME;
                                Session["PS_H_CORE_ID"] = coreactivitytext.Data[0].PS_ID;
                                Session["PS_H_COREIMG_ID"] = coreactivitytext.Data[1].PS_ID;
                                Session["PS_H_CORE_LASTDATETIME"] = coreactivitytext.Data[0].PS_LASTDATETIME;
                                Session["PS_H_COREIMG_LASTDATETIME"] = coreactivitytext.Data[1].PS_LASTDATETIME;
                                Session["PS_H_CORE_TYPE"] = coreactivitytext.Data[0].PS_TYPE;
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
                return View("HomePageHeroSection");
            }
            return RedirectToAction("HomeMainContent","RecentTickets");
        }

        [HttpPost]
        public async Task<ActionResult> HomeCoreTextSave(List<string> psValues, HomeContentCoreactivity Objcoretext, string Form)
        {
            string HomeContentcoretexturl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_H_CORE_RECID = Session["PS_H_CORE_RECID"].ToString();
            string PS_H_CORE_ACCESSID = Session["PS_H_CORE_ACCESSID"].ToString();
            string PS_H_CORE_PAGENAME = Session["PS_H_CORE_PAGENAME"].ToString();
            string PS_H_CORE_CONTENTTYPE = Session["PS_H_CORE_CONTENTTYPE"].ToString();
            string PS_H_CORE_PARENT = Session["PS_H_CORE_PARENT"].ToString();
            string PS_H_CORE_NAME = Session["PS_H_CORE_NAME"].ToString();
            string PS_H_CORE_ID = Session["PS_H_CORE_ID"].ToString();
            string PS_H_CORE_LASTDATETIME = Session["PS_H_CORE_LASTDATETIME"].ToString();
            string PS_H_CORE_TYPE = Session["PS_H_CORE_TYPE"].ToString();

            try
            {


                var escapedPsValues = psValues.Select(value =>
      value.Replace("\\", "\\\\")  // Escape backslashes
           .Replace("\"", "\\\"")  // Escape quotes
           .Replace("\n", "\\n")   // Escape newlines
           .Replace("\r", "\\r")); // Escape carriage returns

                // Create the desired format for pS_VALUES as a string
                string wrappedSerializedPsValues = "[" + string.Join(",", escapedPsValues.Select(value => $"\"{value}\"")) + "]";

                // Escape the quotes properly for a string value
                string escapedWrappedSerializedPsValues = wrappedSerializedPsValues.Replace("\"", "\\\"");

                // Construct the JSON payload
                var content = $@"{{
                        ""pS_RECID"": ""{PS_H_CORE_RECID}"",
                        ""pS_ACCESSID"": ""{PS_H_CORE_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_H_CORE_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_H_CORE_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_H_CORE_PARENT}"",
                        ""pS_NAME"": ""{PS_H_CORE_NAME}"",
                        ""pS_ID"": ""{PS_H_CORE_ID}"",
                        ""pS_VALUES"": ""{escapedWrappedSerializedPsValues}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_H_CORE_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{HomeContentcoretexturl}"), // Assuming the ID is used in the URL
                    Method = HttpMethod.Put, // Change from POST to PUT
                    Headers =
                            {
                                { "X-Version", "1" },
                                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
                            },
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler)
                {
                };
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiHomeContentCoreactivityResponse>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<ActionResult> HomeCoreImageSave(HttpPostedFileBase file, HomeContentCoreactivityimage Homecontentcoreimages, string ExistingImageCore)
        {

            string HomeContentCoreImageurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_H_COREIMG_RECID = Session["PS_H_COREIMG_RECID"].ToString();
            string PS_H_COREIMG_ACCESSID = Session["PS_H_COREIMG_ACCESSID"].ToString();
            string PS_H_COREIMG_PAGENAME = Session["PS_H_COREIMG_PAGENAME"].ToString();
            string PS_H_COREIMG_CONTENTTYPE = Session["PS_H_COREIMG_CONTENTTYPE"].ToString();
            string PS_H_COREIMG_PARENT = Session["PS_H_COREIMG_PARENT"].ToString();
            string PS_H_COREIMG_NAME = Session["PS_H_COREIMG_NAME"].ToString();
            string PS_H_COREIMG_ID = Session["PS_H_COREIMG_ID"].ToString();
            string PS_H_COREIMG_LASTDATETIME = Session["PS_H_COREIMG_LASTDATETIME"].ToString();
            string PS_H_COREIMG_TYPE = Session["PS_H_CORE_TYPE"].ToString();


            string base64Image;

            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    // Convert the uploaded image to Base64
                    byte[] fileBytes = new byte[file.ContentLength];
                    file.InputStream.Read(fileBytes, 0, file.ContentLength);
                    base64Image = Convert.ToBase64String(fileBytes);
                    // Update session with new image
                    Session["Core_Image"] = base64Image;
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error processing the file: {ex.Message}";
                    return RedirectToAction("HomeCore", "TemplateLayout" , new { section = "Core Section" }); // Redirect to the original page
                }
            }
            else if (!string.IsNullOrEmpty(ExistingImageCore))
            {
                // Use the existing image
                base64Image = ExistingImageCore;

                // Update the session with the existing image (to ensure it's consistent in the session)
                Session["Core_Image"] = base64Image;
            }
            else
            {
                TempData["ErrorMessage"] = "No file selected or existing image provided.";
                return RedirectToAction("HomeCore", "TemplateLayout" , new { section = "Core Section" }); // Redirect to the original page
            }

            try
            {

                // Construct the JSON payload
                var content = $@"{{
                        ""pS_RECID"": ""{PS_H_COREIMG_RECID}"",
                        ""pS_ACCESSID"": ""{PS_H_COREIMG_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_H_COREIMG_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_H_COREIMG_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_H_COREIMG_PARENT}"",
                        ""pS_NAME"": ""{PS_H_COREIMG_NAME}"",
                        ""pS_ID"": ""{PS_H_COREIMG_ID}"",
                        ""pS_VALUES"": ""{base64Image}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_H_COREIMG_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{HomeContentCoreImageurl}"), // Assuming the ID is used in the URL
                    Method = HttpMethod.Put, // Change from POST to PUT
                    Headers =
                            {
                                { "X-Version", "1" },
                                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
                            },
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler)
                {
                };
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiHomeContentCoreactivityImageResponse>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("HomeCore", "TemplateLayout", new { section = "Core Section" });
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
            return RedirectToAction("Index", "Home");



        }
        
        public async Task<ActionResult> HomeSkill(string section)
        {

            Session["PS_H_HERO_TYPE_TEXT"] = null;
            Session["PS_H_CORE_TYPE"] = null;
            Session["PS_HT_ABOUT_TYPE"] = null;
            Session["PS_H_CONTACT_TYPE"] = null;
            Session["PS_H_VIDEO_TYPE"] = null;

            if (section == "Skill Section")
            {

                Session["ContentName"] = section;
                string Weburl = ConfigurationManager.AppSettings["GetApi"];
                string AuthKey = ConfigurationManager.AppSettings["Authkey"];

                string APIKey = ConfigurationManager.AppSettings["APIKey"];



                List<HomeContentSkillText> SkillTextList = new List<HomeContentSkillText>();
                string strparams = "strACCESSID=" + "PS002" + "&strUNICID=" + "PS002_Home_Skill" + "&strGroupID=" + "SkillSection";
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
                                Session["PS_SKILL_RECID"] = Skilltext.Data[1].PS_RECID;
                                Session["PS_SKILLIMG_RECID"] = Skilltext.Data[0].PS_RECID;
                                Session["PS_SKILL_ACCESSID"] = Skilltext.Data[1].PS_ACCESSID;
                                Session["PS_SKILLIMG_ACCESSID"] = Skilltext.Data[0].PS_ACCESSID;
                                Session["PS_SKILL_PAGENAME"] = Skilltext.Data[1].PS_PAGENAME;
                                Session["PS_SKILLIMG_PAGENAME"] = Skilltext.Data[0].PS_PAGENAME;
                                Session["PS_SKILL_CONTENTTYPE"] = Skilltext.Data[1].PS_CONTENTTYPE;
                                Session["PS_SKILLIMG_CONTENTTYPE"] = Skilltext.Data[0].PS_CONTENTTYPE;
                                Session["PS_SKILL_PARENT"] = Skilltext.Data[1].PS_PARENT;
                                Session["PS_SKILLIMG_PARENT"] = Skilltext.Data[0].PS_PARENT;
                                Session["PS_SKILL_NAME"] = Skilltext.Data[1].PS_NAME;
                                Session["PS_SKILLIMG_NAME"] = Skilltext.Data[0].PS_NAME;
                                Session["PS_SKILL_ID"] = Skilltext.Data[1].PS_ID;
                                Session["PS_SKILLIMG_ID"] = Skilltext.Data[0].PS_ID;
                                Session["PS_SKILL_LASTDATETIME"] = Skilltext.Data[1].PS_LASTDATETIME;
                                Session["PS_SKILLIMG_LASTDATETIME"] = Skilltext.Data[0].PS_LASTDATETIME;
                                Session["PS_SKILL_TYPE"] = Skilltext.Data[1].PS_TYPE;
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

                return View("HomePageHeroSection");
            }
            return RedirectToAction("HomeMainContent", "RecentTickets");
        }

        [HttpPost]
        public async Task<ActionResult> HomeSkillText(List<string> psValues, HomeContentSkillText Objskilltext, string Form)
        {
            string HomeContentskilltexturl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_SKILL_RECID = Session["PS_SKILL_RECID"].ToString();
            string PS_SKILL_ACCESSID = Session["PS_SKILL_ACCESSID"].ToString();
            string PS_SKILL_PAGENAME = Session["PS_SKILL_PAGENAME"].ToString();
            string PS_SKILL_CONTENTTYPE = Session["PS_SKILL_CONTENTTYPE"].ToString();
            string PS_SKILL_PARENT = Session["PS_SKILL_PARENT"].ToString();
            string PS_SKILL_NAME = Session["PS_SKILL_NAME"].ToString();
            string PS_SKILL_ID = Session["PS_SKILL_ID"].ToString();
            string PS_SKILL_LASTDATETIME = Session["PS_SKILL_LASTDATETIME"].ToString();
            string PS_SKILL_TYPE = Session["PS_SKILL_TYPE"].ToString();

            try
            {


                var escapedPsValues = psValues.Select(value =>
      value.Replace("\\", "\\\\")  // Escape backslashes
           .Replace("\"", "\\\"")  // Escape quotes
           .Replace("\n", "\\n")   // Escape newlines
           .Replace("\r", "\\r")); // Escape carriage returns

                // Create the desired format for pS_VALUES as a string
                string wrappedSerializedPsValues = "[" + string.Join(",", escapedPsValues.Select(value => $"\"{value}\"")) + "]";

                // Escape the quotes properly for a string value
                string escapedWrappedSerializedPsValues = wrappedSerializedPsValues.Replace("\"", "\\\"");

                // Construct the JSON payload
                var content = $@"{{
                        ""pS_RECID"": ""{PS_SKILL_RECID}"",
                        ""pS_ACCESSID"": ""{PS_SKILL_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_SKILL_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_SKILL_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_SKILL_PARENT}"",
                        ""pS_NAME"": ""{PS_SKILL_NAME}"",
                        ""pS_ID"": ""{PS_SKILL_ID}"",
                        ""pS_VALUES"": ""{escapedWrappedSerializedPsValues}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_SKILL_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{HomeContentskilltexturl}"), // Assuming the ID is used in the URL
                    Method = HttpMethod.Put, // Change from POST to PUT
                    Headers =
                            {
                                { "X-Version", "1" },
                                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
                            },
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler)
                {
                };
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiHomeSkillTextResponse>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<ActionResult> HomeSkillImage(HttpPostedFileBase file, HomeSkillimage Homeskillimages, string ExistingImageSkill)
        {

            string HomeSkillImageUpdateurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_SKILLIMG_RECID = Session["PS_SKILLIMG_RECID"].ToString();
            string PS_SKILLIMG_ACCESSID = Session["PS_SKILLIMG_ACCESSID"].ToString();
            string PS_SKILLIMG_PAGENAME = Session["PS_SKILLIMG_PAGENAME"].ToString();
            string PS_SKILLIMG_CONTENTTYPE = Session["PS_SKILLIMG_CONTENTTYPE"].ToString();
            string PS_SKILLIMG_PARENT = Session["PS_SKILLIMG_PARENT"].ToString();
            string PS_SKILLIMG_NAME = Session["PS_SKILLIMG_NAME"].ToString();
            string PS_SKILLIMG_ID = Session["PS_SKILLIMG_ID"].ToString();
            string PS_SKILLIMG_LASTDATETIME = Session["PS_SKILLIMG_LASTDATETIME"].ToString();
            string PS_SKILLIMG_TYPE = Session["PS_SKILL_TYPE"].ToString();


            string base64Image;


            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    // Convert the uploaded image to Base64
                    byte[] fileBytes = new byte[file.ContentLength];
                    file.InputStream.Read(fileBytes, 0, file.ContentLength);
                    base64Image = Convert.ToBase64String(fileBytes);

                    // Update session with new image
                    Session["Skill_Image"] = base64Image;

                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error processing the file: {ex.Message}";
                    return RedirectToAction("HomeSkill", "TemplateLayout" , new { section = "Skill Section" }); // Redirect to the original page
                }
            }
            else if (!string.IsNullOrEmpty(ExistingImageSkill))
            {
                // Use the existing image
                base64Image = ExistingImageSkill;

                // Update the session with the existing image (to ensure it's consistent in the session)
                Session["Skill_Image"] = base64Image;

            }
            else
            {
                TempData["ErrorMessage"] = "No file selected or existing image provided.";
                return RedirectToAction("HomeSkill", "TemplateLayout" , new { section = "Skill Section" }); // Redirect to the original page
            }

            try
            {

                // Construct the JSON payload
                var content = $@"{{
                        ""pS_RECID"": ""{PS_SKILLIMG_RECID}"",
                        ""pS_ACCESSID"": ""{PS_SKILLIMG_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_SKILLIMG_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_SKILLIMG_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_SKILLIMG_PARENT}"",
                        ""pS_NAME"": ""{PS_SKILLIMG_NAME}"",
                        ""pS_ID"": ""{PS_SKILLIMG_ID}"",
                        ""pS_VALUES"": ""{base64Image}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_SKILLIMG_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{HomeSkillImageUpdateurl}"), // Assuming the ID is used in the URL
                    Method = HttpMethod.Put, // Change from POST to PUT
                    Headers =
                            {
                                { "X-Version", "1" },
                                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
                            },
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler)
                {
                };
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiHomeSkillImageResponse>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("HomeSkill", "TemplateLayout",new { section = "Skill Section"});
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
            return RedirectToAction("Index", "Home");



        }

        public async Task<ActionResult> HomeContact(string section)
        {

            Session["PS_H_HERO_TYPE_TEXT"] = null;
            Session["PS_H_CORE_TYPE"] = null;
            Session["PS_SKILL_TYPE"] = null;
            Session["PS_HT_ABOUT_TYPE"] = null;
            Session["PS_H_VIDEO_TYPE"] = null;
            if (section == "Contact Us")
            {
                Session["ContentName"] = section;

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

                                ApiHomeContactus Contactcontent = JsonConvert.DeserializeObject<ApiHomeContactus>(jsonString);
                                Session["PS_H_CONTACT_RECID"] = Contactcontent.Data[0].PS_RECID;
                                Session["PS_H_CONTACT_ACCESSID"] = Contactcontent.Data[0].PS_ACCESSID;
                                Session["PS_H_CONTACT_PAGENAME"] = Contactcontent.Data[0].PS_PAGENAME;
                                Session["PS_H_CONTACT_CONTENTTYPE"] = Contactcontent.Data[0].PS_CONTENTTYPE;
                                Session["PS_H_CONTACT_PARENT"] = Contactcontent.Data[0].PS_PARENT;
                                Session["PS_H_CONTACT_NAME"] = Contactcontent.Data[0].PS_NAME;
                                Session["PS_H_CONTACT_ID"] = Contactcontent.Data[0].PS_ID;
                                Session["PS_H_CONTACT_LASTDATETIME"] = Contactcontent.Data[0].PS_LASTDATETIME;
                                Session["PS_H_CONTACT_TYPE"] = Contactcontent.Data[0].PS_TYPE;
                                //mainMenuList = rootObjects.Data;
                                List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(Contactcontent.Data[0].PS_VALUES);
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
                return View("HomePageHeroSection");
            }
            return RedirectToAction("HomeMainContent","RecentTickets");
        }

        [HttpPost]
        public async Task<ActionResult> HomeContactSave(List<string> psValues, Homecontactus Objcontact, string Form)
        {
            string Homecontactputurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_H_CONTACT_RECID = Session["PS_H_CONTACT_RECID"].ToString();
            string PS_H_CONTACT_ACCESSID = Session["PS_H_CONTACT_ACCESSID"].ToString();
            string PS_H_CONTACT_PAGENAME = Session["PS_H_CONTACT_PAGENAME"].ToString();
            string PS_H_CONTACT_CONTENTTYPE = Session["PS_H_CONTACT_CONTENTTYPE"].ToString();
            string PS_H_CONTACT_PARENT = Session["PS_H_CONTACT_PARENT"].ToString();
            string PS_H_CONTACT_NAME = Session["PS_H_CONTACT_NAME"].ToString();
            string PS_H_CONTACT_ID = Session["PS_H_CONTACT_ID"].ToString();
            string PS_H_CONTACT_LASTDATETIME = Session["PS_H_CONTACT_LASTDATETIME"].ToString();
            string PS_H_CONTACT_TYPE = Session["PS_H_CONTACT_TYPE"].ToString();

            try
            {


                var escapedPsValues = psValues.Select(value =>
      value.Replace("\\", "\\\\")  // Escape backslashes
           .Replace("\"", "\\\"")  // Escape quotes
           .Replace("\n", "\\n")   // Escape newlines
           .Replace("\r", "\\r")); // Escape carriage returns

                // Create the desired format for pS_VALUES as a string
                string wrappedSerializedPsValues = "[" + string.Join(",", escapedPsValues.Select(value => $"\"{value}\"")) + "]";

                // Escape the quotes properly for a string value
                string escapedWrappedSerializedPsValues = wrappedSerializedPsValues.Replace("\"", "\\\"");

                // Construct the JSON payload
                var content = $@"{{
                        ""pS_RECID"": ""{PS_H_CONTACT_RECID}"",
                        ""pS_ACCESSID"": ""{PS_H_CONTACT_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_H_CONTACT_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_H_CONTACT_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_H_CONTACT_PARENT}"",
                        ""pS_NAME"": ""{PS_H_CONTACT_NAME}"",
                        ""pS_ID"": ""{PS_H_CONTACT_ID}"",
                        ""pS_VALUES"": ""{escapedWrappedSerializedPsValues}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_H_CONTACT_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{Homecontactputurl}"), // Assuming the ID is used in the URL
                    Method = HttpMethod.Put, // Change from POST to PUT
                    Headers =
                            {
                                { "X-Version", "1" },
                                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
                            },
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler)
                {
                };
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiHomeContactus>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> HeroSectionVideo(string section)
        {
            Session["PS_H_HERO_TYPE_TEXT"] = null;
            Session["PS_H_CORE_TYPE"] = null;
            Session["PS_SKILL_TYPE"] = null;
            Session["PS_HTYP_ABOUT_TYPE"] = null;
            Session["PS_H_CONTACT_TE"] = null;

            if (section == "Hero Section Video")
            {
                Session["ContentName"] = section;

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
                                //mainMenuList = rootObjects.Data;
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
                return View("HomePageHeroSection");
            }
            return RedirectToAction("HomeMainContent", "RecentTickets");

        }

        [HttpPost]
        public async Task<ActionResult> HeroSectionVideoSave(List<string> psValues, HeroVideolink Objcontact, string Form)
        {
            string HeroVideoputurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_H_VIDEO_RECID = Session["PS_H_VIDEO_RECID"].ToString();
            string PS_H_VIDEO_ACCESSID = Session["PS_H_VIDEO_ACCESSID"].ToString();
            string PS_H_VIDEO_PAGENAME = Session["PS_H_VIDEO_PAGENAME"].ToString();
            string PS_H_VIDEO_CONTENTTYPE = Session["PS_H_VIDEO_CONTENTTYPE"].ToString();
            string PS_H_VIDEO_PARENT = Session["PS_H_VIDEO_PARENT"].ToString();
            string PS_H_VIDEO_NAME = Session["PS_H_VIDEO_NAME"].ToString();
            string PS_H_VIDEO_ID = Session["PS_H_VIDEO_ID"].ToString();
            string PS_H_VIDEO_LASTDATETIME = Session["PS_H_VIDEO_LASTDATETIME"].ToString();
            string PS_H_VIDEO_TYPE = Session["PS_H_VIDEO_TYPE"].ToString();

            try
            {


                var escapedPsValues = psValues.Select(value =>
      value.Replace("\\", "\\\\")  // Escape backslashes
           .Replace("\"", "\\\"")  // Escape quotes
           .Replace("\n", "\\n")   // Escape newlines
           .Replace("\r", "\\r")); // Escape carriage returns

                // Create the desired format for pS_VALUES as a string
                string wrappedSerializedPsValues = "[" + string.Join(",", escapedPsValues.Select(value => $"\"{value}\"")) + "]";

                // Escape the quotes properly for a string value
                string escapedWrappedSerializedPsValues = wrappedSerializedPsValues.Replace("\"", "\\\"");

                // Construct the JSON payload
                var content = $@"{{
                        ""pS_RECID"": ""{PS_H_VIDEO_RECID}"",
                        ""pS_ACCESSID"": ""{PS_H_VIDEO_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_H_VIDEO_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_H_VIDEO_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_H_VIDEO_PARENT}"",
                        ""pS_NAME"": ""{PS_H_VIDEO_NAME}"",
                        ""pS_ID"": ""{PS_H_VIDEO_ID}"",
                        ""pS_VALUES"": ""{escapedWrappedSerializedPsValues}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_H_VIDEO_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{HeroVideoputurl}"), // Assuming the ID is used in the URL
                    Method = HttpMethod.Put, // Change from POST to PUT
                    Headers =
                            {
                                { "X-Version", "1" },
                                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
                            },
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler)
                {
                };
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiHeroVideoLink>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
            return RedirectToAction("Index", "Home");
        }





    }
}