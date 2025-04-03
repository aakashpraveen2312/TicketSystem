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
    public class TemplateCanteenController : Controller
    {
        // GET: TemplateCanteen
        public async Task<ActionResult> CanteenMain(string section)
        {
            if (section == "Canteen Main")
            {
                Session["ContentName"] = section;

                string Weburl = ConfigurationManager.AppSettings["GetApi"];
                string AuthKey = ConfigurationManager.AppSettings["Authkey"];

                string APIKey = ConfigurationManager.AppSettings["APIKey"];



                List<CanteenManagement> canteenmainList = new List<CanteenManagement>();
                string strparams = "strACCESSID=" + "PS003" + "&strUNICID=" + "PS003_Services_CanteenManagementSystem_Main" + "&strGroupID=" + "MainSection_CanteenManagement";
                string canteenmainurl = Weburl + "?" + strparams;

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


                            var response = await client.GetAsync(canteenmainurl);

                            if (response.IsSuccessStatusCode)
                            {
                                var jsonString = await response.Content.ReadAsStringAsync();

                                ApiResponsecanteenmanagementmain canteenmain = JsonConvert.DeserializeObject<ApiResponsecanteenmanagementmain>(jsonString);
                                Session["PS_T_CanteenMAIN_RECID"] = canteenmain.Data[0].PS_RECID;
                               
                                Session["PS_T_CanteenMAIN_ACCESSID"] = canteenmain.Data[0].PS_ACCESSID;
                           
                                Session["PS_T_CanteenMAIN_PAGENAME"] = canteenmain.Data[0].PS_PAGENAME;
                              
                                Session["PS_T_CanteenMAIN_CONTENTTYPE"] = canteenmain.Data[0].PS_CONTENTTYPE;
                          
                                Session["PS_T_CanteenMAIN_PARENT"] = canteenmain.Data[0].PS_PARENT;
                             
                                Session["PS_T_CanteenMAIN_NAME"] = canteenmain.Data[0].PS_NAME;
                           
                                Session["PS_T_CanteenMAIN_ID"] = canteenmain.Data[0].PS_ID;
                             
                                Session["PS_T_CanteenMAIN_LASTDATETIME"] = canteenmain.Data[0].PS_LASTDATETIME;
                              
                                Session["PS_T_CanteenMAIN_TYPE"] = canteenmain.Data[0].PS_TYPE;

                                List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(canteenmain.Data[0].PS_VALUES);
                                //string psValues = canteenmain.Data[1].PS_VALUES.ToString();
                                //string base64Image = psValues;

                             
                                //if (!string.IsNullOrEmpty(base64Image))
                                //{
                                   
                                //    string mimeType = GetImageMimeType(base64Image);  

                                //    ViewBag.HOMECONTENTMAINIMAGE = base64Image;
                                //    ViewBag.MIMEType = mimeType; 
                                //    Session["Main_content_Images"] = ViewBag.HOMECONTENTMAINIMAGE;
                                //}

                                ViewBag.CanteenMainText = rootObjects;
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

                return View("CanteenMain");
            }
            return RedirectToAction("HomeMainContent", "RecentTickets");
        }

        [HttpPost]
        public async Task<ActionResult> CanteenMainTextSave(List<string> psValues, CanteenManagement Objcanteenmain, string Form)
        {
            string HomeContentMainurl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];

            string APIKey = ConfigurationManager.AppSettings["APIKey"];
            string PS_T_CanteenMAIN_RECID = Session["PS_T_CanteenMAIN_RECID"].ToString();
            string PS_T_CanteenMAIN_ACCESSID = Session["PS_T_CanteenMAIN_ACCESSID"].ToString();
            string PS_T_CanteenMAIN_PAGENAME = Session["PS_T_CanteenMAIN_PAGENAME"].ToString();
            string PS_T_CanteenMAIN_CONTENTTYPE = Session["PS_T_CanteenMAIN_CONTENTTYPE"].ToString();
            string PS_T_CanteenMAIN_PARENT = Session["PS_T_CanteenMAIN_PARENT"].ToString();
            string PS_T_CanteenMAIN_NAME = Session["PS_T_CanteenMAIN_NAME"].ToString();
            string PS_T_CanteenMAIN_ID = Session["PS_T_CanteenMAIN_ID"].ToString();
            string PS_T_CanteenMAIN_LASTDATETIME = Session["PS_T_CanteenMAIN_LASTDATETIME"].ToString();
            string PS_T_CanteenMAIN_TYPE = Session["PS_T_CanteenMAIN_TYPE"].ToString();


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
                        ""pS_RECID"": ""{PS_T_CanteenMAIN_RECID}"",
                        ""pS_ACCESSID"": ""{PS_T_CanteenMAIN_ACCESSID}"",
                        ""pS_PAGENAME"": ""{PS_T_CanteenMAIN_PAGENAME}"",
                        ""pS_CONTENTTYPE"": ""{PS_T_CanteenMAIN_CONTENTTYPE}"",
                        ""pS_PARENT"": ""{PS_T_CanteenMAIN_PARENT}"",
                        ""pS_NAME"": ""{PS_T_CanteenMAIN_NAME}"",
                        ""pS_ID"": ""{PS_T_CanteenMAIN_ID}"",
                        ""pS_VALUES"": ""{escapedWrappedSerializedPsValues}"", 
                        ""pS_LASTDATETIME"": ""1"",
                        ""pS_TYPE"": ""{PS_T_CanteenMAIN_TYPE}""
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
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponsecanteenmanagementmain>(responseBody);
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