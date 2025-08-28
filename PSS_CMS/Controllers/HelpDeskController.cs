using CaptchaMvc.HtmlHelpers;
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
    public class HelpDeskController : Controller
    {
        // GET: HelpDesk
        public async Task<ActionResult> HDDashboard()
        {
            string WEBURLGET = ConfigurationManager.AppSettings["HELPDESKDASHBOARD"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "Userid=" + Session["UserRECID"] + "&type=" + Session["UserRole"] + "&cmprecid=" + Session["CompanyID"];
            string finalurl = WEBURLGET + "?" + strparams;
            Dashborardchart dashboardData = null;

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
                            dashboardData = JsonConvert.DeserializeObject<Dashborardchart>(jsonString);

                            var totalTickets = dashboardData.TotalTickets;
                            var closedTickets = dashboardData.CloseTickets;
                            var closedPercentage = (closedTickets / (double)totalTickets) * 100;

                            ViewBag.ClosedPercentage = closedPercentage;
                            ViewBag.TotalTickets = totalTickets;
                            ViewBag.ClosedTickets = closedTickets;
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
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }

            // Fetch WTD and MTD chart data
            Dashborardchart wtdMtdData = await HDWTDANDMTDCHARTHD();
            if (wtdMtdData != null)
            {
                ViewBag.MonthTotalTickets = wtdMtdData.MonthWise?.MonthTotalTickets ?? 0;
                ViewBag.MonthCloseTickets = wtdMtdData.MonthWise?.MonthCloseTickets ?? 0;

                ViewBag.WeekTotalTickets = wtdMtdData.WeekWise?.WeekTotalTickets ?? 0;              
                ViewBag.WeekCloseTickets = wtdMtdData.WeekWise?.WeekCloseTickets ?? 0;
            }

            return View(dashboardData);
        }

        public async Task<Dashborardchart> HDWTDANDMTDCHARTHD()
        {
            string WEBURLGET = ConfigurationManager.AppSettings["HELPDESKWDMD"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "Userid=" + Session["UserRECID"] + "&type=" + Session["UserRole"] + "&cmprecid=" + Session["CompanyID"];
            string finalurl = WEBURLGET + "?" + strparams;
            Dashborardchart wtdMtdData = null;

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
                            wtdMtdData = JsonConvert.DeserializeObject<Dashborardchart>(jsonString);
                        }
                        else
                        {
                            Console.WriteLine("Error: " + response.ReasonPhrase);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }
            await HDStackedBarChart();
            return wtdMtdData;
        }
        public async Task<ActionResult> HDStackedBarChart()
        {
            string WEBURLGET = ConfigurationManager.AppSettings["HELPDESKSTACKEDBAR"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "Userid=" + Session["UserRECID"] + "&cmprecid=" + Session["CompanyID"];
            string finalurl = WEBURLGET + "?" + strparams;
            List<DashboardPriority> dashboardDataPriority = new List<DashboardPriority>();

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
                            var rootObjects = JsonConvert.DeserializeObject<DashboardPriorityList>(jsonString);
                            dashboardDataPriority = rootObjects.Data;

                            ViewBag.Labels1 = JsonConvert.SerializeObject(new[] { "Critical", "Emergency", "Urgent", "Normal" });
                            ViewBag.ClosedTickets1 = JsonConvert.SerializeObject(dashboardDataPriority.Select(d => d.ClosedTickets));                        
                            ViewBag.TotalTickets1 = JsonConvert.SerializeObject(dashboardDataPriority.Select(d => d.TotalTickets));


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
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }

            return View(dashboardDataPriority);
        }

        [HttpGet]
        public async Task<ActionResult> Ticket()
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
            await ComboBoxProductNewticket(viewModel);

            return View(viewModel);
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
            ""tC_PRECID"": ""{tickets.SelectedProjectType}"",        
            ""tC_CURECID"": ""{tickets.SelectedCustomer}"",        
            ""tC_TICKETDATE"": ""{tickets.TC_Dates}"",        
            ""tC_SUBJECT"": ""{tickets.TC_SUBJECT}"",        
            ""tC_OTP"": ""{"6757"}"",
            ""tC_COMMENTS"": ""{HttpUtility.JavaScriptStringEncode(tickets.TC_COMMENTS)}"",
            ""tC_REQUEST_ATTPREFIX"": ""{base64Image}"",  
            ""tC_REQUEST_DATETIME"": ""{DateTime.Now.ToString("yyyy-MM-dd")}"",          
            ""tC_STATUS"": ""{"S"}"",
            ""tC_PRIORITYTYPE"": ""{tickets.TC_PRIORITYTYPE}"",
            ""tC_TICKETTYPE"": ""{tickets.SelectedTicketType}"",
            ""tC_USERNAME"": ""{Session["UserName"]}"",
            ""tC_REFERENCETRECID"": ""{0}""
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
                        return Json(new { status = "error", message = "Error: " + response.ReasonPhrase });
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

        public async Task ComboBoxProductNewticket(Tickets viewModel)
        {
            string webUrlGet = ConfigurationManager.AppSettings["HELPDESKNEWTICKETPRODUCT"];
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

        public async Task<ActionResult> ComboBoxTicketHistoryProduct()
        {

            List<SelectListItem> Product = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["HELPDESKNEWTICKETPRODUCT"];
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
        public async Task<JsonResult> ComboProductTicketNew(string Recid)
        {
            var customerResult = new List<object>();

            string webUrlGet = ConfigurationManager.AppSettings["CUSTOMERPRODUCTCOMBO"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"]?.ToString();
            string cmpRecId = Session["CompanyID"]?.ToString();
            string strparams = "companyId=" + cmpRecId + "&productid=" + Recid;
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
                            var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(jsonString);

                            if (apiResponse?.Data != null)
                            {
                                customerResult = apiResponse.Data.Select(data => new
                                {
                                    Value = data.CU_RECID.ToString(),
                                    Text = data.CU_NAME,
                                    WarrantyUpto = data.CU_WARRANTYUPTO,
                                    WarrantyFreeCalls = data.CU_WARRANTYFREECALLS
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

            return Json(customerResult, JsonRequestBehavior.AllowGet);
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

        public async Task<ActionResult> ReviewTickets(string recid2, string status, string REOPENUSERNAME, string projectid)
        {
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ClientResponseTicket(Reviewtickets tickets, HttpPostedFileBase myfile, string statusparam)
        {
            var combox ="C";

            try
            {
                // Handle File Upload
                string base64Image = ProcessFileUpload(Request.Files);

                    var apiUrl = ConfigurationManager.AppSettings["UpdateComboresponseHD"];
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
                        return Json(new { status = "Y", message = "Ticket closed successfully!" });
                    }

                    else
                    {
                        return Json(new { status = "N", message = apiResponse.Message });
                    }

                
            }
            catch (Exception ex)
            {
                return Json(new { status = "Error", message = "Exception occurred: " + ex.Message });
            }
        }

    }

}