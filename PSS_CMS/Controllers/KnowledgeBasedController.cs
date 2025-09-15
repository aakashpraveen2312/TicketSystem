using Newtonsoft.Json;
using PSS_CMS.Fillter;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    [ApiKeyAuthorize]
    public class KnowledgeBasedController : Controller
    {
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
        // GET: KnowledgeBased
        public async Task<ActionResult> Whitepaper(int? projectID,string searchPharse)
        {
            if (searchPharse == "")
            {
                // Clear the session if the input is an empty string
                Session["searchPharse"] = null;
                searchPharse = null;
            }
            else if (!string.IsNullOrWhiteSpace(searchPharse))
            {
                // Store valid search input
                Session["searchPharse"] = searchPharse;
            }
            else if (Session["searchPharse"] != null)
            {
                // Reuse previous value from session
                searchPharse = Session["searchPharse"].ToString();
            }



            WhitePaper objRecents = new WhitePaper();
            string Weburl = ConfigurationManager.AppSettings["GETWHITEPAPER"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            List<WhitePaper> RecentTicketList = new List<WhitePaper>();
            string strparams = "productid=" + projectID + "&cmprecid=" + Session["CompanyID"];
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
                            var rootObjects = JsonConvert.DeserializeObject<APIResponsewhitepaper>(jsonString);
                            RecentTicketList = rootObjects.Data;


                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                RecentTicketList = RecentTicketList
                                    .Where(r => r.WP_TITLE.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.WP_Description.ToLower().Contains(searchPharse.ToLower()))
                                    .ToList();
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
            await ProductAdminWhitepaper();
            return View(RecentTicketList);
        }

        public async Task<ActionResult> WhitepaperPost()
        {
            await ProductAdminWhitepaper();
            return View();
        }

        [HttpPost]

        public async Task<ActionResult> WhitepaperPost(WhitePaper whitepaper, HttpPostedFileBase myfile)
        {
            try
            {
                // Declare fileBytes and base64Image once, before the if block
                byte[] fileBytes = null;
                string base64Image = null;

                // Check if files are uploaded
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0]; // Get the first file from the request
                    if (file != null && file.ContentLength > 0)
                    {
                        // If file exists, read and convert it to base64
                        fileBytes = new byte[file.ContentLength];
                        file.InputStream.Read(fileBytes, 0, file.ContentLength);
                        base64Image = Convert.ToBase64String(fileBytes);

                        // Assign the base64 image to the model property
                        whitepaper.WP_ATTACHEMENT = base64Image;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "No file uploaded.");
                    }

                }
                // Define your API URL and keys
                var WhitepaperPostURL = ConfigurationManager.AppSettings["WHITEPAPERPOST"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();
                whitepaper.WP_TITLE = whitepaper.WP_TITLE?.Replace("\"", ""); // Removes double quotes
                whitepaper.WP_Description = whitepaper.WP_Description?.Replace("\"", ""); // Removes double quotes
                // Construct the JSON content for the API request
                var content = $@"{{           
            ""WP_TITLE"": ""{HttpUtility.JavaScriptStringEncode(whitepaper.WP_TITLE)}"",           
            ""WP_Description"": ""{HttpUtility.JavaScriptStringEncode(whitepaper.WP_Description)}"",
            ""WP_ATTACHEMENT"": ""{whitepaper.WP_ATTACHEMENT}"",
            ""WP_CREATEDDATETIME"": ""{DateTime.Now.ToString("yyyy-MM-dd")}"",
            ""wP_SORTORDER"": ""{"1"}"",                              
            ""wP_CURECID"": ""{0}"",                              
            ""wP_CRECID"": ""{ Session["CompanyID"]}"",                              
            ""wP_PRECID"": ""{(string.IsNullOrEmpty(whitepaper.SelectedProjectType2) ? "0" : whitepaper.SelectedProjectType2)}"",                              
            ""wP_URECID"": ""{ Session["UserRECID"]}"",                              
            ""wP_DISABLE"": ""{"N"}""           
        }}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(WhitepaperPostURL),
                    Method = HttpMethod.Post,
                    Headers =
            {
                { "X-Version", "1" },
                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
            },
                    Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
                };

                // Set up HTTP client with custom validation (for SSL certificates)
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler);
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                // Send the request and await the response
                var response = await client.SendAsync(request);
                // Check if the response is successful
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<APIResponsewhitepaper>(responseBody);

                    // Return the appropriate result based on the API response
                    if (apiResponse.Status == "Y")
                    {
                        return Json(new { success = true, message = "Whitepaper created successfully!" });
                    }
                    else if (apiResponse.Status == "N")
                    {
                        return Json(new { success = false, message = apiResponse.Message});
                    }
                    else
                    {
                        return Json(new { success = false, message = "Failed to create Whitepaper." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Error: " + response.ReasonPhrase });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Exception occurred: " + ex.Message });
            }
        }

        public async Task<ActionResult> ProductAdminWhitepaper()

        {
            List<SelectListItem> Product = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["PRODUCTGET"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "cmprecid=" + Session["CompanyID"];
            string url = webUrlGet + "?" + strparams;
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

                        var response = await client.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var rootObjects = JsonConvert.DeserializeObject<ProductMasterRootObject>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                Product = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.P_RECID.ToString(), // or the appropriate value field
                                    Text = t.P_NAME,
                                }).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            // Assuming you are passing ticketTypes to the view
            ViewBag.Product = Product;

            return View();
        }


        public async Task<ActionResult> Edit(int WP_RECID,int WP_PROJECTID,string WP_USERID,int wP_PRECID)
        {
            Session["WP_PROJECTID"] = WP_PROJECTID;
            Session["WP_USERID"] = WP_USERID;
            Session["WP_RECID"] = WP_RECID;
            Session["wP_PRECID"] = wP_PRECID;

            string WEBURLGETBYID = ConfigurationManager.AppSettings["WHITEPAPERGETBYID"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            WhitePaper whitePaper = null;

            string strparams = "recID=" + WP_RECID + "&cmprecid=" + Session["CompanyID"];
            string finalurl = WEBURLGETBYID + "?" + strparams;

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
                            var content = JsonConvert.DeserializeObject<RootObjectWHITEPAPERGET>(jsonString);
                            string base64Image = content.Data.WP_ATTACHEMENT;
                            if (!string.IsNullOrEmpty(base64Image))
                            {

                                string mimeType = GetImageMimeType(base64Image);

                                ViewBag.IMAGEWHITEPAPER = base64Image;
                                ViewBag.MIMETypeWHITEPAPER = mimeType;

                            }
                            whitePaper = content.Data;
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
                ModelState.AddModelError(string.Empty, "Exception occured: " + ex.Message);
            }

            return View(whitePaper);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(WhitePaper whitePaper, HttpPostedFileBase myfile)
        {

            try
            {
                // Declare fileBytes and base64Image once, before the if block
                byte[] fileBytes = null;
                string base64Image = null;

                // Check if files are uploaded
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0]; // Get the first file from the request
                    if (file != null && file.ContentLength > 0)
                    {
                        // If file exists, read and convert it to base64
                        fileBytes = new byte[file.ContentLength];
                        file.InputStream.Read(fileBytes, 0, file.ContentLength);
                        base64Image = Convert.ToBase64String(fileBytes);

                        // Assign the base64 image to the model property
                        whitePaper.WP_ATTACHEMENT = base64Image;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "No file uploaded.");
                    }

                }
                // Define your API URL and keys
                var WhitepaperUpdateURL = ConfigurationManager.AppSettings["WHITEPAPERUPDATE"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();
                whitePaper.WP_TITLE = whitePaper.WP_TITLE?.Replace("\"", ""); // Removes double quotes
                whitePaper.WP_Description = whitePaper.WP_Description?.Replace("\"", ""); // Removes double quotes
                // Construct the JSON content for the API request
                var content = $@"{{           
            ""wP_RECID"": ""{Session["WP_RECID"]}"",           
            ""WP_TITLE"": ""{HttpUtility.JavaScriptStringEncode(whitePaper.WP_TITLE)}"",           
            ""WP_Description"": ""{HttpUtility.JavaScriptStringEncode(whitePaper.WP_Description)}"",
            ""WP_ATTACHEMENT"": ""{whitePaper.WP_ATTACHEMENT}"",
            ""WP_CREATEDDATETIME"": ""{DateTime.Now.ToString("yyyy-MM-dd")}"",
            ""wP_SORTORDER"": ""{"1"}"",                              
            ""wP_CURECID"": ""{0}"",                              
            ""wP_CRECID"": ""{ Session["CompanyID"]}"",                              
            ""wP_PRECID"": ""{ Session["wP_PRECID"]}"",                              
            ""wP_URECID"": ""{ Session["UserRECID"]}"",                              
            ""wP_DISABLE"": ""{"N"}""           
        }}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(WhitepaperUpdateURL),
                    Method = HttpMethod.Put,
                    Headers =
            {
                { "X-Version", "1" },
                { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
            },
                    Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
                };

                // Set up HTTP client with custom validation (for SSL certificates)
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                var client = new HttpClient(handler);
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                // Send the request and await the response
                var response = await client.SendAsync(request);
                // Check if the response is successful
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<APIResponsewhitepaper>(responseBody);

                    if (apiResponse.Status == "Y")
                    {
                        return Json(new { success = true, message = "Whitepaper edited successfully!" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Failed to edit Whitepaper." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Error: " + response.ReasonPhrase });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Exception: " + ex.Message });
            }
        }

        public async Task<ActionResult> Delete(int WP_RECID)
        {
            string WEBURLDELETE = ConfigurationManager.AppSettings["WHITEPAPERDELETE"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            string strparams = "RECID=" + WP_RECID + "&cmprecid=" + Session["CompanyID"];
            string finalurl = WEBURLDELETE + "?" + strparams;

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


                        var request = new HttpRequestMessage
                        {
                            Method = HttpMethod.Delete,
                            RequestUri = new Uri(finalurl)
                        };

                        var response = await client.SendAsync(request);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseBody = await response.Content.ReadAsStringAsync();
                            var apiResponse = JsonConvert.DeserializeObject<APIResponsewhitepaper>(responseBody);

                            if (apiResponse.Status == "Y")
                            {

                                string redirectUrl = Url.Action("Whitepaper", "KnowledgeBased", new { });
                                return Json(new { status = "success", message = apiResponse.Message, redirectUrl = redirectUrl });
                            }
                            else if (apiResponse.Status == "U")
                            {
                                return Json(new { status = "error", message = apiResponse.Message });
                            }
                            else if (apiResponse.Status == "N")
                            {
                                return Json(new { status = "error", message = apiResponse.Message });
                            }


                        }
                        else
                        {
                            Console.WriteLine($"Failed to delete: {response.StatusCode} - {response.ReasonPhrase}");
                        }
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Request error occurred: {httpEx.Message}");
            }
            catch (TaskCanceledException tcEx)
            {
                Console.WriteLine($"Request timed out: {tcEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }
            return View();
        }

    }
}