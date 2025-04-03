using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using PSS_CMS.Models;

namespace PSS_CMS.Controllers
{
    public class RecentLearninganddevelopmentController : Controller
    {
        // GET: RecentLearninganddevelopment
        public async Task<ActionResult> OLearningAndDevelopmentMain()
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
                                Session["PS_O_LearningAndDevelopmentMAIN_RECID"] = Learninganddevelopmentmain.Data[0].PS_RECID;
                                Session["PS_O_LearningAndDevelopmentMAINIMG_RECID"] = Learninganddevelopmentmain.Data[1].PS_RECID;

                                Session["PS_O_LearningAndDevelopmentMAIN_ACCESSID"] = Learninganddevelopmentmain.Data[0].PS_ACCESSID;
                                Session["PS_O_LearningAndDevelopmentMAINIMG_ACCESSID"] = Learninganddevelopmentmain.Data[1].PS_ACCESSID;

                                Session["PS_O_LearningAndDevelopmentMAIN_PAGENAME"] = Learninganddevelopmentmain.Data[0].PS_PAGENAME;
                                Session["PS_O_LearningAndDevelopmentMAINIMG_PAGENAME"] = Learninganddevelopmentmain.Data[1].PS_PAGENAME;

                                Session["PS_O_LearningAndDevelopmentMAIN_CONTENTTYPE"] = Learninganddevelopmentmain.Data[0].PS_CONTENTTYPE;
                                Session["PS_O_LearningAndDevelopmentMAINIMG_CONTENTTYPE"] = Learninganddevelopmentmain.Data[1].PS_CONTENTTYPE;

                                Session["PS_O_LearningAndDevelopmentMAIN_PARENT"] = Learninganddevelopmentmain.Data[0].PS_PARENT;
                                Session["PS_O_LearningAndDevelopmentMAINIMG_PARENT"] = Learninganddevelopmentmain.Data[1].PS_PARENT;
                            
                                Session["PS_O_LearningAndDevelopmentMAIN_NAME"] = Learninganddevelopmentmain.Data[0].PS_NAME;
                                Session["PS_O_LearningAndDevelopmentMAINIMG_NAME"] = Learninganddevelopmentmain.Data[1].PS_NAME;

                                Session["PS_O_LearningAndDevelopmentMAIN_ID"] = Learninganddevelopmentmain.Data[0].PS_ID;
                                Session["PS_O_LearningAndDevelopmentMAINIMG_ID"] = Learninganddevelopmentmain.Data[1].PS_ID;

                                Session["PS_O_LearningAndDevelopmentMAIN_LASTDATETIME"] = Learninganddevelopmentmain.Data[0].PS_LASTDATETIME;
                                Session["PS_O_LearningAndDevelopmentMAINIMG_LASTDATETIME"] = Learninganddevelopmentmain.Data[1].PS_LASTDATETIME;

                                Session["PS_O_LearningAndDevelopmentMAIN_TYPE"] = Learninganddevelopmentmain.Data[0].PS_TYPE;


                                List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(Learninganddevelopmentmain.Data[0].PS_VALUES);
                                string psValues = Learninganddevelopmentmain.Data[1].PS_VALUES.ToString();
                                string base64Image = psValues;


