using Newtonsoft.Json;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
    public class ClientImageController : Controller
    {
        // GET: ClientImage
        public async Task<ActionResult> ClientImageGet(string section)
        {
            if (section == "Client Section")

            {

                Session["ContentName"] = section;
               

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
                return View("ClientImageGet");
            }
            return RedirectToAction("Index","Home");
        }
        [HttpPost]
        public async Task<ActionResult> HomePageClientSectionSave(IEnumerable<HttpPostedFileBase> imageFiles)
        {
            string Weburl = ConfigurationManager.AppSettings["UpdateApi"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string APIKey = ConfigurationManager.AppSettings["APIKey"];

            // Helper function to check if any session variable is missing
            bool IsSessionVariableMissing(string variable)
            {
                return string.IsNullOrEmpty(Session[variable]?.ToString());
            }

            // Retrieve session data for PS_RECID, PS_ACCESSID, etc.
            string PS_CLIENT_RECID = Session["PS_CLIENT_RECID"]?.ToString();
            string PS_CLIENT_ACCESSID = Session["PS_CLIENT_ACCESSID"]?.ToString();
            string PS_CLIENT_PAGENAME = Session["PS_CLIENT_PAGENAME"]?.ToString();
            string PS_CLIENT_CONTENTTYPE = Session["PS_CLIENT_CONTENTTYPE"]?.ToString();
            string PS_CLIENT_PARENT = Session["PS_CLIENT_PARENT"]?.ToString();
            string PS_CLIENT_NAME = Session["PS_CLIENT_NAME"]?.ToString();
            string PS_CLIENT_ID = Session["PS_CLIENT_ID"]?.ToString();
            string PS_CLIENT_GROUPID = Session["PS_CLIENT_GROUPID"]?.ToString();

            // Validate session variables
            if (IsSessionVariableMissing("PS_CLIENT_RECID") ||
                IsSessionVariableMissing("PS_CLIENT_PAGENAME") ||
                IsSessionVariableMissing("PS_CLIENT_ACCESSID") ||
                IsSessionVariableMissing("PS_CLIENT_CONTENTTYPE") ||
                IsSessionVariableMissing("PS_CLIENT_PARENT") ||
                IsSessionVariableMissing("PS_CLIENT_NAME") ||
                IsSessionVariableMissing("PS_CLIENT_ID") ||
                IsSessionVariableMissing("PS_CLIENT_GROUPID"))
            {
                ModelState.AddModelError("", "One or more required session variables are missing.");
                return RedirectToAction("ClientImageGet");
            }

            // Retrieve session data for pS_IDs and pS_VALUES (MainContentPS_ID and MainContentImages)
            var pS_IDs = Session["MainContentPS_ID"] as List<string> ?? new List<string>();
            var existingImages = Session["MainContentImages"] as List<string> ?? new List<string>();

            // Validate if both session lists are available and have the same number of items
            if (pS_IDs.Count != existingImages.Count || pS_IDs.Count == 0)
            {
                ModelState.AddModelError("", "Mismatch in the number of pS_IDs and images or empty session data.");
                return RedirectToAction("ClientImageGet");
            }

            try
            {
                // Process uploaded images
                if (imageFiles != null)
                {

                    int index = 0;
                    foreach (var file in imageFiles)
                    {
                        if (file != null && file.ContentLength > 0)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                // Read file content into memory stream
                                file.InputStream.CopyTo(memoryStream);
                                byte[] fileBytes = memoryStream.ToArray();

                                // Convert file content to Base64
                                string base64Image = Convert.ToBase64String(fileBytes);

                                // Update the image list
                                if (index < existingImages.Count)
                                {
                                    existingImages[index] = base64Image; // Replace existing image
                                }
                                else
                                {
                                    existingImages.Add(base64Image); // Add new image
                                }
                            }
                        }
                        index++;
                    }
                }

                // Loop through pS_IDs and existingImages and send separate requests for each
                for (int i = 0; i < pS_IDs.Count; i++)
                {
                    string pS_ID = pS_IDs[i];
                    string pS_VALUE = existingImages[i];

                    // Prepare the payload for each image ID
                    string payload = $@"{{
                ""pS_RECID"": ""{PS_CLIENT_RECID}"",
                ""pS_ACCESSID"": ""{PS_CLIENT_ACCESSID}"",
                ""pS_PAGENAME"": ""{PS_CLIENT_PAGENAME}"",
                ""pS_CONTENTTYPE"": ""{PS_CLIENT_CONTENTTYPE}"",
                ""pS_PARENT"": ""{PS_CLIENT_PARENT}"",
                ""pS_NAME"": ""{PS_CLIENT_NAME}"",
                ""pS_ID"": ""{pS_ID}"", 
                ""pS_VALUES"": ""{pS_VALUE}"", 
                ""pS_LASTDATETIME"": ""1"",
                ""pS_TYPE"": ""{Session["PS_CLIENT_TYPE"]}""
            }}";

                    // Send API request for each image ID and value
                    using (HttpClientHandler handler = new HttpClientHandler())
                    {
                        handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                        using (HttpClient client = new HttpClient(handler))
                        {
                            client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                            client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                            var content = new StringContent(payload, Encoding.UTF8, "application/json");
                            var response = await client.PutAsync(Weburl, content);

                            if (response.IsSuccessStatusCode)
                            {
                                string responseBody = await response.Content.ReadAsStringAsync();
                                var apiResponse = JsonConvert.DeserializeObject<ApiClientimage>(responseBody);

                                if (apiResponse.Status == "Y")
                                {
                                    // Update the session with the latest image data
                                    Session["MainContentImages"] = existingImages;
                                }
                                else
                                {
                                    ModelState.AddModelError("", "API returned an error.");
                                    break; // Exit the loop if there is an error
                                }
                            }
                            else
                            {
                                string errorResponse = await response.Content.ReadAsStringAsync();
                                ModelState.AddModelError("", $"Error: {response.ReasonPhrase} - {errorResponse}");
                                break; // Exit the loop if there is an error
                            }
                        }
                    }
                }

                // After the loop, redirect to the Home page if everything was successful
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Log or display more detailed exception message if needed
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}