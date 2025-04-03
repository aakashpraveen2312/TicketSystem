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
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    public class TicketsController : Controller
    {
        // GET: Tickets
        [HttpGet]
        public ActionResult Ticket()
        {

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

        private string GetImageMimeType2(string base64Image2)
        {
            if (base64Image2.Contains("data:image/jpeg;base64,"))
                return "image/jpeg";
            if (base64Image2.Contains("data:image/png;base64,"))
                return "image/png";
            if (base64Image2.Contains("data:image/gif;base64,"))
                return "image/gif";
            if (base64Image2.Contains("data:image/bmp;base64,"))
                return "image/bmp";
            // Default to JPEG if not found
            return "image/jpeg";
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Ticket(Tickets tickets, HttpPostedFileBase myfile)
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
                        tickets.TC_REQUEST_ATTACHMENT_PREFIX = base64Image;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "No file uploaded.");
                    }

                }
                // Define your API URL and keys
                var NewTicketPostURL = ConfigurationManager.AppSettings["NewTicketurl"];
                //       string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                //     string APIKey = Session["APIKEY"].ToString();

                // Construct the JSON content for the API request
                var content = $@"{{           
            ""tC_USERID"": ""{Session["UserID"]}"",           
            ""tC_COMPANYID"": ""{ Session["CompanyID"]}"",
            ""tC_PROJECTID"": ""{Session["ProjectID"]}"",        
            ""tC_TICKETDATE"": ""{DateTime.Now.ToString("yyyy-MM-dd")}"",        
            ""tC_SUBJECT"": ""{tickets.TC_SUBJECT}"",        
            ""tC_OTP"": ""{"6757"}"",
            ""tC_COMMENTS"": ""{tickets.TC_COMMENTS}"",
            ""tC_REQUEST_ATTACHMENT_PREFIX"": ""{base64Image}"",  
            ""tC_REQUEST_DATETIME"": ""{DateTime.Now.ToString("yyyy-MM-dd")}"",
            ""tC_RESPONSE_ATTACHMENT_PREFIX"": ""{""}"",
            ""tC_RESPONSE_USERID"": ""{0}"",
            ""tC_RESPONSE_DATETIME"": ""{DateTime.Now.ToString("yyyy-MM-dd")}"",
            ""tC_RESPONSE_COMMENTS"": ""{""}"",            
            ""tC_STATUS"": ""{"S"}"",
            ""tC_PRIORITYTYPE"": ""{tickets.TC_PRIORITYTYPE}""
        }}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(NewTicketPostURL),
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

                // Add headers for authentication
                //    client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                //   client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                // Send the request and await the response
                var response = await client.SendAsync(request);
                // Check if the response is successful
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseTicketsResponse>(responseBody);

                    // Return the appropriate result based on the API response
                    if (apiResponse.Status == "Y")

                    {
                        return RedirectToAction("Ticket", "Tickets");
                    }
                    else if (apiResponse.Status == "U" || apiResponse.Status == "N")
                    {
                        return Json(new { status = "error", message = apiResponse.Message });
                    }
                    else
                    {
                        return RedirectToAction("List", "Quatation");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            return View("Ticket");
        }


        //public ActionResult Ticket_History()
        //{
        //    List<Tickethistory> Lists = new List<Tickethistory>
        //    {
        //        new Tickethistory { Serialnumber = 1,  Name = "Prathesh",Reason="Reason1",Date="25/11/2024" },
        //        new Tickethistory { Serialnumber = 2,  Name = "Manoj",Reason="Reason2",Date="28/11/2024" },
        //        new Tickethistory { Serialnumber = 3,  Name = "Aaksh",Reason="Reason3",Date="05/12/2024" },
        //        new Tickethistory { Serialnumber = 4,  Name = "Ashwath",Reason="Reason4",Date="10/12/2024" },
        //        new Tickethistory { Serialnumber = 5,  Name = "Sanjay",Reason="Reason5",Date="26/12/2024" }
        //    };
        //    return View(Lists);
        //}

        public async Task<ActionResult> Ticket_History()
        {

            Tickethistory objRecents = new Tickethistory();


            string Weburl = ConfigurationManager.AppSettings["ClientScreen"];

            List<Tickethistory> RecentTicketListall = new List<Tickethistory>();



            string strparams = "?TC_USERID=" + Session["UserID"];

            string url = Weburl+ strparams;

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {

                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var response = await client.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var rootObjects = JsonConvert.DeserializeObject<ApiResponseTicketsHistoryResponse>(jsonString);
                            RecentTicketListall = rootObjects.Data;
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

            return View(RecentTicketListall);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ClientResponseTicket(Reviewtickets tickets, HttpPostedFileBase myfile, string statusparam)
        {
            var combox = tickets.Combo== "ReOpen"?"O":"C";
            

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
                        tickets.TC_REQUEST_ATTACHMENT_PREFIX = base64Image;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "No file uploaded.");
                    }

                }
                // Define your API URL and keys
                var NewTicketPostURL = ConfigurationManager.AppSettings["ClientResponse"];
                //       string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                //     string APIKey = Session["APIKEY"].ToString();

                // Construct the JSON content for the API request
                var content = $@"{{           
            ""tc_RECID"": ""{ Session["RECORDID"]}"",           
            ""tC_USERID"": ""{Session["UserID"]}"",           
            ""tC_COMPANYID"": ""{ Session["CompanyID"]}"",
            ""tC_PROJECTID"": ""{Session["ProjectID"]}"",        
            ""tC_TICKETDATE"": ""{DateTime.Now.ToString("yyyy-MM-dd")}"",        
            ""tC_SUBJECT"": ""{""}"",        
            ""tC_OTP"": ""{"6757"}"",
            ""tC_COMMENTS"": ""{tickets.TC_COMMENTS}"",
            ""tC_REQUEST_ATTACHMENT_PREFIX"": ""{base64Image}"",  
            ""tC_REQUEST_DATETIME"": ""{DateTime.Now.ToString("yyyy-MM-dd")}"",
            ""tC_RESPONSE_ATTACHMENT_PREFIX"": ""{""}"",
            ""tC_RESPONSE_USERID"": ""{0}"",
            ""tC_RESPONSE_DATETIME"": ""{DateTime.Now.ToString("yyyy-MM-dd")}"",
            ""tC_RESPONSE_COMMENTS"": ""{""}"",            
            ""tC_STATUS"": ""{combox}"",
            ""tC_PRIORITYTYPE"": ""{tickets.TC_PRIORITYTYPE}""
        }}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(NewTicketPostURL),
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

                // Add headers for authentication
                //    client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                //   client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                // Send the request and await the response
                var response = await client.SendAsync(request);
                // Check if the response is successful
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseTicketsResponse>(responseBody);

                    // Return the appropriate result based on the API response
                    if (apiResponse.Status == "Y")

                    {
                        return RedirectToAction("Ticket", "Tickets");
                    }
                    else if (apiResponse.Status == "U" || apiResponse.Status == "N")
                    {
                        return Json(new { status = "error", message = apiResponse.Message });
                    }
                    else
                    {
                        return RedirectToAction("List", "Quatation");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            return View("Ticket");
        }


        public async Task<ActionResult> ReviewTickets(string recid2)
        {
            Session["RECORDID"] = recid2;

            //TempData["AppUserRecID"] = id;
            string WEBURLGETBYID = ConfigurationManager.AppSettings["AdminGetSingleURL"];
            //   string Authkey = ConfigurationManager.AppSettings["Authkey"];

            //List<Bins> BinsList = new List<Bins>();
            Reviewtickets Review = null;

            //     string APIKey = Session["APIKEY"].ToString();

            //GlobalVariables.AU_RECID = id;


            string strparams = "RECID=" + recid2;
            string finalurl = WEBURLGETBYID + "?" + strparams;

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        //  client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                        //   client.DefaultRequestHeaders.Add("Authorization", Authkey);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                        var response = await client.GetAsync(finalurl);



                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var content = JsonConvert.DeserializeObject<APIIReviewticketsGetAllREC>(jsonString);

                            //APIResponseRecenttickets content2 = JsonConvert.DeserializeObject<APIResponseRecenttickets>(jsonString);
                           
                            Session["TC_USERID"] = content.Data.TC_USERID;
                            Session["CompanyID"] = content.Data.TC_COMPANYID;
                            Session["ProjectID"] = content.Data.TC_PROJECTID;

                            //List<string> rootObjects = JsonConvert.DeserializeObject<List<string>>(content2.Data.);

                            string base64Image = content.Data.TC_REQUEST_ATTACHMENT_PREFIX;
                            string base64Image2 = content.Data.TC_RESPONSE_ATTACHMENT_PREFIX;

                            if (!string.IsNullOrEmpty(base64Image))
                            {
                                string mimeType = GetImageMimeType(base64Image);
                                ViewBag.AdminTicketsIMAGE = base64Image;
                            }

                            if (!string.IsNullOrEmpty(base64Image2))
                            {
                                string mimeType2 = GetImageMimeType2(base64Image2);
                                ViewBag.AdminTicketsIMAGE2 = base64Image2;
                            }
                            
                            
                            
                            Review = content.Data;
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
            return View(Review);
        }




    }
}