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
using Newtonsoft.Json;
using PSS_CMS.Fillter;
using PSS_CMS.Models;

namespace PSS_CMS.Controllers
{
    [ApiKeyAuthorize]
    public class AccountantCustomerInvoiceController : Controller
    {
        // GET: AccountantCustomerInvoice
        public async Task<ActionResult> List(string searchPharse)
        {
            Projectmaster objprojectmaster = new Projectmaster();

            string Weburl = ConfigurationManager.AppSettings["CUSTOMERGET"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<Projectmaster> projectmasterlist = new List<Projectmaster>();

            string strparams = "CompanyRecID=" + Session["CompanyID"];
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
                            var rootObjects = JsonConvert.DeserializeObject<ProjectMasterRootObject>(jsonString);
                            projectmasterlist = rootObjects.Data;

                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                projectmasterlist = projectmasterlist
                                    .Where(r => r.CU_CODE.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.CU_EMAIL.ToString().Contains(searchPharse.ToLower()) ||
                                                r.CU_NAME.ToString().Contains(searchPharse.ToLower()) ||
                                                r.CU_MOBILENO.ToString().Contains(searchPharse.ToLower()) ||
                                                r.CU_INVOICENO.ToString().Contains(searchPharse.ToLower()) ||
                                                r.CU_WARRANTYFREECALLS.ToString().Contains(searchPharse.ToLower()) ||
                                                r.CU_WARRANTYUPTO.ToString().Contains(searchPharse.ToLower()) ||
                                                r.CU_SORTORDER.ToString().Contains(searchPharse.ToLower()))
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
            return View(projectmasterlist);
        }
        public async Task<ActionResult> Create()
        {
            await ComboProductSelection();
            await ComboUser();
            await LocationType();
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(Projectmaster projectmaster)
        {
            try
            {
                var ProjectmasterPostURL = ConfigurationManager.AppSettings["CUSTOMERPOST"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{           
            ""cU_CODE"": ""{projectmaster.CU_CODE}"",           
            ""cU_NAME"": ""{projectmaster.CU_NAME}"",           
            ""cU_EMAIL"": ""{ projectmaster.CU_EMAIL}"",                    
            ""cU_PRECID"": ""{(string.IsNullOrWhiteSpace(projectmaster.SelectedProduct) ? "0" : projectmaster.SelectedProduct)}"",                
            ""cU_SPRECID"": ""{ projectmaster.SelectedLocationRef}"",                    
            ""cU_LTRECID"": ""{ projectmaster.SelectedLocation}"",                    
            ""cU_MOBILENO"": ""{ projectmaster.CU_MOBILENO}"",                    
            ""cU_URECID"": ""{ projectmaster.CU_URECID}"",                    
            ""cU_INVOICENO"": ""{ projectmaster.CU_INVOICENO}"",                    
            ""cU_WARRANTYUPTO"": ""{ projectmaster.CU_WARRANTYUPTO}"",                    
            ""cU_WARRANTYFREECALLS"": ""{ projectmaster.CU_WARRANTYFREECALLS}"",                    
            ""cU_ADDRESS"": ""{ projectmaster.CU_ADDRESS}"",                    
            ""cU_GST"": ""{ projectmaster.CU_GST}"",                    
            ""cU_SORTORDER"": ""{ projectmaster.CU_SORTORDER}"",                    
            ""cU_PANNUMBER"": ""{ projectmaster.CU_PANNUMBER}"",                    
            ""cU_TANNUMBER"": ""{ projectmaster.CU_TANNUMBER}"",                    
            ""cU_CONTACTPERSONNAME1"": ""{ projectmaster.CU_CONTACTPERSONNAME1}"",                    
            ""cU_CONTACTPERSONMOBILE1"": ""{ projectmaster.CU_CONTACTPERSONMOBILE1}"",                    
            ""cU_CONTACTPERSONEMAILID1"": ""{ projectmaster.CU_CONTACTPERSONEMAILID1}"",                    
            ""cU_CONTACTPERSONDESIGINATION1"": ""{ projectmaster.CU_CONTACTPERSONDESIGINATION1}"",   
              
            ""cU_CONTACTPERSONNAME2"": ""{ projectmaster.CU_CONTACTPERSONNAME2}"",                    
            ""cU_CONTACTPERSONMOBILE2"": ""{ projectmaster.CU_CONTACTPERSONMOBILE2}"",                    
            ""cU_CONTACTPERSONEMAILID2"": ""{ projectmaster.CU_CONTACTPERSONEMAILID2}"",                    
            ""cU_CONTACTPERSONDESIGINATION2"": ""{ projectmaster.CU_CONTACTPERSONDESIGINATION2}"",                    
                        
            ""cU_CONTACTPERSONNAME3"": ""{ projectmaster.CU_CONTACTPERSONNAME3}"",                    
            ""cU_CONTACTPERSONMOBILE3"": ""{ projectmaster.CU_CONTACTPERSONMOBILE3}"",                    
            ""cU_CONTACTPERSONEMAILID3"": ""{ projectmaster.CU_CONTACTPERSONEMAILID3}"",                    
            ""cU_CONTACTPERSONDESIGINATION3"": ""{ projectmaster.CU_CONTACTPERSONDESIGINATION3}"",                    
                          
                          
            ""cU_DISABLE"": ""{(projectmaster.IsDisabled ? "Y" : "N")}"",        
            ""cU_CRECID"": ""{Session["CompanyID"]}""           
        }}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(ProjectmasterPostURL),
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


                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ProjectMasterObjects>(responseBody);

                    if (apiResponse.Status == "Y")
                    {
                        return Json(new { success = true, message = apiResponse.Message });
                    }
                    else if (apiResponse.Status == "U" || apiResponse.Status == "N")
                    {
                        return Json(new { success = false, message = apiResponse.Message });
                    }
                    else
                    {
                        return Json(new { success = false, message = "An unexpected status was returned." });
                    }
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Code, Name, Invoice Number, Email,Warranty Upto and Mobile Number fields are mandatory."
                    });
                }


            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            return View(projectmaster);
        }
        public async Task<ActionResult> Edit(int? Recid, string Name)
        {
            Session["Productrecid"] = Recid;
            Session["Name"] = Name;
            string WEBURLGETBYID = ConfigurationManager.AppSettings["CUSTOMERGETBYID"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            Projectmaster projectmaster = null;

            string strparams = "Recid=" + Recid + "&companyId=" + Session["CompanyID"];
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
                            var content = JsonConvert.DeserializeObject<ProjectMasterObjects>(jsonString);
                            projectmaster = content.Data;
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
            await ComboProductSelectionEdit(projectmaster.CU_PRECID);
            await ComboUserEdit(projectmaster.CU_NAME);
            ViewBag.LocationType = await LocationTypeEdit(projectmaster.CU_LTRECID);
            ViewBag.LocationRef = await LocationRefEdit(projectmaster.CU_LTRECID, projectmaster.CU_SPRECID);
            return View(projectmaster);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(Projectmaster projectmaster)
        {
            try
            {
                var ProjectmasterUpdateURL = ConfigurationManager.AppSettings["CUSTOMERPUT"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{           
            ""cU_RECID"": ""{Session["Productrecid"]}"",           
            ""cU_CODE"": ""{projectmaster.CU_CODE}"",           
            ""cU_NAME"": ""{projectmaster.CU_NAME}"",
            ""cU_EMAIL"": ""{projectmaster.CU_EMAIL}"",
            ""cU_MOBILENO"": ""{projectmaster.CU_MOBILENO}"",
            ""cU_URECID"": ""{ projectmaster.CU_URECID}"", 
            ""cU_PRECID"": ""{projectmaster.SelectedProduct}"",                
            ""cU_SPRECID"": ""{ projectmaster.CU_SPRECID}"",                    
            ""cU_LTRECID"": ""{ projectmaster.CU_LTRECID}"",    
            ""cU_INVOICENO"": ""{projectmaster.CU_INVOICENO}"",
            ""cU_WARRANTYUPTO"": ""{projectmaster.CU_WARRANTYUPTO}"",
            ""cU_WARRANTYFREECALLS"": ""{projectmaster.CU_WARRANTYFREECALLS}"",
            ""cU_SORTORDER"": ""{projectmaster.CU_SORTORDER}"",
            ""cU_ADDRESS"": ""{ projectmaster.CU_ADDRESS}"",                    
            ""cU_GST"": ""{ projectmaster.CU_GST}"",     
            ""cU_DISABLE"": ""{(projectmaster.IsDisabled ? "Y" : "N")}"",  
            ""cU_PANNUMBER"": ""{ projectmaster.CU_PANNUMBER}"",                    
            ""cU_TANNUMBER"": ""{ projectmaster.CU_TANNUMBER}"",                    
            ""cU_CONTACTPERSONNAME1"": ""{ projectmaster.CU_CONTACTPERSONNAME1}"",                    
            ""cU_CONTACTPERSONMOBILE1"": ""{ projectmaster.CU_CONTACTPERSONMOBILE1}"",                    
            ""cU_CONTACTPERSONEMAILID1"": ""{ projectmaster.CU_CONTACTPERSONEMAILID1}"",                    
            ""cU_CONTACTPERSONDESIGINATION1"": ""{ projectmaster.CU_CONTACTPERSONDESIGINATION1}"",   
              
            ""cU_CONTACTPERSONNAME2"": ""{ projectmaster.CU_CONTACTPERSONNAME2}"",                    
            ""cU_CONTACTPERSONMOBILE2"": ""{ projectmaster.CU_CONTACTPERSONMOBILE2}"",                    
            ""cU_CONTACTPERSONEMAILID2"": ""{ projectmaster.CU_CONTACTPERSONEMAILID2}"",                    
            ""cU_CONTACTPERSONDESIGINATION2"": ""{ projectmaster.CU_CONTACTPERSONDESIGINATION2}"",                    
                        
            ""cU_CONTACTPERSONNAME3"": ""{ projectmaster.CU_CONTACTPERSONNAME3}"",                    
            ""cU_CONTACTPERSONMOBILE3"": ""{ projectmaster.CU_CONTACTPERSONMOBILE3}"",                    
            ""cU_CONTACTPERSONEMAILID3"": ""{ projectmaster.CU_CONTACTPERSONEMAILID3}"",                    
            ""cU_CONTACTPERSONDESIGINATION3"": ""{ projectmaster.CU_CONTACTPERSONDESIGINATION3}"",                    
                          
            ""cU_CRECID"": ""{ Session["CompanyID"]}""                              
        }}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(ProjectmasterUpdateURL),
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
                    var apiResponse = JsonConvert.DeserializeObject<ProjectMasterObjects>(responseBody);

                    if (apiResponse.Status == "Y")
                    {
                        return Json(new { success = true, message = apiResponse.Message });
                    }
                    else
                    {
                        return Json(new { success = false, message = apiResponse.Message });
                    }
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Code, Name, Invoice Number, Email,Warranty Upto and Mobile Number fields are mandatory."
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Exception: " + ex.Message });
            }
        }
        public async Task<ActionResult> Delete(int? Recid)
        {
            string ProjectmasterDeleteUrl = ConfigurationManager.AppSettings["CUSTOMERDELETE"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            string strparams = "RecordId=" + Recid + "&companyId=" + Session["CompanyID"];
            string finalurl = ProjectmasterDeleteUrl + "?" + strparams;

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
                            var apiResponse = JsonConvert.DeserializeObject<ProjectMasterObjects>(responseBody);

                            if (apiResponse.Status == "Y")
                            {

                                string redirectUrl = Url.Action("List", "AccountantCustomerInvoice", new { });
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

        public async Task<ActionResult> ComboProductSelection()

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

        public async Task<ActionResult> ComboProductSelectionEdit(int selectedRoleCode)

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
                                    Selected = (t.P_RECID == selectedRoleCode) // ✅ compare with passed selectedRoleCode
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

        public async Task<ActionResult> ComboUser()
        {
            List<UserDropdownItem> userList = new List<UserDropdownItem>();

            try
            {
                string webUrlGet = ConfigurationManager.AppSettings["GETUSERSBASEDONROLE"];
                string authKey = ConfigurationManager.AppSettings["Authkey"];
                string apiKey = Session["APIKEY"]?.ToString();
                string companyId = Session["CompanyID"]?.ToString();

                if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(companyId))
                {
                    ViewBag.User = userList;
                    return View();
                }

                string strParams = $"role=User&companyId={companyId}";
                string url = $"{webUrlGet}?{strParams}";

                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("ApiKey", apiKey);
                        client.DefaultRequestHeaders.Add("Authorization", authKey);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var response = await client.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var rootObjects = JsonConvert.DeserializeObject<ApiResponseUserObjects>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                userList = rootObjects.Data.Select(t => new UserDropdownItem
                                {
                                    Value = t.U_RECID.ToString(),
                                    Text = t.U_USERNAME,
                                    Email = t.U_EMAILID,
                                    Mobile = t.U_MOBILENO
                                }).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }

            ViewBag.User = userList;
            return View();
        }

        public async Task ComboUserEdit(string selectedUserName)
        {
            List<UserDropdownItem> userList = new List<UserDropdownItem>();

            try
            {
                string webUrlGet = ConfigurationManager.AppSettings["GETUSERSBASEDONROLE"];
                string authKey = ConfigurationManager.AppSettings["Authkey"];
                string apiKey = Session["APIKEY"]?.ToString();
                string companyId = Session["CompanyID"]?.ToString();

                if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(companyId))
                {
                    ViewBag.User = userList; // Keep it as List<UserDropdownItem>
                    return;
                }

                string url = $"{webUrlGet}?role=User&companyId={companyId}";

                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("ApiKey", apiKey);
                        client.DefaultRequestHeaders.Add("Authorization", authKey);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var response = await client.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var rootObjects = JsonConvert.DeserializeObject<ApiResponseUserObjects>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                userList = rootObjects.Data.Select(t => new UserDropdownItem
                                {
                                    Value = t.U_USERNAME.ToString(),
                                    Text = t.U_USERNAME,
                                    Email = t.U_EMAILID,
                                    Mobile = t.U_MOBILENO
                                }).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }

            ViewBag.User = userList; // ✅ Now it's a List, not a SelectList
        }

        public async Task<ActionResult> LocationType()

        {
            List<SelectListItem> LocationType = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["LOCATIONTYPE"];
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
                            var rootObjects = JsonConvert.DeserializeObject<ProjectMasterRootObject>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                LocationType = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.LT_RECID.ToString(), // or the appropriate value field
                                    Text = t.LT_NAME,
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
            ViewBag.LocationType = LocationType;

            return View();
        }

        public async Task<List<SelectListItem>> LocationTypeEdit(int selectedLocationid)
        {
            List<SelectListItem> LocationType = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["LOCATIONTYPE"];
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
                            var rootObjects = JsonConvert.DeserializeObject<ProjectMasterRootObject>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                LocationType = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.LT_RECID.ToString(),
                                    Text = t.LT_NAME,
                                    Selected = (t.LT_RECID == selectedLocationid)
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

            return LocationType;
        }



        public async Task<JsonResult> LocationRef(int? locationid)
        {
            List<SelectListItem> LocationRef = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["LOCATIONREF"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "cmprecid=" + Session["CompanyID"] + "&locationTypeId=" + locationid;
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
                            var rootObjects = JsonConvert.DeserializeObject<ProjectMasterRootObject>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                LocationRef = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.SP_RECID.ToString(),
                                    Text = t.SP_NAME
                                }).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            // 🔹 Return JSON (not View)
            return Json(LocationRef, JsonRequestBehavior.AllowGet);
        }

        // 🔹 Method to fetch and build LocationRef list
        public async Task<List<SelectListItem>> LocationRefEdit(int? locationid, int selectedstoragepointid)
        {
            List<SelectListItem> LocationRef = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["LOCATIONREF"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "cmprecid=" + Session["CompanyID"] + "&locationTypeId=" + locationid;
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
                            var rootObjects = JsonConvert.DeserializeObject<ProjectMasterRootObject>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                LocationRef = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.SP_RECID.ToString(),
                                    Text = t.SP_NAME,
                                    Selected = (t.SP_RECID == selectedstoragepointid)
                                }).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // optional logging
            }

            return LocationRef;
        }

    }
}