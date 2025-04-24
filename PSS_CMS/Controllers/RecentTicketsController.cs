using Newtonsoft.Json;
using PSS_CMS.Fillter;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
    public class RecentTicketsController : Controller
    {
       
        public async Task<ActionResult> RecentTicket(string userid, string userrole, string searchPharse, string status, string projectType, string ticketType, string StartDate, string EndDate)
        {
            Recenttickets objRecents = new Recenttickets();
            string Weburl = ConfigurationManager.AppSettings["AdminTicketURL"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            List<Recenttickets> RecentTicketList = new List<Recenttickets>();

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
                            var rootObjects = JsonConvert.DeserializeObject<APIResponseRecenttickets>(jsonString);
                            RecentTicketList = rootObjects.Data;


                            if (
                                 string.IsNullOrWhiteSpace(ticketType) &&
                                 string.IsNullOrWhiteSpace(status) &&
                                 string.IsNullOrWhiteSpace(projectType) &&
                                 string.IsNullOrWhiteSpace(StartDate) &&
                                 string.IsNullOrWhiteSpace(EndDate) &&
                                 string.IsNullOrWhiteSpace(searchPharse))
                            {
                                // Exclude Closed tickets on the first load if no filters are applied
                                RecentTicketList = RecentTicketList.Where(t => t.TC_STATUS != "C").ToList();
                            }

                            // Apply Search Filter
                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                var lowerSearch = searchPharse.ToLower();

                                RecentTicketList = RecentTicketList
                                    .Where(r =>
                                        (r.TC_PROJECTID?.ToLower().Contains(lowerSearch) ?? false) ||
                                        (r.TC_USERNAME?.ToLower().Contains(lowerSearch) ?? false) ||
                                        (r.TC_COMMENTS?.ToLower().Contains(lowerSearch) ?? false) ||
                                        (r.TC_PRIORITYTYPE?.ToLower().Contains(lowerSearch) ?? false) ||
                                        (r.TC_TICKETTYPE?.ToLower().Contains(lowerSearch) ?? false) ||
                                        (r.TC_STATUS?.ToLower().Contains(lowerSearch) ?? false) ||
                                        (r.TC_TICKETDATES?.ToLower().Contains(lowerSearch) ?? false)
                                    )
                                    .ToList();
                            }


                            // Apply Status Filter
                            if (!string.IsNullOrEmpty(status))
                            {
                                RecentTicketList = RecentTicketList.Where(t => t.TC_STATUS == status).ToList();
                            }
                           
                            // Apply Project Type Filter
                            if (!string.IsNullOrEmpty(projectType))
                            {
                                RecentTicketList = RecentTicketList.Where(t => t.TC_PROJECTID == projectType).ToList();
                            }

                            // Apply Ticket Type Filter
                            if (!string.IsNullOrEmpty(ticketType))
                            {
                                RecentTicketList = RecentTicketList.Where(t => t.TC_TICKETTYPE == ticketType).ToList();
                            }
                            //else
                            //{
                            //    // Exclude Closed tickets on first load
                            //    RecentTicketList = RecentTicketList.Where(t => t.TC_STATUS != "C").ToList();
                            //}
                            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
                            {
                                //DateTime fromDate = DateTime.Parse(StartDate);//parse it is used to convert the string to datetime object
                                //DateTime toDate = DateTime.Parse(EndDate);


                                RecentTicketList = RecentTicketList
          .Where(t => string.Compare(t.TC_TICKETDATE, StartDate) >= 0 &&
                      string.Compare(t.TC_TICKETDATE, EndDate) <= 0)
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

            await TicketType();
            await ProjectType();
            return View(RecentTicketList);
        }


        public async Task<ActionResult> AdminTickets(string recid2, string userid, string status)
        {
            await ProjectTypeAdminFAQ();
            IEnumerable<Admintickets> ticketadminList = await GetAdminTickets(recid2, userid, status); // Your logic to get a list of tickets
            return View(ticketadminList); // Pass the collection to the view
        }

        public async Task<IEnumerable<Admintickets>> GetAdminTickets(string recid2, string userid, string status)
        {
            ViewBag.NameOptions = new List<SelectListItem>
{
    new SelectListItem { Value = "Neelakrishnan", Text = "Neelakrishnan" },
    new SelectListItem { Value = "Yogesh", Text = "Yogesh" }
};


            Session["RECORDID"] = recid2;
            Session["STATUS"] = status;
            

            string WEBURLGETBYID = ConfigurationManager.AppSettings["AdminGetSingleURL"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<Admintickets> ticketadminList = new List<Admintickets>();
           
            string strparams = "TC_USERID=" + userid + "&StrRecid=" + recid2 + "&cmprecid=" + Session["CompanyID"];
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
                            var content = JsonConvert.DeserializeObject<RootObjectsuser>(jsonString);
                            ticketadminList = content.Data;
                            Session["LastRecid"] = content.LastRecid;
                            Session["LastStatus"] = content.LastStatus;
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
            return ticketadminList;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AdminResponseTicket(Admintickets Admintickets, string RESPONSE_DATETIME, string RESPONSE_COMMENTS, HttpPostedFileBase myfile)
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
                        Admintickets.TC_REQUEST_ATTACHMENT_PREFIX = base64Image;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "No file uploaded.");
                    }

                }
                // Define your API URL and keys
                var AdminResponsePostURL = ConfigurationManager.AppSettings["AdminResponse"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();
                var content = $@"{{           
            ""tC_RECID"": ""{Session["LastRecid"]}"",           
            ""tC_CRECID"": ""{Session["CompanyID"]}"",           
            ""tC_RESPONSE_ATTACHMENT_PREFIX"": ""{base64Image}"",
            ""tC_RESPONSE_USERID"": ""{Session["UserID"]}"",
            ""tC_RESPONSE_DATETIME"": ""{DateTime.Now.ToString("yyyy-MM-dd")}"",
            ""tC_RESPONSE_COMMENTS"": ""{HttpUtility.JavaScriptStringEncode(RESPONSE_COMMENTS)}"",                              
            ""tC_ADMINNAME"": ""{Session["UserName"]}"",                              
            ""tC_STATUS"": ""{"R"}""           
        }}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(AdminResponsePostURL),
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
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseTicketsResponse>(responseBody);

                    if (apiResponse.Status == "Y")
                    {
                        return Json(new { success = true, message = "Response submitted successfully!" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Submission failed. Please try again." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Error: " + response.ReasonPhrase });
                }
            }
    
    catch (Exception ex)
    {
        return Json(new { success = false, message = "Exception occurred: " + ex.Message
    });
    }
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

        public async Task<ActionResult> TicketType()
        {
            List<SelectListItem> ticketTypes = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["COMBOBOXTICKETTYPE"];
            string strparams = "cmprecid=" + Session["CompanyID"];
            string finalurl = webUrlGet + "?" + strparams;
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

                        var response = await client.GetAsync(finalurl);
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var rootObjects = JsonConvert.DeserializeObject<TicketTypeModels>(jsonString);

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
        public async Task<ActionResult> ProjectType()
        
        {
            List<SelectListItem> projectTypes = new List<SelectListItem>();

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
                            var rootObjects = JsonConvert.DeserializeObject<TicketTypeModels>(jsonString);

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

        public async Task<ActionResult> FAQADMIN(string searchPharse,int ?projectID)
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
            string Weburl = ConfigurationManager.AppSettings["FAQGET1"];
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
                            { FAQList = FAQList.Where(r => r.F_QUESTION.ToLower().Contains(searchPharse.ToLower()) ||
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
           
            await ProjectTypeAdminFAQ();
      
            return View(FAQList);
        }

      
        public async Task <ActionResult> FAQADMINPOST()
        {
            await ProjectTypeAdminFAQ();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> FAQADMINPOST(Faq faq, HttpPostedFileBase myfile)
        {
            try
            {
                await ProjectTypeAdminFAQ();
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
                        faq.F_ATTACHEMENT = base64Image;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "No file uploaded.");
                    }

                }
                // Define your API URL and keys
                var FaqPostURL = ConfigurationManager.AppSettings["FAQPOST"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();
                //faq.F_QUESTION = faq.F_QUESTION?.Replace("\"", ""); // Removes double quotes
                //faq.F_ANSWER = faq.F_ANSWER?.Replace("\"", ""); // Removes double quotes

                // Construct the JSON content for the API request
                var content = $@"{{                     
            ""f_QUESTION"": ""{HttpUtility.JavaScriptStringEncode(faq.F_QUESTION)}"",           
            ""f_ANSWER"": ""{HttpUtility.JavaScriptStringEncode(faq.F_ANSWER)}"",
            ""f_ATTACHEMENT"": ""{faq.F_ATTACHEMENT}"",
            ""f_CREATEDDATETIME"": ""{DateTime.Now.ToString("yyyy-MM-dd")}"",
            ""f_SORTORDER"": ""{"1"}"",                              
            ""F_PROJECTRECID"": ""{faq.SelectedProjectType1}"",                              
            ""F_USERID"": ""{Session["UserID"]}"",                              
            ""f_CRECID"": ""{Session["CompanyID"]}"",                              
            ""f_DISABLE"": ""{"N"}""               
        }}";
              
                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(FaqPostURL),
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
                    var apiResponse = JsonConvert.DeserializeObject<RootObjectFAQ>(responseBody);

                    if (apiResponse.Status == "Y")
                    {
                        return Json(new { success = true, message = "FAQ created successfully!" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Failed to create FAQ." });
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

        //Admintickets FAQ Ticket type combo
        public async Task<ActionResult> ProjectTypeAdminFAQ()

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
                            var rootObjects = JsonConvert.DeserializeObject<TicketTypeModels>(jsonString);

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


       

        public async Task<ActionResult> Edit(int F_RECID,int F_PROJECTRECID)
        {
            
            
            string WEBURLGETBYID = ConfigurationManager.AppSettings["FAQGETBYID"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            Faq faq = null;

            string strparams = "recID=" + F_RECID + "&cmprecid=" + Session["CompanyID"] + "&projectID="+ F_PROJECTRECID;
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
                            var content = JsonConvert.DeserializeObject<RootObjectFAQGET>(jsonString);
                            string base64Image = content.Data.F_ATTACHEMENT;
                            Session["F_USERID"] =content.Data.F_USERID;
                            Session["F_RECID"] =content.Data. F_RECID;
                            Session["F_PROJECTRECID"] =content.Data.F_PROJECTRECID;
                            if (!string.IsNullOrEmpty(base64Image))
                            {

                                string mimeType = GetImageMimeType(base64Image);

                                ViewBag.IMAGE = base64Image;
                                ViewBag.MIMEType = mimeType;
                               
                            }
                            faq = content.Data;
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
           
            return View(faq);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(Faq faq, HttpPostedFileBase myfile)
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
                        faq.F_ATTACHEMENT = base64Image;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "No file uploaded.");
                    }

                }
                // Define your API URL and keys
                var FaqPostURL = ConfigurationManager.AppSettings["FAQUPDATE"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();
                //faq.F_QUESTION = faq.F_QUESTION?.Replace("\"", ""); // Removes double quotes
                //faq.F_ANSWER = faq.F_ANSWER?.Replace("\"", ""); // Removes double quotes

                // Construct the JSON content for the API request
                var content = $@"{{           
            ""f_RECID"": ""{ Session["F_RECID"]}"",           
            ""f_QUESTION"": ""{HttpUtility.JavaScriptStringEncode(faq.F_QUESTION)}"",           
            ""f_ANSWER"": ""{HttpUtility.JavaScriptStringEncode(faq.F_ANSWER)}"",
            ""f_ATTACHEMENT"": ""{faq.F_ATTACHEMENT}"",
            ""f_CREATEDDATETIME"": ""{DateTime.Now.ToString("yyyy-MM-dd")}"",
            ""f_SORTORDER"": ""{"1"}"",                              
            ""f_PROJECTRECID"": ""{Session["F_PROJECTRECID"]}"",                              
            ""f_USERID"": ""{Session["F_USERID"]}"",                              
            ""f_CRECID"": ""{Session["CompanyID"]}"",                              
            ""f_DISABLE"": ""{"N"}""           
        }}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(FaqPostURL),
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
                    var apiResponse = JsonConvert.DeserializeObject<RootObjectFAQ>(responseBody);

                    if (apiResponse.Status == "Y")
                    {
                        return Json(new { success = true, message = "FAQ edited successfully!" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Failed to edit FAQ." });
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

        public async Task<ActionResult> Delete(int F_RECID)
        {

            string WEBURLDELETE = ConfigurationManager.AppSettings["FAQDELETE"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            string strparams = "RECID=" + F_RECID + "&cmprecid=" + Session["CompanyID"];
            string finalurl = WEBURLDELETE + "?" + strparams;
           
            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                        client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                        var request = new HttpRequestMessage
                        {
                            Method = HttpMethod.Delete,
                            RequestUri = new Uri(finalurl)
                        };

                        var response = await client.SendAsync(request);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseBody = await response.Content.ReadAsStringAsync();
                            var apiResponse = JsonConvert.DeserializeObject<RootObjectFAQ>(responseBody);

                            if (apiResponse.Status == "Y")
                            {

                                string redirectUrl = Url.Action("FAQADMIN", "RecentTickets", new {});
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

        public async Task<ActionResult> ExcelAdminDownload()
        {
            string Weburl = ConfigurationManager.AppSettings["ExcelAdminTicketURL"];
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


    }
}
