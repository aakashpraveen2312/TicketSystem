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

        public ActionResult RefreshCaptcha()
        {
            return PartialView("_CaptchaPartial");
        }
        // GET: Tickets changes by aakash
        [HttpGet]
        public async Task<ActionResult> Ticket(int? contractRecId, int? invoiceRecId)
        {
           
            var viewModel = new Tickets();
            // Load Products
            viewModel.TicketCombo2 = new TicketCombo2();
            viewModel.TicketCombo2.TicketTypes2 =
                await GetProducts(contractRecId, invoiceRecId);

            string Weburl = ConfigurationManager.AppSettings["COMBOBOXTICKETTYPE"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "cmprecid=" + Session["CompanyID"];
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

            return View(viewModel);
        }


        [HttpGet]
        public async Task<ActionResult> AddonCustomerCreateTicket()
        {
            var viewModel = new Tickets();
            string Weburl = ConfigurationManager.AppSettings["COMBOBOXTICKETTYPE"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "cmprecid=" + Session["CompanyID"];
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
            await ComboBoxProduct(viewModel);

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
        public async Task<ActionResult> Ticket(Tickets tickets, TicketComboTypes types, HttpPostedFileBase myfile)
        {
            var OTP = tickets.TC_OTP;

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
                        tickets.TC_REQUEST_ATTPREFIX = base64Image;
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
            ""tC_URECID"": ""{Session["UserRECID"]}"",           
            ""tC_CRECID"": ""{ Session["CompanyID"]}"",          
            ""tC_PRECID"": ""{tickets.SelectedProjectType ?? "0"}"",        
            ""tC_CURECID"": ""{tickets.SelectedCustomer ?? "0"}"",        
            ""tC_TICKETDATE"": ""{tickets.TC_Dates}"",        
            ""tC_SUBJECT"": ""{tickets.TC_SUBJECT}"",        
            ""tC_OTP"": ""{"6757"}"",
            ""tC_COMMENTS"": ""{HttpUtility.JavaScriptStringEncode(tickets.TC_COMMENTS)}"",
            ""tC_REQUEST_ATTPREFIX"": ""{base64Image}"",  
            ""tC_REQUEST_DATETIME"": ""{DateTime.Now.ToString("yyyy-MM-dd")}"",          
            ""tC_STATUS"": ""{"S"}"",
            ""tC_PRIORITYTYPE"": ""{tickets.TC_PRIORITYTYPE}"",
            ""tC_TICKETTYPE"": ""{tickets.SelectedTicketType}"",
            ""tC_PAIDSERVICE"": ""{(tickets.paidservice ? "Y" : "N")}"",           
            ""tC_USERNAME"": ""{Session["UserName"] ?? ""} - {Session["Role"] ?? ""}"",
            ""tC_REFERENCETRECID"": ""{0}"",
  ""tC_HFLAG"": ""{"N"}"",
""tC_ASSIGNFLAG"": ""{"N"}"",
""tC_PICKFLAG"": ""{"N"}"",
""tC_PRODUCTSERIALNUMBER"": ""{tickets.P_SERIALNUMBER ?? "No serial number"}"",
""tC_CUSTOMERTYPE"": ""{tickets.CustomerType ?? "Addon User"}""
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

                if (tickets.SelectedProjectType == null || tickets.SelectedProjectType == "0")
                {
                    return Json(new { status = "error", Message = "Please select the Product" });
                }

                if (string.IsNullOrWhiteSpace(tickets.TC_SUBJECT))
                {
                    return Json(new { status = "error", Message = "Please enter the Subject" });
                }

                if (string.IsNullOrWhiteSpace(tickets.TC_PRIORITYTYPE))
                {
                    return Json(new { status = "error", Message = "Please select the Priority Type" });
                }

                if (string.IsNullOrWhiteSpace(tickets.SelectedTicketType))
                {
                    return Json(new { status = "error", Message = "Please select the Ticket Type" });
                }

                if (string.IsNullOrWhiteSpace(tickets.TC_COMMENTS))
                {
                    return Json(new { status = "error", Message = "Please enter the Comments" });
                }

                // CAPTCHA VALIDATION AFTER SUCCESS
                if (string.IsNullOrWhiteSpace(Request["CaptchaInputText"]))
                {
                    return Json(new { status = "error", Message = "Please enter the Captcha." });
                }

                if (!this.IsCaptchaValid("Invalid captcha"))
                {
                    return Json(new { status = "error", Message = "Invalid Captcha." });
                }

                var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<ApiResponseTicketsResponse>(responseBody);

                        if (apiResponse.Status == "Y")
                        {
                        return Json(new { status = "success", Message = apiResponse.Message });
                    }
                        else if (apiResponse.Status == "U" || apiResponse.Status == "N")
                        {
                        return Json(new { status = "error", Message = apiResponse.Message });
                    }
                        else
                        {
                            return Json(new { status = "Error Occured" });
                        }
                    }
                    else
                    {
                        return Json(new { status = "error", Message = "Error: " + response.ReasonPhrase });
                    }
              
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            return View("Ticket");
        }

        public async Task<ActionResult> Ticket_History(string userid, string userrole, string searchPharse, string status, string projectType, string ticketType, string StartDate, string EndDate)
        {
            Tickethistory objRecents = new Tickethistory();

            string Weburl = ConfigurationManager.AppSettings["ClientTicketURL"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<Tickethistory> RecentTicketListall = new List<Tickethistory>();

            string strparams = "USERID=" + Session["UserRECID"] + "&StrUsertype=" + Session["UserRole"] + "&cmprecid=" + Session["CompanyID"];
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
                            //if (!string.IsNullOrEmpty(projectType))
                            //{
                            //    RecentTicketListall = RecentTicketListall.Where(t => t.P_RECID == projectType).ToList();
                            //}
                            if (!string.IsNullOrEmpty(projectType))
                            {
                                RecentTicketListall = RecentTicketListall.Where(t => t.P_RECID.ToString() == projectType).ToList();
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
                                    .Where(r => r.P_RECID.ToString().ToLower().Contains(searchPharse.ToLower()) ||
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
            await ComboBoxTicketHistoryProduct();
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
                        tC_URECID = Session["UserRECID"],
                        tC_CRECID = Session["CompanyID"],
                        tC_PRECID = Session["ProjectID"],
                        tC_TICKETDATE = DateTime.Now.ToString("yyyy-MM-dd"),
                        tC_SUBJECT = Session["Subject"],
                        tC_OTP = "6757",
                        tC_COMMENTS = HttpUtility.JavaScriptStringEncode(tickets.TC_COMMENTS),
                        tC_REQUEST_ATTPREFIX = base64Image,
                        tC_REQUEST_DATETIME = DateTime.Now.ToString("yyyy-MM-dd"),
                        tC_STATUS = combox,
                        tC_PRIORITYTYPE = Session["TC_PRIORITYTYPE"],
                        tC_TICKETTYPE = Session["TC_TICKETTYPE"],
                        tC_USERNAME = Session["REOPENUSERNAME"],
                        tC_REFERENCETRECID = Session["ReferenceRecID"]
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
                        tC_STATUS = combox,
                        tC_SISTATUS = "YG"
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
                        return Json(new { status = "Y", message = "Ticket closed successfully!" });
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


        public async Task<ActionResult> ReviewTickets(string recid2, string status, string REOPENUSERNAME, string projectid,string Customertype,int? ticuserid,string prodserial)
        {
            Session["Customertype"] = string.IsNullOrWhiteSpace(Customertype) ? "" : Customertype;
            Session["Ticid"] = recid2;
            Session["Prodid"] = projectid;
            Session["prodserial"] = string.IsNullOrWhiteSpace(prodserial) ? "" : prodserial;
            Session["ticuserid"] = ticuserid ?? 0;
            IEnumerable<Ticket> ticketList = await GetTickets(recid2, status, REOPENUSERNAME, projectid); // Your logic to get a list of tickets
            return View(ticketList); // Pass the collection to the view
        }

        public async Task<IEnumerable<Ticket>> GetTickets(string recid2, string status, String REOPENUSERNAME, string projectid)
        {
            Session["ProjectID"] = projectid;
            Session["RECORDID"] = recid2;
            Session["Status"] = status;
            Session["REOPENUSERNAME"] = REOPENUSERNAME;

            string WEBURLGETBYID = ConfigurationManager.AppSettings["AdminGetSingleURL"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            List<Ticket> ticketList = new List<Ticket>();

            string strparams = "USERID=" + Session["UserRECID"] + "&StrRecid=" + recid2 + "&cmprecid=" + Session["CompanyID"];
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
        public async Task<ActionResult> DeleteUpdateTicket(Reviewtickets tickets, HttpPostedFileBase myfile, string statusparam, string recid2, String userclosedname)
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
            string strparams = "cmprecid=" + Session["CompanyID"];
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
        public async Task<ActionResult> ComboBoxTicketHistoryProduct()
        {

            List<SelectListItem> Product = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["COMBOFORPRODUCTSELECTED"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "companyId=" + Session["CompanyID"] + "&UserID=" + Session["UserRECID"];
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
                                Product = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.P_RECID.ToString(), // or the appropriate value field
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
            ViewBag.Product = Product;

            return View();
        }
        private async Task<List<SelectListItem>> GetProducts(int? contractRecId, int? invoiceRecid)
        {
            Session["contractRecId"] = contractRecId == null
                  ? 0
                  : contractRecId;
            string webUrlGet = ConfigurationManager.AppSettings["ROLEDBASEDPRODUCTS"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            string strparams = "companyId=" + Session["CompanyID"]
                             + "&UserID=" + Session["UserRECID"]
                             + "&contractRecId=" + contractRecId
                             + "&invoiceRecId=" + invoiceRecid;

            string url = webUrlGet + "?" + strparams;

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                    client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                    var response = await client.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                        return new List<SelectListItem>();

                    var jsonString = await response.Content.ReadAsStringAsync();

                    var rootObjects =
                        JsonConvert.DeserializeObject<ApiResponseTicketsResponseTypes>(jsonString);

                    var products = rootObjects?.Data ?? new List<TicketComboTypes>();

                    return products.Select(x => new SelectListItem
                    {
                        Value = x.P_RECID.ToString(),
                        Text = x.P_NAME + " - " + x.P_SERIALNUMBER,

                    }).ToList();
                }
            }
        }
        //new Ticket combo project type
        [HttpGet]
        public async Task<JsonResult> ComboBoxProductNewticket(int? contractRecId, int? invoiceRecid)
        {
            Session["contractRecId"] = contractRecId == null
                  ? 0
                  : contractRecId;

            string webUrlGet = ConfigurationManager.AppSettings["ROLEDBASEDPRODUCTS"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            string strparams = "companyId=" + Session["CompanyID"]
                             + "&UserID=" + Session["UserRECID"]
                             + "&contractRecId=" + contractRecId
                             + "&invoiceRecId=" + invoiceRecid;

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

                            var result = ticketTypes2.Select(item => new
                            {
                                Value = item.P_RECID,
                                Text = item.P_NAME + " - " + item.P_SERIALNUMBER,
                                SerialNo = item.P_SERIALNUMBER
                            }).ToList();

                            return Json(result, JsonRequestBehavior.AllowGet); // ✅ IMPORTANT
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new List<object>(), JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> ComboProductTicketNew(string Recid, string SerialNo)
        {
            var customerResult = new List<object>();

            string webUrlGet = ConfigurationManager.AppSettings["CUSTOMERPRODUCTCOMBOFORUSER"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"]?.ToString();
            string cmpRecId = Session["CompanyID"]?.ToString();
            string strparams = "companyId=" + cmpRecId + "&productid=" + Recid + "&userrecid=" + Session["UserRECID"]+ "&SerialNo="+ SerialNo + "&contractrecid=" + Session["contractRecId"];
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
                            var apiResponse = JsonConvert.DeserializeObject<ApiResponseObject>(jsonString);

                            if (apiResponse?.Data != null)
                            {
                                customerResult = new List<object>
    {
        new {
            Value = apiResponse.Data.CU_RECID.ToString(),
            Text = apiResponse.Data.CU_NAME,
            WarrantyUpto = apiResponse.Data.CU_WARRANTYUPTO,
            ProductUpto = apiResponse.Data.LatestPaymentDueDate,
            WarrantyFreeCalls = apiResponse.Data.CU_WARRANTYFREECALLS,
            CustomerType = apiResponse.CustomerType
        }
    };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(customerResult, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> FAQ(string searchPharse, int? projectID)

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
            string Weburl = ConfigurationManager.AppSettings["FAQGET"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<Faq> FAQList = new List<Faq>();


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
                            var rootObjects = JsonConvert.DeserializeObject<RootObjectFAQ>(jsonString);
                            FAQList = rootObjects.Data;
                            if (!string.IsNullOrEmpty(searchPharse))
                            {
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
            await FAQComboProduct();
            return View(FAQList);
        }

        //FAQ project type combo
        public async Task<ActionResult> FAQComboProduct()
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
                            var rootObjects = JsonConvert.DeserializeObject<TicketTypeModel>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                Product = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.P_RECID.ToString(), // or the appropriate value field
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
            ViewBag.Product = Product;

            return View();
        }

        public async Task<ActionResult> ExcelUserDownload()
        {
            string Weburl = ConfigurationManager.AppSettings["ExcelClientTicketURL"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"]?.ToString();

            string strparams = "TC_USERID=" + Session["UserRECID"] + "&StrUsertype=" + Session["UserRole"] + "&cmprecid=" + Session["CompanyID"];
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
                                        Session["UserRole"] + "-Tickets" + ".xlsx");
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

        [HttpPost]
        public async Task<ActionResult> SaveRating(int? rating)
        {
            var apiUrl = ConfigurationManager.AppSettings["UpdateRatings"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();


            var content = JsonConvert.SerializeObject(new
            {
                tC_RECID = Session["RECORDID"],
                tC_CRECID = Session["CompanyID"],
                tC_RATINGS = rating
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
            return View();
        }

        //PDF
        public async Task<ActionResult> UserTicketsPdfDownload()
        {
            string Weburl = ConfigurationManager.AppSettings["UserClientsTicketsPdf"];

            string apiKey = Session["APIKEY"]?.ToString();
            string authKey = ConfigurationManager.AppSettings["AuthKey"];

            int userId = Convert.ToInt32(Session["UserRECID"]);
            string userRole = Session["UserRole"]?.ToString(); // should be "User"
            int companyId = Convert.ToInt32(Session["CompanyID"]);

            string finalUrl =
                $"{Weburl}?TC_USERID={userId}&StrUsertype={userRole}&cmprecid={companyId}";

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (s, c, ch, e) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("ApiKey", apiKey);
                        client.DefaultRequestHeaders.Add("Authorization", authKey);

                        var response = await client.GetAsync(finalUrl);

                        if (!response.IsSuccessStatusCode)
                        {
                            return Content("PDF download failed.");
                        }

                        byte[] pdfBytes = await response.Content.ReadAsByteArrayAsync();

                        return File(
                            pdfBytes,
                            "application/pdf",
                            $"User_Tickets_{DateTime.Now:yyyyMMddHHmmss}.pdf"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }

        public async Task<JsonResult> GetContractsModal()
        {
            var contractList = new List<object>();

            string webUrlGet = ConfigurationManager.AppSettings["TICKETCONTRACTGET"]; // 🔁 add in Web.config
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"]?.ToString();

            string strparams = "cmprecid=" + Session["CompanyID"] + "&Userid=" + Session["UserRECID"];
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

                            var apiResponse = JsonConvert.DeserializeObject<ContractApiResponse>(jsonString);

                            if (apiResponse != null && apiResponse.Status == "Y" && apiResponse.Data != null)
                            {
                                contractList = apiResponse.Data.Select(c => new
                                {
                                    CP_RECID = c.CP_RECID,
                                    CP_CODE = c.CP_CODE,
                                    CP_CONTRACTAMOUNT = c.CP_CONTRACTAMOUNT,
                                    CU_RECID = c.CU_RECID,
                                    CU_WARRANTYUPTO = c.CU_WARRANTYUPTO
                                }).ToList<object>();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Status = contractList.Count > 0 ? "Y" : "N",
                Data = contractList
            }, JsonRequestBehavior.AllowGet);
        }


        public async Task<JsonResult> GetSalesInvoiceModal()
        {
            var salesList = new List<object>();

            string webUrlGet = ConfigurationManager.AppSettings["TICKETSALESINVOICE"]; // 🔁 add in Web.config
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"]?.ToString();

            string strparams = "cmprecid=" + Session["CompanyID"] + "&Userid=" + Session["UserRECID"];
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

                            var apiResponse = JsonConvert.DeserializeObject<SalesheaderRootObject>(jsonString);

                            if (apiResponse != null && apiResponse.Status == "Y" && apiResponse.Data != null)
                            {
                               salesList = apiResponse.Data.Select(c => new
                                {
                                    SIH_RECID = c.SIH_RECID,
                                    SIH_INVOICENO = c.SIH_INVOICENO,
                                    SIH_INVOICEAMOUNT = c.SIH_INVOICEAMOUNT,
                                    SIH_INVOICEDATE = c.SIH_INVOICEDATE
                                }).ToList<object>();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Status = salesList.Count > 0 ? "Y" : "N",
                Data = salesList
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DraftDownloadPDF()
        {
            return View();
        }
        // GET: PDF
        [HttpPost]
        public async Task<ActionResult> DraftDownloadPDF(PDFDetails pDFDetails)
        {
            try
            {
                var MaterialcategoryPostURL = ConfigurationManager.AppSettings["RECENTTICKETPDFDRAFT"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{
            ""ticketRecID"": ""{Session["RECORDID"]}"",
            ""companyRecID"": ""{Session["CompanyID"]}""
        }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(MaterialcategoryPostURL),
                    Method = HttpMethod.Post,
                    Headers =
            {
                { "X-Version", "1" },
                { HttpRequestHeader.Accept.ToString(), "application/json" }
            },
                    Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
                };

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };

                var client = new HttpClient(handler);
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<pdfdetail>(responseString);

                    if (result.Status == "Y" && !string.IsNullOrEmpty(result.fileUrl))
                    {
                        // Extract file name from full local path
                        var fileName = result.fileUrl;

                        // Build web-accessible URL (adjust base URL if needed)
                        //var requestUrl = HttpContext.Request.Url;
                        //var baseUrl = $"{requestUrl.Scheme}://{requestUrl.Authority}";
                        //var fileUrl = $"{baseUrl}/GeneratedPDFs/{fileName}";

                        return Json(new { status = "success", url = fileName });
                    }


                    return Json(new { status = "error", message = result.Message });
                }
                else
                {
                    return Json(new { status = "error", message = "No material consumptions are there" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = "Exception occurred: " + ex.Message });
            }
        }



        public ActionResult ContractDownloadPDF()
        {
            return View();
        }
        // GET: PDF
        [HttpPost]
        public async Task<ActionResult> ContractDownloadPDF(PDFDetails pDFDetails)
        {
            try
            {
                var MaterialcategoryPostURL = ConfigurationManager.AppSettings["TICCONTRACTPDF"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{
            ""ticketRecID"": ""{Session["RECORDID"]}"",
            ""companyRecID"": ""{Session["CompanyID"]}"",
            ""userRecID"": ""{Session["ticuserid"]}"",
            ""productRecID"": ""{Session["Prodid"]}"",
            ""productSerial"": ""{Session["prodserial"]}""
        }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(MaterialcategoryPostURL),
                    Method = HttpMethod.Post,
                    Headers =
            {
                { "X-Version", "1" },
                { HttpRequestHeader.Accept.ToString(), "application/json" }
            },
                    Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
                };

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };

                var client = new HttpClient(handler);
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<pdfdetail>(responseString);

                    if (result.Status == "Y" && !string.IsNullOrEmpty(result.fileUrl))
                    {
                        // Extract file name from full local path
                        var fileName = result.fileUrl;

                        // Build web-accessible URL (adjust base URL if needed)
                        //var requestUrl = HttpContext.Request.Url;
                        //var baseUrl = $"{requestUrl.Scheme}://{requestUrl.Authority}";
                        //var fileUrl = $"{baseUrl}/GeneratedPDFs/{fileName}";

                        return Json(new { status = "success", url = fileName });
                    }


                    return Json(new { status = "error", message = result.Message });
                }
                else
                {
                    return Json(new { status = "error", message = "No material consumptions are there" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = "Exception occurred: " + ex.Message });
            }
        }


        public ActionResult WarrantyDownloadPDF()
        {
            return View();
        }
        // GET: PDF
        [HttpPost]
        public async Task<ActionResult> WarrantyDownloadPDF(PDFDetails pDFDetails)
        {
            try
            {
                var MaterialcategoryPostURL = ConfigurationManager.AppSettings["TICWARRANTYPDF"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{
            ""ticketRecID"": ""{Session["RECORDID"]}"",
            ""companyRecID"": ""{Session["CompanyID"]}"",
            ""userRecID"": ""{Session["ticuserid"]}"",
            ""productRecID"": ""{Session["Prodid"]}"",
            ""productSerial"": ""{Session["prodserial"]}""
        }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(MaterialcategoryPostURL),
                    Method = HttpMethod.Post,
                    Headers =
            {
                { "X-Version", "1" },
                { HttpRequestHeader.Accept.ToString(), "application/json" }
            },
                    Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
                };

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };

                var client = new HttpClient(handler);
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<pdfdetail>(responseString);

                    if (result.Status == "Y" && !string.IsNullOrEmpty(result.fileUrl))
                    {
                        // Extract file name from full local path
                        var fileName = result.fileUrl;

                        // Build web-accessible URL (adjust base URL if needed)
                        //var requestUrl = HttpContext.Request.Url;
                        //var baseUrl = $"{requestUrl.Scheme}://{requestUrl.Authority}";
                        //var fileUrl = $"{baseUrl}/GeneratedPDFs/{fileName}";

                        return Json(new { status = "success", url = fileName });
                    }


                    return Json(new { status = "error", message = result.Message });
                }
                else
                {
                    return Json(new { status = "error", message = "No material consumptions are there" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = "Exception occurred: " + ex.Message });
            }
        }
        public async Task<ActionResult> ViewDetails(int ticketRecID)
        {
            ServiceInvoiceData invoiceData = new ServiceInvoiceData();

            Session["TicketRecID"] = ticketRecID;

            string Weburl = ConfigurationManager.AppSettings["VIEWDATA"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();


            string strparams = "cmprecid=" + Session["CompanyID"] + "&ticketRecID=" + ticketRecID;
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

                            // Deserialize to your root object
                            var rootObjects = JsonConvert.DeserializeObject<ServiceInvoiceRootObjects>(jsonString);

                            if (rootObjects != null && rootObjects.Status == "Y")
                            {
                                // Billable and NonBillable materials
                                var billable = rootObjects.Data?.BillableMaterials ?? new List<ServiceMaterials>();
                                var nonBillable = rootObjects.Data?.NonBillableMaterials ?? new List<ServiceMaterials>();

                                // Pass to View
                                ViewBag.Billable = billable;
                                ViewBag.NonBillable = nonBillable;

                                // If you want to strongly bind whole object (instead of only ViewBag)
                                return View(rootObjects.Data);
                            }
                            else
                            {
                                // API returned Status != "Y"
                                ViewBag.Billable = new List<ServiceMaterials>();
                                ViewBag.NonBillable = new List<ServiceMaterials>();
                                return View(new ServiceInvoiceData());
                            }
                        }
                        else
                        {
                            // API failure case
                            ViewBag.Billable = new List<ServiceMaterials>();
                            ViewBag.NonBillable = new List<ServiceMaterials>();
                            return View(new ServiceInvoiceData());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            return View(invoiceData);
        }


        public async Task ComboBoxProduct(Tickets viewModel)
        {
            string webUrlGet = ConfigurationManager.AppSettings["GETCUSTOMERPRODUCTLIST"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "companyId=" + Session["CompanyID"] + "&UserID=" + Session["UserRECID"];
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
                                Value = item.P_RECID.ToString(),
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

        public ActionResult ContractAndWarranty()
        {
            return View();
        }


        public class ProductSelectItem
        {
            public string Value { get; set; }
            public string Text { get; set; }
            public string SerialNo { get; set; }
        }
    }
}