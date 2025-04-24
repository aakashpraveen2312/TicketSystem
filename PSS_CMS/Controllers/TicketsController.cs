using CaptchaMvc.HtmlHelpers;
using ClosedXML.Excel;
using Newtonsoft.Json;
using PSS_CMS.Fillter;
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
    [ApiKeyAuthorize]
    public class TicketsController : Controller
    {
        // GET: Tickets changes by aakash
        [HttpGet]
        public async Task<ActionResult> Ticket()
         {
            var viewModel = new Tickets();
            string Weburl = ConfigurationManager.AppSettings["COMBOBOXTICKETTYPE"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams ="cmprecid=" + Session["CompanyID"];
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
                            var rootObjects = JsonConvert.DeserializeObject<ApiResponseTicketsResponseTypes>(jsonString);
                            var ticketTypes = rootObjects?.Data ?? new List<TicketComboTypes>();

                            viewModel.TicketCombo.TicketTypes = ticketTypes.Select(item => new SelectListItem
                            {
                                Value = item.TT_TICKETTYPE,
                                Text = item.TT_TICKETTYPE
                            }).ToList();
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, $"Error: {response.ReasonPhrase}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Exception occurred: {ex.Message}");
            }

            // Pass the view model to the next method
            await ComboBoxTicketHistoryProjectTypeNewticket(viewModel);

            return View(viewModel);
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Ticket(Tickets tickets,TicketComboTypes types, HttpPostedFileBase myfile)
         {
            var OTP=tickets.TC_OTP;
            
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
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();
             
                var content = $@"{{           
            ""tC_USERID"": ""{Session["UserID"]}"",           
            ""tC_CRECID"": ""{ Session["CompanyID"]}"",          
            ""tC_PROJECTID"": ""{tickets.SelectedProjectType}"",        
            ""tC_TICKETDATE"": ""{tickets.TC_Dates}"",        
            ""tC_SUBJECT"": ""{tickets.TC_SUBJECT}"",        
            ""tC_OTP"": ""{"6757"}"",
    ""tC_COMMENTS"": ""{HttpUtility.JavaScriptStringEncode(tickets.TC_COMMENTS)}"",
            ""tC_REQUEST_ATTACHMENT_PREFIX"": ""{base64Image}"",  
            ""tC_REQUEST_DATETIME"": ""{DateTime.Now.ToString("yyyy-MM-dd")}"",          
            ""tC_STATUS"": ""{"S"}"",
            ""tC_PRIORITYTYPE"": ""{tickets.TC_PRIORITYTYPE}"",
            ""tC_TICKETTYPE"": ""{tickets.SelectedTicketType}"",
            ""tC_USERNAME"": ""{Session["UserName"]}"",
            ""tC_REFERENCEID"": ""{0}""
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
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                if (this.IsCaptchaValid("Captcha is not valid"))
                {
                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<ApiResponseTicketsResponse>(responseBody);

                        if (apiResponse.Status == "Y")
                        {
                            return Json(new { status = "Form submmited successfully" });
                        }
                        else if (apiResponse.Status == "U" || apiResponse.Status == "N")
                        {
                            return Json(new { apiResponse.Message });
                        }
                        else
                        {
                            return Json(new { status = "Error Occured" });
                        }
                    }
                    else
                    {
                        return Json(new { status = "error", message ="Error: " + response.ReasonPhrase });
                    }
                }
                else
                {
                    return Json(new { status = "error", message = "Captcha is not valid." });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            return View("Ticket");
        }

        public async Task<ActionResult> Ticket_History(string userid, string userrole, string searchPharse, string status, string projectType, string ticketType,string StartDate,string EndDate)
        {
            Tickethistory objRecents = new Tickethistory();

            string Weburl = ConfigurationManager.AppSettings["ClientTicketURL"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<Tickethistory> RecentTicketListall = new List<Tickethistory>();
            
            string strparams = "TC_USERID=" + Session["UserID"] + "&StrUsertype=" + Session["UserRole"] + "&cmprecid=" + Session["CompanyID"];
            string url = Weburl + "?" + strparams;

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
                            var rootObjects = JsonConvert.DeserializeObject<ApiResponseTicketsHistoryResponse>(jsonString);
                            RecentTicketListall = rootObjects.Data;


                            if (
                                 string.IsNullOrWhiteSpace(ticketType) &&
                                 string.IsNullOrWhiteSpace(status) &&
                                 string.IsNullOrWhiteSpace(projectType) &&
                                 string.IsNullOrWhiteSpace(StartDate) &&
                                 string.IsNullOrWhiteSpace(EndDate) &&
                                 string.IsNullOrWhiteSpace(searchPharse))
                            {
                                // Exclude Closed tickets on the first load if no filters are applied
                                RecentTicketListall = RecentTicketListall.Where(t => t.TC_STATUS != "C").ToList();
                            }

                            if (!string.IsNullOrEmpty(ticketType))
                            {
                                RecentTicketListall = RecentTicketListall.Where(t => t.TC_TICKETTYPE == ticketType).ToList();
                            }
                           
                           
                            if (!string.IsNullOrEmpty(status))
                            {
                                RecentTicketListall = RecentTicketListall.Where(t => t.TC_STATUS == status).ToList();
                            }
                            if (!string.IsNullOrEmpty(projectType))
                            {
                                RecentTicketListall = RecentTicketListall.Where(t => t.TC_PROJECTID == projectType).ToList();
                            }
                            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
                            {
                                //DateTime fromDate = DateTime.Parse(StartDate);//parse it is used to convert the string to datetime object
                                //DateTime toDate = DateTime.Parse(EndDate);


                                RecentTicketListall = RecentTicketListall
          .Where(t => string.Compare(t.TC_TICKETDATE, StartDate) >= 0 &&
                      string.Compare(t.TC_TICKETDATE, EndDate) <= 0)
          .ToList();
                            }
                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                RecentTicketListall = RecentTicketListall
                                    .Where(r => r.TC_PROJECTID.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.TC_SUBJECT.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.AdminNameDisplay.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.TC_PRIORITYTYPE.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.TC_STATUS.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.TC_TICKETTYPE.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.TC_TICKETDATES.ToLower().Contains(searchPharse.ToLower()))
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

            await ComboBoxTicketHistory();
            await ComboBoxTicketHistoryProjectType();
            return View(RecentTicketListall);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ClientResponseTicket(Reviewtickets tickets, HttpPostedFileBase myfile, string statusparam)
        {
            var combox = tickets.Combo == "Re-open" ? "O" :
                         (tickets.Combo == "Close" ? "C" : "S");

            try
            {
                // Handle File Upload
                string base64Image = ProcessFileUpload(Request.Files);

                if (combox == "O")
                {
                    var apiUrl = ConfigurationManager.AppSettings["ClientResponse"];
                    string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                    string APIKey = Session["APIKEY"].ToString();

                    var content = JsonConvert.SerializeObject(new
                    {
                        tC_USERID = Session["UserID"],
                        tC_CRECID = Session["CompanyID"],
                        tC_PROJECTID = Session["ProjectID"],
                        tC_TICKETDATE = DateTime.Now.ToString("yyyy-MM-dd"),
                        tC_SUBJECT = Session["Subject"],
                        tC_OTP = "6757",
                        tC_COMMENTS =  HttpUtility.JavaScriptStringEncode(tickets.TC_COMMENTS),
                        tC_REQUEST_ATTACHMENT_PREFIX = base64Image,
                        tC_REQUEST_DATETIME = DateTime.Now.ToString("yyyy-MM-dd"),
                        tC_STATUS = combox,
                        tC_PRIORITYTYPE = Session["TC_PRIORITYTYPE"],
                        tC_TICKETTYPE = Session["TC_TICKETTYPE"],
                        tC_USERNAME = Session["REOPENUSERNAME"],
                        tC_REFERENCEID = Session["ReferenceRecID"]
                    });
                    // Set up HTTP client with custom validation (for SSL certificates)
                    var handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                    };

                    var client = new HttpClient(handler);
                    client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                    client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                    var apiResponse = await SendApiRequest(apiUrl, content, HttpMethod.Post, APIKey, AuthKey);



                    if (apiResponse.Status == "Y")
                    {
                        return Json(new { status = "Y", message = "Ticket reopened successfully!" });
                    }
                        
                    else
                    {
                        return Json(new { status = "N", message = apiResponse.Message });
                    }
                      
                }
                else
                {
                    var apiUrl = ConfigurationManager.AppSettings["UpdateComboresponse"];
                    string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                    string APIKey = Session["APIKEY"].ToString();


                    var content = JsonConvert.SerializeObject(new
                    {
                        tC_RECID = Session["RECORDID"],
                        tC_CRECID = Session["CompanyID"],
                        tC_USERNAME = Session["REOPENUSERNAME"],
                        tC_STATUS = combox
                    });
                    // Set up HTTP client with custom validation (for SSL certificates)
                    var handler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                    };

                    var client = new HttpClient(handler);
                    client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                    client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                    var apiResponse = await SendApiRequest(apiUrl, content, HttpMethod.Put, APIKey, AuthKey);


                    if (apiResponse.Status == "Y")
                    {
                        return Json(new { status = "Y", message ="Ticket closed successfully!" });
                    }
                       
                    else
                    {
                        return Json(new { status = "N", message = apiResponse.Message });
                    }
                       
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = "Error", message = "Exception occurred: " + ex.Message });
            }
        }

        // Helper method to process file uploads
        private string ProcessFileUpload(HttpFileCollectionBase files)
        {
            if (files.Count > 0)
            {
                var file = files[0];
                if (file != null && file.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(file.InputStream))
                    {
                        byte[] fileBytes = binaryReader.ReadBytes(file.ContentLength);
                        return Convert.ToBase64String(fileBytes);
                    }
                }
            }
            return null;
        }

        // Helper method to send API requests
        private async Task<ApiResponseTicketsResponse> SendApiRequest(string url, string content, HttpMethod method, string apiKey, string authKey)
        {
            using (var client = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            }))
            {
                var request = new HttpRequestMessage(method, new Uri(url))
                {
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };

                // ✅ Add headers
                request.Headers.Add("ApiKey", apiKey);
                request.Headers.Add("Authorization", authKey);
                request.Headers.Add("X-Version", "1");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<ApiResponseTicketsResponse>(responseBody);
            }
        }


        public async Task<ActionResult> ReviewTickets(string recid2,string status,string REOPENUSERNAME,string projectid)
        {
            IEnumerable<Ticket> ticketList = await GetTickets(recid2, status, REOPENUSERNAME, projectid); // Your logic to get a list of tickets
            return View(ticketList); // Pass the collection to the view
        }

        public async Task<IEnumerable<Ticket>> GetTickets(string recid2, string status,String REOPENUSERNAME,string projectid)
        {
            Session["ProjectID"] = projectid;
            Session["RECORDID"] = recid2;
            Session["Status"] = status;
            Session["REOPENUSERNAME"] = REOPENUSERNAME;

            string WEBURLGETBYID = ConfigurationManager.AppSettings["AdminGetSingleURL"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            List<Ticket> ticketList = new List<Ticket>();

            string strparams = "TC_USERID=" + Session["UserID"] + "&StrRecid=" + recid2 + "&cmprecid=" + Session["CompanyID"];
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
                            var content = JsonConvert.DeserializeObject<TicketModel>(jsonString);

                            ticketList = content.Data;
                            Session["ReferenceRecID"] = content.Data[0].TC_RECID;
                            Session["Subject"] = content.Data[0].TC_SUBJECT;
                            Session["TC_PRIORITYTYPE"] = content.Data[0].TC_PRIORITYTYPE;
                            Session["TC_TICKETTYPE"] = content.Data[0].TC_TICKETTYPE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            return ticketList;
        }
       
        //User can cloe their ticket click the refresh icon
        public async Task<ActionResult> DeleteUpdateTicket(Reviewtickets tickets, HttpPostedFileBase myfile, string statusparam, string recid2,String userclosedname)
        {           
            try
                {
                Session["RECORDID"] = recid2;
                Session["USERCLOSEDNAME"] = userclosedname;
                   
                    var UpdateTicketPostURL = ConfigurationManager.AppSettings["UpdateComboresponse"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                // Construct the JSON content for the API request
                var content = $@"{{  
            ""tC_RECID"": ""{Session["RECORDID"] }"",
            ""tC_USERNAME"": ""{Session["USERCLOSEDNAME"] }"",
            ""tC_STATUS"": ""{"C"}"",
            ""tC_CRECID"": ""{Session["CompanyID"]}""
        }}";

                    // Create the HTTP request
                    var request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(UpdateTicketPostURL),
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
                        var apiResponse = JsonConvert.DeserializeObject<ApiResponseTicketsResponse>(responseBody);

                    // Return the appropriate result based on the API response
                    if (apiResponse.Status == "Y")
                    {
                        return Json(new { success = true, message = "Ticket closed successfully." });
                    }
                    else if (apiResponse.Status == "U" || apiResponse.Status == "N")
                    {
                        return Json(new { success = false, message = apiResponse.Message });
                    }
                }

                return Json(new { success = false, message = "Error occurred while closing the ticket." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Exception: " + ex.Message });
            }

        }

        public async Task<ActionResult> ComboBoxTicketHistory()
        {

               List<SelectListItem> ticketTypes = new List<SelectListItem>();
                string strparams ="cmprecid=" + Session["CompanyID"];
                string webUrlGet = ConfigurationManager.AppSettings["COMBOBOXTICKETTYPE"];

                string url = webUrlGet + "?" + strparams;

                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

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
                                var rootObjects = JsonConvert.DeserializeObject<TicketTypeModel>(jsonString);

                                if (rootObjects?.Data != null)
                                {
                                    ticketTypes = rootObjects.Data.Select(t => new SelectListItem
                                    {
                                        Value = t.TT_TICKETTYPE, // or the appropriate value field
                                        Text = t.TT_TICKETTYPE // or the appropriate text field
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
                ViewBag.TicketTypes = ticketTypes;

                return View();
        }
        //new Ticket list view combo project type
        public async Task<ActionResult> ComboBoxTicketHistoryProjectType()
        {

            List<SelectListItem> projectTypes = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["COMBOBOXPROJECTTYPE"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "userid=" + Session["UserID"] + "&StrUsertype=" + Session["UserRole"] + "&cmprecid="+ Session["CompanyID"];
            //string strparams = "companyId=" + Session["CompanyID"];
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
                            var rootObjects = JsonConvert.DeserializeObject<TicketTypeModel>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                projectTypes = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.P_NAME, // or the appropriate value field
                                    Text = t.P_NAME // or the appropriate text field
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
            ViewBag.ProjectTypes = projectTypes;

            return View();
        }

        //new Ticket combo project type
        public async Task ComboBoxTicketHistoryProjectTypeNewticket(Tickets viewModel)
        {
            string webUrlGet = ConfigurationManager.AppSettings["COMBOBOXPROJECTTYPE"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "userid=" + Session["UserID"] + "&StrUsertype=" + Session["UserRole"] + "&cmprecid=" + Session["CompanyID"];
            //string strparams = "companyId=" + Session["CompanyID"];
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
                            var rootObjects = JsonConvert.DeserializeObject<ApiResponseTicketsResponseTypes>(jsonString);
                            var ticketTypes2 = rootObjects?.Data ?? new List<TicketComboTypes>();

                            viewModel.TicketCombo2.TicketTypes2 = ticketTypes2.Select(item => new SelectListItem
                            {
                                Value = item.P_NAME,
                                Text = item.P_NAME
                            }).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Exception occurred: {ex.Message}");
            }
        }


        public async Task<ActionResult> FAQ(string searchPharse,int projectID = 0)
        {                    
            string Weburl = ConfigurationManager.AppSettings["FAQGET1"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<Faq> FAQList = new List<Faq>();

           
            string strparams = "projectID=" + projectID + "&cmprecid=" + Session["CompanyID"];
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
                            var rootObjects = JsonConvert.DeserializeObject<RootObjectFAQ>(jsonString);
                            FAQList = rootObjects.Data;
                            if (!string.IsNullOrEmpty(searchPharse)) {
                                FAQList = FAQList.Where(r => r.F_QUESTION.ToLower().Contains(searchPharse.ToLower()) ||
                                r.F_ANSWER.ToLower().Contains(searchPharse.ToLower())).ToList();
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
            await FAQComboProjectType();
            return View(FAQList);
        }

        //FAQ project type combo
        public async Task<ActionResult> FAQComboProjectType()
        {

            List<SelectListItem> projectTypes = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["COMBOPRODUCTSHOW"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "userid=" + Session["UserID"] + "&StrUsertype=" + Session["UserRole"] + "&cmprecid=" + Session["CompanyID"];
            //string strparams = "companyId=" + Session["CompanyID"];
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
                            var rootObjects = JsonConvert.DeserializeObject<TicketTypeModel>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                projectTypes = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.TPM_RECID.ToString(), // or the appropriate value field
                                    Text = t.TPM_PRODUCTNAME // or the appropriate text field
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
            ViewBag.ProjectTypes = projectTypes;

            return View();
        }

        public async Task<ActionResult> ExcelUserDownload()
        {
            string Weburl = ConfigurationManager.AppSettings["ExcelClientTicketURL"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"]?.ToString();

            string strparams = "TC_USERID=" + Session["UserID"] + "&StrUsertype=" + Session["UserRole"] + "&cmprecid=" + Session["CompanyID"];
            string url = Weburl + "?" + strparams;

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                        client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                        var response = await client.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            var fileBytes = await response.Content.ReadAsByteArrayAsync();

                            return File(fileBytes,
                                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                        "UserTickets.xlsx");
                        }
                        else
                        {
                            return Content("API Error: " + response.ReasonPhrase);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("Exception occurred: " + ex.Message);
            }
        }

        //public async Task<ActionResult> Edit(int? recid)
        //{
        //    Session["EDITRECID"] = recid;
        //    var viewModel = new Tickets();
        //    Tickets RecentTicketListall = null;
        //    string Weburl = ConfigurationManager.AppSettings["GetByID"];
        //    string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
        //    string APIKey = Session["APIKEY"].ToString();
        //    string strparams = "cmprecid=" + Session["CompanyID"] + "&RECID=" + recid;
        //    string finalurl = Weburl + "?" + strparams;

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

        //                var response = await client.GetAsync(finalurl);
        //                if (response.IsSuccessStatusCode)
        //                {
        //                    var jsonString = await response.Content.ReadAsStringAsync();                         
        //                    var rootObjects = JsonConvert.DeserializeObject<ApiResponseTicketsResponse>(jsonString);
        //                    RecentTicketListall = rootObjects.Data;
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError(string.Empty, $"Error: {response.ReasonPhrase}");
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ModelState.AddModelError(string.Empty, $"Exception occurred: {ex.Message}");
        //    }

        //    await Ticket();

        //    return View(RecentTicketListall);
        //}
        //[HttpPost]
        //public async Task<ActionResult> Edit(Tickets tickets)
        //{
        //    return View();
        //}

    }
}