                                if (!string.IsNullOrEmpty(base64Image))
                                {

                                    string mimeType = GetImageMimeType(base64Image);

                                    ViewBag.LEARNINGANDDEVELOPMENTMAINIMAGE = base64Image;
                                    ViewBag.MIMEType = mimeType;
                                    Session["learninganddevelopment_content_Images_O"] = ViewBag.LEARNINGANDDEVELOPMENTMAINIMAGE;
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
            await OLearninganddevelopmentAboutUs();
                return View();
           
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
        public async Task<ActionResult> OLearningAndDevelopmentMainTextSave(List<string> psValues, LearningandDevelopment Objlearninganddevelopmentmain, string Form)
        {
            string HomeContentMainurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_O_LearningAndDevelopmentMAIN_RECID = Session["PS_O_LearningAndDevelopmentMAIN_RECID"].ToString();
            string PS_O_LearningAndDevelopmentMAIN_ACCESSID = Session["PS_O_LearningAndDevelopmentMAIN_ACCESSID"].ToString();
            string PS_O_LearningAndDevelopmentMAIN_PAGENAME = Session["PS_O_LearningAndDevelopmentMAIN_PAGENAME"].ToString();
            string PS_O_LearningAndDevelopmentMAIN_CONTENTTYPE = Session["PS_O_LearningAndDevelopmentMAIN_CONTENTTYPE"].ToString();
            string PS_O_LearningAndDevelopmentMAIN_PARENT = Session["PS_O_LearningAndDevelopmentMAIN_PARENT"].ToString();
            string PS_O_LearningAndDevelopmentMAIN_NAME = Session["PS_O_LearningAndDevelopmentMAIN_NAME"].ToString();
            string PS_O_LearningAndDevelopmentMAIN_ID = Session["PS_O_LearningAndDevelopmentMAIN_ID"].ToString();
            string PS_O_LearningAndDevelopmentMAIN_LASTDATETIME = Session["PS_O_LearningAndDevelopmentMAIN_LASTDATETIME"].ToString();
            string PS_O_LearningAndDevelopmentMAIN_TYPE = Session["PS_O_LearningAndDevelopmentMAIN_TYPE"].ToString();


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
                        ""pS_RECID"": ""{PS_O_LearningAndDevelopmentMAIN_RECID}"",
                        ""pS_ACCESSID"": ""{PS_O_LearningAndDevelopmentMAIN_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_O_LearningAndDevelopmentMAIN_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_O_LearningAndDevelopmentMAIN_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_O_LearningAndDevelopmentMAIN_PARENT}"",
                        ""pS_NAME"": ""{PS_O_LearningAndDevelopmentMAIN_NAME}"",
                        ""pS_ID"": ""{PS_O_LearningAndDevelopmentMAIN_ID}"",
                        ""pS_VALUES"": ""{escapedWrappedSerializedPsValues}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_O_LearningAndDevelopmentMAIN_TYPE}""
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
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseLearningAndDevelopmentMain>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("OLearningAndDevelopmentMain", "RecentLearninganddevelopment");
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
        public async Task<ActionResult> OLearningAndDevelopmentMainImageSave(HttpPostedFileBase file, LearningandDevelopmentImages Objlearninganddevelopmentimage, string ExistingImageMain)
        {
            string LearninganddevelopmentMainImageurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_O_LearningAndDevelopmentMAINIMG_RECID = Session["PS_O_LearningAndDevelopmentMAINIMG_RECID"].ToString();
            string PS_O_LearningAndDevelopmentMAINIMG_ACCESSID = Session["PS_O_LearningAndDevelopmentMAINIMG_ACCESSID"].ToString();
            string PS_O_LearningAndDevelopmentMAINIMG_PAGENAME = Session["PS_O_LearningAndDevelopmentMAINIMG_PAGENAME"].ToString();
            string PS_O_LearningAndDevelopmentMAINIMG_CONTENTTYPE = Session["PS_O_LearningAndDevelopmentMAINIMG_CONTENTTYPE"].ToString();
            string PS_O_LearningAndDevelopmentMAINIMG_PARENT = Session["PS_O_LearningAndDevelopmentMAINIMG_PARENT"].ToString();
            string PS_O_LearningAndDevelopmentMAINIMG_NAME = Session["PS_O_LearningAndDevelopmentMAINIMG_NAME"].ToString();
            string PS_O_LearningAndDevelopmentMAINIMG_ID = Session["PS_O_LearningAndDevelopmentMAINIMG_ID"].ToString();
            string PS_O_LearningAndDevelopmentMAINIMG_LASTDATETIME = Session["PS_O_LearningAndDevelopmentMAINIMG_LASTDATETIME"].ToString();
            string PS_O_LearningAndDevelopmentMAIN_TYPE = Session["PS_O_LearningAndDevelopmentMAIN_TYPE"].ToString();


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
                    Session["learninganddevelopment_content_Images"] = base64Image;
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
                Session["learninganddevelopment_content_Images"] = base64Image;
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
                        ""pS_RECID"": ""{PS_O_LearningAndDevelopmentMAINIMG_RECID}"",
                        ""pS_ACCESSID"": ""{PS_O_LearningAndDevelopmentMAINIMG_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_O_LearningAndDevelopmentMAINIMG_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_O_LearningAndDevelopmentMAINIMG_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_O_LearningAndDevelopmentMAINIMG_PARENT}"",
                        ""pS_NAME"": ""{PS_O_LearningAndDevelopmentMAINIMG_NAME}"",
                        ""pS_ID"": ""{PS_O_LearningAndDevelopmentMAINIMG_ID}"",
                        ""pS_VALUES"": ""{base64Image}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_O_LearningAndDevelopmentMAIN_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{LearninganddevelopmentMainImageurl}"), // Assuming the ID is used in the URL
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
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseLearningAndDevelopmentImageResponse>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("OLearningAndDevelopmentMain", "RecentLearninganddevelopment", new { section = "LearningAndDevelopment Main Section" });
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

        public async Task<ActionResult> OLearninganddevelopmentAboutUs()
        {
           
           

                string Weburl = ConfigurationManager.AppSettings["GetApi"];
                string AuthKey = ConfigurationManager.AppSettings["Authkey"];

                string APIKey = ConfigurationManager.AppSettings["APIKey"];



                List<LearningandDevelopmentaboutus> aboutuscontentList = new List<LearningandDevelopmentaboutus>();
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
                                Session["PS_LO_ABOUT_RECID"] = learninganddevelopmentmaincontentaboutus.Data[0].PS_RECID;
                                Session["PS_LO_ABOUT_ACCESSID"] = learninganddevelopmentmaincontentaboutus.Data[0].PS_ACCESSID;
                                Session["PS_LO_ABOUT_PAGENAME"] = learninganddevelopmentmaincontentaboutus.Data[0].PS_PAGENAME;
                                Session["PS_LO_ABOUT_CONTENTTYPE"] = learninganddevelopmentmaincontentaboutus.Data[0].PS_CONTENTTYPE;
                                Session["PS_LO_ABOUT_PARENT"] = learninganddevelopmentmaincontentaboutus.Data[0].PS_PARENT;
                                Session["PS_LO_ABOUT_NAME"] = learninganddevelopmentmaincontentaboutus.Data[0].PS_NAME;
                                Session["PS_LO_ABOUT_ID"] = learninganddevelopmentmaincontentaboutus.Data[0].PS_ID;
                                Session["PS_LO_ABOUT_LASTDATETIME"] = learninganddevelopmentmaincontentaboutus.Data[0].PS_LASTDATETIME;
                                Session["PS_LO_ABOUT_TYPE"] = learninganddevelopmentmaincontentaboutus.Data[0].PS_TYPE;
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
            await OLearningandDevelopmentWhyUs();
                return View();
           

        }
        [HttpPost]
        public async Task<ActionResult> OLearninganddevelopmentAboutUsSave(List<string> psValues, LearningandDevelopmentaboutus Objaboutuslearninganddevelopment, string Form)
        {
            string LearninganddevelopmentContentAboutUSurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_LO_ABOUT_RECID = Session["PS_LO_ABOUT_RECID"].ToString();
            string PS_LO_ABOUT_ACCESSID = Session["PS_LO_ABOUT_ACCESSID"].ToString();
            string PS_LO_ABOUT_PAGENAME = Session["PS_LO_ABOUT_PAGENAME"].ToString();
            string PS_LO_ABOUT_CONTENTTYPE = Session["PS_LO_ABOUT_CONTENTTYPE"].ToString();
            string PS_LO_ABOUT_PARENT = Session["PS_LO_ABOUT_PARENT"].ToString();
            string PS_LO_ABOUT_NAME = Session["PS_LO_ABOUT_NAME"].ToString();
            string PS_LO_ABOUT_ID = Session["PS_LO_ABOUT_ID"].ToString();
            string PS_LO_ABOUT_LASTDATETIME = Session["PS_LO_ABOUT_LASTDATETIME"].ToString();
            string PS_LO_ABOUT_TYPE = Session["PS_LO_ABOUT_TYPE"].ToString();

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
                        ""pS_RECID"": ""{PS_LO_ABOUT_RECID}"",
                        ""pS_ACCESSID"": ""{PS_LO_ABOUT_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_LO_ABOUT_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_LO_ABOUT_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_LO_ABOUT_PARENT}"",
                        ""pS_NAME"": ""{PS_LO_ABOUT_NAME}"",
                        ""pS_ID"": ""{PS_LO_ABOUT_ID}"",
                        ""pS_VALUES"": ""{escapedWrappedSerializedPsValues}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_LO_ABOUT_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{LearninganddevelopmentContentAboutUSurl}"), // Assuming the ID is used in the URL
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
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseLearningAndDevelopmentMainaboutus>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("OLearningAndDevelopmentMain", "RecentLearninganddevelopment");
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

        public async Task<ActionResult> OLearningandDevelopmentWhyUs()
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
                                Session["PS_LTO_LearningAndDevelopmentWHYUS_RECID"] = Learninganddevelopmentwhyus.Data[0].PS_RECID;
                                Session["PS_LTO_LearningAndDevelopmentWHYUSIMG_RECID"] = Learninganddevelopmentwhyus.Data[1].PS_RECID;
                                Session["PS_LTO_LearningAndDevelopmentWHYUS_ACCESSID"] = Learninganddevelopmentwhyus.Data[0].PS_ACCESSID;
                                Session["PS_LTO_LearningAndDevelopmentWHYUSIMG_ACCESSID"] = Learninganddevelopmentwhyus.Data[1].PS_ACCESSID;
                                Session["PS_LTO_LearningAndDevelopmentWHYUS_PAGENAME"] = Learninganddevelopmentwhyus.Data[0].PS_PAGENAME;
                                Session["PS_LTO_LearningAndDevelopmentWHYUSIMG_PAGENAME"] = Learninganddevelopmentwhyus.Data[1].PS_PAGENAME;
                                Session["PS_LTO_LearningAndDevelopmentWHYUS_CONTENTTYPE"] = Learninganddevelopmentwhyus.Data[0].PS_CONTENTTYPE;
                                Session["PS_LTO_LearningAndDevelopmentWHYUSIMG_CONTENTTYPE"] = Learninganddevelopmentwhyus.Data[1].PS_CONTENTTYPE;
                                Session["PS_LTO_LearningAndDevelopmentWHYUS_PARENT"] = Learninganddevelopmentwhyus.Data[0].PS_PARENT;
                                Session["PS_LTO_LearningAndDevelopmentWHYUSIMG_PARENT"] = Learninganddevelopmentwhyus.Data[1].PS_PARENT;
                                Session["PS_LTO_LearningAndDevelopmentWHYUS_NAME"] = Learninganddevelopmentwhyus.Data[0].PS_NAME;
                                Session["PS_LTO_LearningAndDevelopmentWHYUSIMG_NAME"] = Learninganddevelopmentwhyus.Data[1].PS_NAME;
                                Session["PS_LTO_LearningAndDevelopmentWHYUS_ID"] = Learninganddevelopmentwhyus.Data[0].PS_ID;
                                Session["PS_LTO_LearningAndDevelopmentWHYUSIMG_ID"] = Learninganddevelopmentwhyus.Data[1].PS_ID;
                                Session["PS_LTO_LearningAndDevelopmentWHYUS_LASTDATETIME"] = Learninganddevelopmentwhyus.Data[0].PS_LASTDATETIME;
                                Session["PS_LTO_LearningAndDevelopmentWHYUSIMG_LASTDATETIME"] = Learninganddevelopmentwhyus.Data[1].PS_LASTDATETIME;
                                Session["PS_LTO_LearningAndDevelopmentWHYUS_TYPE"] = Learninganddevelopmentwhyus.Data[0].PS_TYPE;


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
            await OLearningAndDevelopmentSkillsSection();
                return View();
           
        }

        [HttpPost]
        public async Task<ActionResult> OLearningAndDevelopmentWhyUsTextSave(List<string> psValues, LearningandDevelopmentWhyUSText Objlearninganddevelopmentwhyustext, string Form)
        {
            string LearninganddevelopmentContentwhyusurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_LTO_LearningAndDevelopmentWHYUS_RECID = Session["PS_LTO_LearningAndDevelopmentWHYUS_RECID"].ToString();
            string PS_LTO_LearningAndDevelopmentWHYUS_ACCESSID = Session["PS_LTO_LearningAndDevelopmentWHYUS_ACCESSID"].ToString();
            string PS_LTO_LearningAndDevelopmentWHYUS_PAGENAME = Session["PS_LTO_LearningAndDevelopmentWHYUS_PAGENAME"].ToString();
            string PS_LTO_LearningAndDevelopmentWHYUS_CONTENTTYPE = Session["PS_LTO_LearningAndDevelopmentWHYUS_CONTENTTYPE"].ToString();
            string PS_LTO_LearningAndDevelopmentWHYUS_PARENT = Session["PS_LTO_LearningAndDevelopmentWHYUS_PARENT"].ToString();
            string PS_LTO_LearningAndDevelopmentWHYUS_NAME = Session["PS_LTO_LearningAndDevelopmentWHYUS_NAME"].ToString();
            string PS_LTO_LearningAndDevelopmentWHYUS_ID = Session["PS_LTO_LearningAndDevelopmentWHYUS_ID"].ToString();
            string PS_LTO_LearningAndDevelopmentWHYUS_LASTDATETIME = Session["PS_LTO_LearningAndDevelopmentWHYUS_LASTDATETIME"].ToString();
            string PS_LTO_LearningAndDevelopmentWHYUS_TYPE = Session["PS_LTO_LearningAndDevelopmentWHYUS_TYPE"].ToString();


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
                        ""pS_RECID"": ""{PS_LTO_LearningAndDevelopmentWHYUS_RECID}"",
                        ""pS_ACCESSID"": ""{PS_LTO_LearningAndDevelopmentWHYUS_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_LTO_LearningAndDevelopmentWHYUS_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_LTO_LearningAndDevelopmentWHYUS_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_LTO_LearningAndDevelopmentWHYUS_PARENT}"",
                        ""pS_NAME"": ""{PS_LTO_LearningAndDevelopmentWHYUS_NAME}"",
                        ""pS_ID"": ""{PS_LTO_LearningAndDevelopmentWHYUS_ID}"",
                        ""pS_VALUES"": ""{escapedWrappedSerializedPsValues}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_LTO_LearningAndDevelopmentWHYUS_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{LearninganddevelopmentContentwhyusurl}"), // Assuming the ID is used in the URL
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
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseLearningAndDevelopmentWhyUSText>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("OLearningAndDevelopmentMain", "RecentLearninganddevelopment");
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
        public async Task<ActionResult> OLearningAndDevelopmentWhyUsImageSave(HttpPostedFileBase file, LearningandDevelopmentWhyUSImage Objlearninganddevelopmentwhyusimage, string ExistingImageMain)
        {
            string LearninganddevelopmentWhyusImageurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_LTO_LearningAndDevelopmentWHYUSIMG_RECID = Session["PS_LTO_LearningAndDevelopmentWHYUSIMG_RECID"].ToString();
            string PS_LTO_LearningAndDevelopmentWHYUSIMG_ACCESSID = Session["PS_LTO_LearningAndDevelopmentWHYUSIMG_ACCESSID"].ToString();
            string PS_LTO_LearningAndDevelopmentWHYUSIMG_PAGENAME = Session["PS_LTO_LearningAndDevelopmentWHYUSIMG_PAGENAME"].ToString();
            string PS_LTO_LearningAndDevelopmentWHYUSIMG_CONTENTTYPE = Session["PS_LTO_LearningAndDevelopmentWHYUSIMG_CONTENTTYPE"].ToString();
            string PS_LTO_LearningAndDevelopmentWHYUSIMG_PARENT = Session["PS_LTO_LearningAndDevelopmentWHYUSIMG_PARENT"].ToString();
            string PS_LTO_LearningAndDevelopmentWHYUSIMG_NAME = Session["PS_LTO_LearningAndDevelopmentWHYUSIMG_NAME"].ToString();
            string PS_LTO_LearningAndDevelopmentWHYUSIMG_ID = Session["PS_LTO_LearningAndDevelopmentWHYUSIMG_ID"].ToString();
            string PS_LTO_LearningAndDevelopmentWHYUSIMG_LASTDATETIME = Session["PS_LTO_LearningAndDevelopmentWHYUSIMG_LASTDATETIME"].ToString();
            string PS_LTO_LearningAndDevelopmentWHYUS_TYPE = Session["PS_LTO_LearningAndDevelopmentWHYUS_TYPE"].ToString();


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
                    Session["learninganddevelopment_whyus_Images"] = base64Image;
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
                Session["learninganddevelopment_whyus_Images"] = base64Image;
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
                        ""pS_RECID"": ""{PS_LTO_LearningAndDevelopmentWHYUSIMG_RECID}"",
                        ""pS_ACCESSID"": ""{PS_LTO_LearningAndDevelopmentWHYUSIMG_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_LTO_LearningAndDevelopmentWHYUSIMG_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_LTO_LearningAndDevelopmentWHYUSIMG_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_LTO_LearningAndDevelopmentWHYUSIMG_PARENT}"",
                        ""pS_NAME"": ""{PS_LTO_LearningAndDevelopmentWHYUSIMG_NAME}"",
                        ""pS_ID"": ""{PS_LTO_LearningAndDevelopmentWHYUSIMG_ID}"",
                        ""pS_VALUES"": ""{base64Image}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_LTO_LearningAndDevelopmentWHYUS_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{LearninganddevelopmentWhyusImageurl}"), // Assuming the ID is used in the URL
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
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseLearningAndDevelopmentWhyUSImage>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("OLearningAndDevelopmentMain", "RecentLearninganddevelopment", new { section = "LearningAndDevelopment WhyUS Section" });
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

        public async Task<ActionResult> OLearningAndDevelopmentSkillsSection()
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
                                Session["PS_LTO_LearningAndDevelopmentSKILL_RECID"] = Learninganddevelopmentskill.Data[0].PS_RECID;
                                Session["PS_LTO_LearningAndDevelopmentSKILLIMG_RECID"] = Learninganddevelopmentskill.Data[1].PS_RECID;
                                Session["PS_LTO_LearningAndDevelopmentSKILL_ACCESSID"] = Learninganddevelopmentskill.Data[0].PS_ACCESSID;
                                Session["PS_LTO_LearningAndDevelopmentSKILLIMG_ACCESSID"] = Learninganddevelopmentskill.Data[1].PS_ACCESSID;
                                Session["PS_LTO_LearningAndDevelopmentSKILL_PAGENAME"] = Learninganddevelopmentskill.Data[0].PS_PAGENAME;
                                Session["PS_LTO_LearningAndDevelopmentSKILLIMG_PAGENAME"] = Learninganddevelopmentskill.Data[1].PS_PAGENAME;
                                Session["PS_LTO_LearningAndDevelopmentSKILL_CONTENTTYPE"] = Learninganddevelopmentskill.Data[0].PS_CONTENTTYPE;
                                Session["PS_LTO_LearningAndDevelopmentSKILLIMG_CONTENTTYPE"] = Learninganddevelopmentskill.Data[1].PS_CONTENTTYPE;
                                Session["PS_LTO_LearningAndDevelopmentSKILL_PARENT"] = Learninganddevelopmentskill.Data[0].PS_PARENT;
                                Session["PS_LTO_LearningAndDevelopmentSKILLIMG_PARENT"] = Learninganddevelopmentskill.Data[1].PS_PARENT;
                                Session["PS_LTO_LearningAndDevelopmentSKILL_NAME"] = Learninganddevelopmentskill.Data[0].PS_NAME;
                                Session["PS_LTO_LearningAndDevelopmentSKILLIMG_NAME"] = Learninganddevelopmentskill.Data[1].PS_NAME;
                                Session["PS_LTO_LearningAndDevelopmentSKILL_ID"] = Learninganddevelopmentskill.Data[0].PS_ID;
                                Session["PS_LTO_LearningAndDevelopmentSKILLIMG_ID"] = Learninganddevelopmentskill.Data[1].PS_ID;
                                Session["PS_LTO_LearningAndDevelopmentSKILL_LASTDATETIME"] = Learninganddevelopmentskill.Data[0].PS_LASTDATETIME;
                                Session["PS_LTO_LearningAndDevelopmentSKILLIMG_LASTDATETIME"] = Learninganddevelopmentskill.Data[1].PS_LASTDATETIME;
                                Session["PS_LTO_LearningAndDevelopmentSKILL_TYPE"] = Learninganddevelopmentskill.Data[0].PS_TYPE;

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

        [HttpPost]
        public async Task<ActionResult> OLearningAndDevelopmentSkillTextSave(List<string> psValues, LearningandDevelopmentSkillsText Objlearninganddevelopmentskilltext, string Form)
        {
            string LearninganddevelopmentContentskillurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_LTO_LearningAndDevelopmentSKILL_RECID = Session["PS_LTO_LearningAndDevelopmentSKILL_RECID"].ToString();
            string PS_LTO_LearningAndDevelopmentSKILL_ACCESSID = Session["PS_LTO_LearningAndDevelopmentSKILL_ACCESSID"].ToString();
            string PS_LTO_LearningAndDevelopmentSKILL_PAGENAME = Session["PS_LTO_LearningAndDevelopmentSKILL_PAGENAME"].ToString();
            string PS_LTO_LearningAndDevelopmentSKILL_CONTENTTYPE = Session["PS_LTO_LearningAndDevelopmentSKILL_CONTENTTYPE"].ToString();
            string PS_LTO_LearningAndDevelopmentSKILL_PARENT = Session["PS_LTO_LearningAndDevelopmentSKILL_PARENT"].ToString();
            string PS_LTO_LearningAndDevelopmentSKILL_NAME = Session["PS_LTO_LearningAndDevelopmentSKILL_NAME"].ToString();
            string PS_LTO_LearningAndDevelopmentSKILL_ID = Session["PS_LTO_LearningAndDevelopmentSKILL_ID"].ToString();
            string PS_LTO_LearningAndDevelopmentSKILL_LASTDATETIME = Session["PS_LTO_LearningAndDevelopmentSKILL_LASTDATETIME"].ToString();
            string PS_LTO_LearningAndDevelopmentSKILL_TYPE = Session["PS_LTO_LearningAndDevelopmentSKILL_TYPE"].ToString();


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
                        ""pS_RECID"": ""{PS_LTO_LearningAndDevelopmentSKILL_RECID}"",
                        ""pS_ACCESSID"": ""{PS_LTO_LearningAndDevelopmentSKILL_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_LTO_LearningAndDevelopmentSKILL_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_LTO_LearningAndDevelopmentSKILL_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_LTO_LearningAndDevelopmentSKILL_PARENT}"",
                        ""pS_NAME"": ""{PS_LTO_LearningAndDevelopmentSKILL_NAME}"",
                        ""pS_ID"": ""{PS_LTO_LearningAndDevelopmentSKILL_ID}"",
                        ""pS_VALUES"": ""{escapedWrappedSerializedPsValues}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_LTO_LearningAndDevelopmentSKILL_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{LearninganddevelopmentContentskillurl}"), // Assuming the ID is used in the URL
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
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseLearningAndDevelopmentSkillText>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("OLearningAndDevelopmentMain", "RecentLearninganddevelopment");
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
        public async Task<ActionResult> OLearningAndDevelopmentSkillImageSave(HttpPostedFileBase file, LearningandDevelopmentSkillsImg ObjlearninganddevelopmentSkillimage, string ExistingImageMain)
        {
            string LearninganddevelopmentWhyusImageurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_LTO_LearningAndDevelopmentSKILLIMG_RECID = Session["PS_LTO_LearningAndDevelopmentSKILLIMG_RECID"].ToString();
            string PS_LTO_LearningAndDevelopmentSKILLIMG_ACCESSID = Session["PS_LTO_LearningAndDevelopmentSKILLIMG_ACCESSID"].ToString();
            string PS_LTO_LearningAndDevelopmentSKILLIMG_PAGENAME = Session["PS_LTO_LearningAndDevelopmentSKILLIMG_PAGENAME"].ToString();
            string PS_LTO_LearningAndDevelopmentSKILLIMG_CONTENTTYPE = Session["PS_LTO_LearningAndDevelopmentSKILLIMG_CONTENTTYPE"].ToString();
            string PS_LTO_LearningAndDevelopmentSKILLIMG_PARENT = Session["PS_LTO_LearningAndDevelopmentSKILLIMG_PARENT"].ToString();
            string PS_LTO_LearningAndDevelopmentSKILLIMG_NAME = Session["PS_LTO_LearningAndDevelopmentSKILLIMG_NAME"].ToString();
            string PS_LTO_LearningAndDevelopmentSKILLIMG_ID = Session["PS_LTO_LearningAndDevelopmentSKILLIMG_ID"].ToString();
            string PS_LTO_LearningAndDevelopmentSKILLIMG_LASTDATETIME = Session["PS_LTO_LearningAndDevelopmentSKILLIMG_LASTDATETIME"].ToString();
            string PS_LTO_LearningAndDevelopmentSKILL_TYPE = Session["PS_LTO_LearningAndDevelopmentSKILL_TYPE"].ToString();


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
                    Session["learninganddevelopment_skill_Images"] = base64Image;
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
                Session["learninganddevelopment_skill_Images"] = base64Image;
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
                        ""pS_RECID"": ""{PS_LTO_LearningAndDevelopmentSKILLIMG_RECID}"",
                        ""pS_ACCESSID"": ""{PS_LTO_LearningAndDevelopmentSKILLIMG_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_LTO_LearningAndDevelopmentSKILLIMG_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_LTO_LearningAndDevelopmentSKILLIMG_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_LTO_LearningAndDevelopmentSKILLIMG_PARENT}"",
                        ""pS_NAME"": ""{PS_LTO_LearningAndDevelopmentSKILLIMG_NAME}"",
                        ""pS_ID"": ""{PS_LTO_LearningAndDevelopmentSKILLIMG_ID}"",
                        ""pS_VALUES"": ""{base64Image}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_LTO_LearningAndDevelopmentSKILL_TYPE}""
                    }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{LearninganddevelopmentWhyusImageurl}"), // Assuming the ID is used in the URL
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
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseLearningAndDevelopmentSkillImg>(responseBody);
                    if (apiResponse.Status == "Y")
                    {
                        return RedirectToAction("OLearningAndDevelopmentMain", "RecentLearninganddevelopment", new { section = "LearningAndDevelopment Skill Section" });
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