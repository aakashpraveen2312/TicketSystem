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
using Microsoft.Extensions.Configuration;
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
            Projectmaster objinclusion = new Projectmaster();

            int SerialNo = objinclusion.SerialNumber;

            if (SerialNo == 0)
            {
                SerialNo = 1; // Initialize to 1 if it's 0
            }

           
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
                            projectmasterlist = rootObjects.Data ?? new List<Projectmaster>();

                            if (projectmasterlist.Count > 0)
                            {
                                // Assign serial numbers
                                for (int i = 0; i < projectmasterlist.Count; i++)
                                {
                                    projectmasterlist[i].SerialNumber = i + 1;
                                }
                            }
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
            
            var apiModel = await GetWarrantyDetails();

            Projectmaster model = new Projectmaster();

           
           // model.CU_WARRANTYFREECALLS = apiModel.Product.P_FREECALLS;
            //ViewBag.Inclusions = apiModel.Inclusions;
            //ViewBag.Exclusions = apiModel.Exclusions;
            await ComboProductSelection();
            await ComboUser();
            await LocationType();
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Create(Projectmaster projectmaster)
        {
            try
            {
                var ProjectmasterPostURL = ConfigurationManager.AppSettings["CUSTOMERPOST"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                DateTime invoiceDate = Convert.ToDateTime(projectmaster.CU_INVOICEDATE);
                int durationMonths = Convert.ToInt32(projectmaster.CU_WARRANTYMONTHS);
                DateTime warrantyUpto = invoiceDate.AddMonths(durationMonths);
                string warrantyUptoDate = warrantyUpto.ToString("yyyy-MM-dd");

                var content = $@"{{           
            ""cU_CODE"": ""{""}"",           
            ""cU_PRODUCTSERIALNUMBER"": ""{projectmaster.CU_PRODUCTSERIALNUMBER}"",           
            ""cU_ADMINRECID"": ""{projectmaster.CU_ADMINRECID}"",           
            ""cU_NAME"": ""{projectmaster.CU_NAME}"",           
            ""cU_EMAIL"": ""{ projectmaster.CU_EMAIL}"",                    
            ""cU_PRECID"": ""{(string.IsNullOrWhiteSpace(projectmaster.SelectedProduct) ? "0" : projectmaster.SelectedProduct)}"",                
            ""cU_SPRECID"": ""{ projectmaster.SelectedLocationRef}"",                    
            ""cU_LTRECID"": ""{ projectmaster.SelectedLocation}"",                    
            ""cU_MOBILENO"": ""{ projectmaster.CU_MOBILENO}"",                    
            ""cU_URECID"": ""{ projectmaster.CU_URECID}"",                    
            ""cU_INVOICENO"": ""{ projectmaster.CU_INVOICENO}"",                    
            ""cU_INVOICEAMOUNT"": ""{ projectmaster.CU_INVOICEAMOUNT}"",                    
            ""cU_INVOICEDATE"": ""{ projectmaster.CU_INVOICEDATE}"",                    
            ""cU_WARRANTYUPTO"": ""{warrantyUptoDate}"",                    
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
            ""cU_CRECID"": ""{Session["CompanyID"]}"",           
            ""cU_CTRECID"": ""{"0"}""         
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
            var apiModel = await GetWarrantyDetails();
            ViewBag.Inclusions = apiModel.Inclusions;
            ViewBag.Exclusions = apiModel.Exclusions;

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

        //Sales Invoice Header
        public async Task<ActionResult> SalesheaderList(string searchPharse)
        {
            Salesheader objsales = new Salesheader();

            int SerialNo = objsales.SerialNumber;

            if (SerialNo == 0)
            {
                SerialNo = 1; // Initialize to 1 if it's 0
            }


            Salesheader objprojectmaster = new Salesheader();

            string Weburl = ConfigurationManager.AppSettings["INVOICEGET"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();


            List<Salesheader> projectmasterlist = new List<Salesheader>();

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
                            var rootObjects = JsonConvert.DeserializeObject<SalesheaderRootObject>(jsonString);
                            projectmasterlist = rootObjects.Data ?? new List<Salesheader>();

                            if (projectmasterlist.Count > 0)
                            {
                                // Assign serial numbers
                                for (int i = 0; i < projectmasterlist.Count; i++)
                                {
                                    projectmasterlist[i].SerialNumber = i + 1;
                                }
                            }
                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                projectmasterlist = projectmasterlist
                                    .Where(r => r.SIH_CODE.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.SIH_INVOICENO.ToString().Contains(searchPharse.ToLower()) ||
                                                r.SIH_SORTORDER.ToString().Contains(searchPharse.ToLower()))
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

        public async Task<ActionResult> SalesheaderAdminListView(string searchPharse)
        {
            Salesheader objsales = new Salesheader();

            int SerialNo = objsales.SerialNumber;

            if (SerialNo == 0)
            {
                SerialNo = 1; // Initialize to 1 if it's 0
            }


            Salesheader objprojectmaster = new Salesheader();

            string Weburl = ConfigurationManager.AppSettings["INVOICEGET"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();


            List<Salesheader> projectmasterlist = new List<Salesheader>();

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
                            var rootObjects = JsonConvert.DeserializeObject<SalesheaderRootObject>(jsonString);
                            projectmasterlist = rootObjects.Data ?? new List<Salesheader>();

                            if (projectmasterlist.Count > 0)
                            {
                                // Assign serial numbers
                                for (int i = 0; i < projectmasterlist.Count; i++)
                                {
                                    projectmasterlist[i].SerialNumber = i + 1;
                                }
                            }
                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                projectmasterlist = projectmasterlist
                                    .Where(r => r.SIH_CODE.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.SIH_INVOICENO.ToString().Contains(searchPharse.ToLower()) ||
                                                r.SIH_SORTORDER.ToString().Contains(searchPharse.ToLower()))
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

        public async Task<ActionResult> SalesheaderCreate()
        {

            Salesheader model = new Salesheader();
            await ComboProductSelection();
            await ComboUser();
            await LocationType();
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> SalesheaderCreate(Salesheader salesheader)
        {
            try
            {
                string customerPostUrl = ConfigurationManager.AppSettings["CUSTOMERPRODUCTPOST"];
                string invoicePostUrl = ConfigurationManager.AppSettings["INVOICEPOST"];
                string authKey = ConfigurationManager.AppSettings["AuthKey"];
                string apiKey = Session["APIKEY"]?.ToString();

                int? customerRecid = 0;

                string access1 = salesheader.CU_CONTACTACCESS1_BOOL ? "Y" : "N";
                string access2 = salesheader.CU_CONTACTACCESS2_BOOL ? "Y" : "N";
                string access3 = salesheader.CU_CONTACTACCESS3_BOOL ? "Y" : "N";

                // ===========================
                // CUSTOMER API CALL
                // ===========================
                var customerContent = $@"{{
            ""cU_RECID"": ""{salesheader.HiddenCustomerRecid}"",
            ""cU_PRECID"": ""{(string.IsNullOrWhiteSpace(salesheader.SelectedProduct) ? "0" : salesheader.SelectedProduct)}"",
            ""cU_NAME"": ""{salesheader.CU_NAME}"",
            ""cU_EMAIL"": ""{salesheader.CU_EMAIL}"",
            ""cU_MOBILENO"": ""{salesheader.CU_MOBILENO}"",
            ""cU_ADDRESS"": ""{salesheader.CU_ADDRESS}"",
            ""cU_GST"": ""{salesheader.CU_GST}"",
            ""cU_PANNUMBER"": ""{salesheader.CU_PANNUMBER}"",
            ""cU_TANNUMBER"": ""{salesheader.CU_TANNUMBER}"",

            ""cU_CONTACTPERSONNAME1"": ""{salesheader.CU_CONTACTPERSONNAME1}"",
            ""cU_CONTACTPERSONEMAILID1"": ""{salesheader.CU_CONTACTPERSONEMAILID1}"",
            ""cU_CONTACTPERSONMOBILE1"": ""{salesheader.CU_CONTACTPERSONMOBILE1}"",
            ""cU_CONTACTPERSONDESIGINATION1"": ""{salesheader.CU_CONTACTPERSONDESIGINATION1}"",
            ""cU_CONTACTACCESS1"": ""{access1}"",

            ""cU_CONTACTPERSONNAME2"": ""{salesheader.CU_CONTACTPERSONNAME2}"",
            ""cU_CONTACTPERSONEMAILID2"": ""{salesheader.CU_CONTACTPERSONEMAILID2}"",
            ""cU_CONTACTPERSONMOBILE2"": ""{salesheader.CU_CONTACTPERSONMOBILE2}"",
            ""cU_CONTACTPERSONDESIGINATION2"": ""{salesheader.CU_CONTACTPERSONDESIGINATION2}"",
            ""cU_CONTACTACCESS2"": ""{access2}"",

            ""cU_CONTACTPERSONNAME3"": ""{salesheader.CU_CONTACTPERSONNAME3}"",
            ""cU_CONTACTPERSONEMAILID3"": ""{salesheader.CU_CONTACTPERSONEMAILID3}"",
            ""cU_CONTACTPERSONMOBILE3"": ""{salesheader.CU_CONTACTPERSONMOBILE3}"",
            ""cU_CONTACTPERSONDESIGINATION3"": ""{salesheader.CU_CONTACTPERSONDESIGINATION3}"",
            ""cU_CONTACTACCESS3"": ""{access3}"",

            ""cU_CRECID"": ""{Session["CompanyID"]}""
        }}";

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Add("ApiKey", apiKey);
                    client.DefaultRequestHeaders.Add("Authorization", authKey);

                    var customerRequest = new HttpRequestMessage(HttpMethod.Post, customerPostUrl)
                    {
                        Content = new StringContent(customerContent, Encoding.UTF8, "application/json")
                    };

                    customerRequest.Headers.Add("X-Version", "1");

                    var customerResponse = await client.SendAsync(customerRequest);

                    if (!customerResponse.IsSuccessStatusCode)
                        return Json(new { success = false, message = "Customer API Failed" });

                    var customerResult = await customerResponse.Content.ReadAsStringAsync();
                    var customerObj = JsonConvert.DeserializeObject<CustomerResponse>(customerResult);

                    if (customerObj.Status != "Y")
                        return Json(new { success = false, message = customerObj.Message });

                    customerRecid = customerObj.CustomerRecid;

                    // ===========================
                    // INVOICE API CALL
                    // ===========================
                    var invoiceContent = $@"{{
                ""siH_SPRECID"": ""{salesheader.SelectedLocationRef}"",
                ""siH_LTRECID"": ""{salesheader.SelectedLocation}"",
                ""siH_INVOICENO"": ""{salesheader.SIH_INVOICENO}"",
                ""siH_INVOICEAMOUNT"": ""{salesheader.SIH_INVOICEAMOUNT}"",
                ""siH_INVOICEDATE"": ""{salesheader.SIH_INVOICEDATE}"",
                ""siH_SORTORDER"": ""{salesheader.SIH_SORTORDER}"",
                ""siH_DISABLE"": ""{(salesheader.IsDisabled ? "Y" : "N")}"",
                ""siH_CRECID"": ""{Session["CompanyID"]}"",
                ""siH_CURECID"": ""{customerRecid}""
            }}";

                    var invoiceRequest = new HttpRequestMessage(HttpMethod.Post, invoicePostUrl)
                    {
                        Content = new StringContent(invoiceContent, Encoding.UTF8, "application/json")
                    };

                    invoiceRequest.Headers.Add("X-Version", "1");

                    var response = await client.SendAsync(invoiceRequest);

                    if (!response.IsSuccessStatusCode)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Code, Name, Invoice Number, Email, Warranty Upto and Mobile Number fields are mandatory."
                        });
                    }

                    var responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ProjectMasterObjects>(responseBody);

                    if (apiResponse.Status == "Y")
                        return Json(new { success = true, message = apiResponse.Message });

                    return Json(new { success = false, message = apiResponse.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<ActionResult> SalesheaderEdit(int Recid)
        {
            Session["invoicerecid"] = Recid;
            string WEBURLGETBYID = ConfigurationManager.AppSettings["INVOICECUSTOMERGETBYID"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            Salesheader salesheader = null;

            string strparams = "recid=" + Recid + "&cmprecid=" + Session["CompanyID"];
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
                            var content = JsonConvert.DeserializeObject<SalesheaderObjects>(jsonString);
                            salesheader = content.Data;
                            Session["CU_RECID"] = salesheader.CU_RECID;
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
            ViewBag.LocationType = await LocationTypeEdit(salesheader.SIH_LTRECID);
            ViewBag.LocationRef = await LocationRefEdit(salesheader.SIH_LTRECID, salesheader.SIH_SPRECID);
            return View(salesheader);
        }

        public async Task<ActionResult> SalesheaderView(int Recid)
        {
            Session["invoicerecid"] = Recid;
            string WEBURLGETBYID = ConfigurationManager.AppSettings["INVOICECUSTOMERGETBYID"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            Salesheader salesheader = null;

            string strparams = "recid=" + Recid + "&cmprecid=" + Session["CompanyID"];
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
                            var content = JsonConvert.DeserializeObject<SalesheaderObjects>(jsonString);
                            salesheader = content.Data;
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
            ViewBag.LocationType = await LocationTypeEdit(salesheader.SIH_LTRECID);
            ViewBag.LocationRef = await LocationRefEdit(salesheader.SIH_LTRECID, salesheader.SIH_SPRECID);
            return View(salesheader);
        }

        public async Task<ActionResult> SalesheaderAdminView(int Recid)
        {
            Session["invoicerecid"] = Recid;
            string WEBURLGETBYID = ConfigurationManager.AppSettings["INVOICECUSTOMERGETBYID"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            Salesheader salesheader = null;

            string strparams = "recid=" + Recid + "&cmprecid=" + Session["CompanyID"];
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
                            var content = JsonConvert.DeserializeObject<SalesheaderObjects>(jsonString);
                            salesheader = content.Data;
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
            ViewBag.LocationType = await LocationTypeEdit(salesheader.SIH_LTRECID);
            ViewBag.LocationRef = await LocationRefEdit(salesheader.SIH_LTRECID, salesheader.SIH_SPRECID);
            return View(salesheader);
        }

        //[HttpPost]
        //public async Task<ActionResult> SalesheaderEdit(Salesheader salesheader)
        //{
        //    try
        //    {
        //        var ProjectmasterUpdateURL = ConfigurationManager.AppSettings["INVOICEPUT"];
        //        string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
        //        string APIKey = Session["APIKEY"].ToString();

        //        DateTime invoiceDate = Convert.ToDateTime(salesheader.SIH_INVOICEDATE);

        //        var content = $@"{{     
        //    ""siH_RECID"": ""{ Session["invoicerecid"]}"",     
        //    ""siH_SPRECID"": ""{ salesheader.SIH_SPRECID}"",                                   
        //    ""siH_LTRECID"": ""{ salesheader.SIH_LTRECID}"",    
        //    ""siH_INVOICENO"": ""{salesheader.SIH_INVOICENO}"",
        //    ""siH_INVOICEAMOUNT"": ""{ salesheader.SIH_INVOICEAMOUNT}"",                    
        //    ""siH_INVOICEDATE"": ""{ salesheader.SIH_INVOICEDATE}"",          
        //    ""siH_DISABLE"": ""{(salesheader.IsDisabled ? "Y" : "N")}"",  
        //    ""siH_SORTORDER"": ""{ salesheader.SIH_SORTORDER}"",                    
        //    ""siH_CRECID"": ""{ Session["CompanyID"]}"",                              
        //    ""siH_CURECID"": ""{"0"}""                              
        //}}";

        //        // Create the HTTP request
        //        var request = new HttpRequestMessage
        //        {
        //            RequestUri = new Uri(ProjectmasterUpdateURL),
        //            Method = HttpMethod.Put,
        //            Headers =
        //    {
        //        { "X-Version", "1" },
        //        { HttpRequestHeader.Accept.ToString(), "application/json, application/xml" }
        //    },
        //            Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
        //        };

        //        // Set up HTTP client with custom validation (for SSL certificates)
        //        var handler = new HttpClientHandler
        //        {
        //            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        //        };
        //        var client = new HttpClient(handler);
        //        client.DefaultRequestHeaders.Add("ApiKey", APIKey);
        //        client.DefaultRequestHeaders.Add("Authorization", AuthKey);
        //        // Send the request and await the response
        //        var response = await client.SendAsync(request);
        //        // Check if the response is successful
        //        if (response.IsSuccessStatusCode)
        //        {
        //            string responseBody = await response.Content.ReadAsStringAsync();
        //            var apiResponse = JsonConvert.DeserializeObject<ProjectMasterObjects>(responseBody);

        //            if (apiResponse.Status == "Y")
        //            {
        //                return Json(new { success = true, message = apiResponse.Message });
        //            }
        //            else
        //            {
        //                return Json(new { success = false, message = apiResponse.Message });
        //            }
        //        }
        //        else
        //        {
        //            return Json(new
        //            {
        //                success = false,
        //                message = "Code, Name, Invoice Number, Email,Warranty Upto and Mobile Number fields are mandatory."
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = "Exception: " + ex.Message });
        //    }
        //}

        [HttpPost]
        public async Task<ActionResult> SalesheaderEdit(Salesheader salesheader)
        {
            try
            {
                string customerPutUrl = ConfigurationManager.AppSettings["CUSTOMERPRODUCTPUT"]; // 🔥 NEW
                string invoicePutUrl = ConfigurationManager.AppSettings["INVOICEPUT"];
                string authKey = ConfigurationManager.AppSettings["AuthKey"];
                string apiKey = Session["APIKEY"]?.ToString();

                int? customerRecid = salesheader.HiddenCustomerRecid;

                string access1 = salesheader.CU_CONTACTACCESS1_BOOL ? "Y" : "N";
                string access2 = salesheader.CU_CONTACTACCESS2_BOOL ? "Y" : "N";
                string access3 = salesheader.CU_CONTACTACCESS3_BOOL ? "Y" : "N";

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Add("ApiKey", apiKey);
                    client.DefaultRequestHeaders.Add("Authorization", authKey);

                    // ===========================
                    // 1️⃣ CUSTOMER UPDATE API
                    // ===========================
                    var customerContent = $@"{{
                ""cU_RECID"": ""{Session["CU_RECID"]}"",
                ""cU_PRECID"": ""{(string.IsNullOrWhiteSpace(salesheader.SelectedProduct) ? "0" : salesheader.SelectedProduct)}"",
                ""cU_NAME"": ""{salesheader.CU_NAME}"",
                ""cU_EMAIL"": ""{salesheader.CU_EMAIL}"",
                ""cU_MOBILENO"": ""{salesheader.CU_MOBILENO}"",
                ""cU_ADDRESS"": ""{salesheader.CU_ADDRESS}"",
                ""cU_GST"": ""{salesheader.CU_GST}"",
                ""cU_PANNUMBER"": ""{salesheader.CU_PANNUMBER}"",
                ""cU_TANNUMBER"": ""{salesheader.CU_TANNUMBER}"",

                ""cU_CONTACTPERSONNAME1"": ""{salesheader.CU_CONTACTPERSONNAME1}"",
                ""cU_CONTACTPERSONEMAILID1"": ""{salesheader.CU_CONTACTPERSONEMAILID1}"",
                ""cU_CONTACTPERSONMOBILE1"": ""{salesheader.CU_CONTACTPERSONMOBILE1}"",
                ""cU_CONTACTPERSONDESIGINATION1"": ""{salesheader.CU_CONTACTPERSONDESIGINATION1}"",
                ""cU_CONTACTACCESS1"": ""{access1}"",

                ""cU_CONTACTPERSONNAME2"": ""{salesheader.CU_CONTACTPERSONNAME2}"",
                ""cU_CONTACTPERSONEMAILID2"": ""{salesheader.CU_CONTACTPERSONEMAILID2}"",
                ""cU_CONTACTPERSONMOBILE2"": ""{salesheader.CU_CONTACTPERSONMOBILE2}"",
                ""cU_CONTACTPERSONDESIGINATION2"": ""{salesheader.CU_CONTACTPERSONDESIGINATION2}"",
                ""cU_CONTACTACCESS2"": ""{access2}"",

                ""cU_CONTACTPERSONNAME3"": ""{salesheader.CU_CONTACTPERSONNAME3}"",
                ""cU_CONTACTPERSONEMAILID3"": ""{salesheader.CU_CONTACTPERSONEMAILID3}"",
                ""cU_CONTACTPERSONMOBILE3"": ""{salesheader.CU_CONTACTPERSONMOBILE3}"",
                ""cU_CONTACTPERSONDESIGINATION3"": ""{salesheader.CU_CONTACTPERSONDESIGINATION3}"",
                ""cU_CONTACTACCESS3"": ""{access3}"",

                ""cU_CRECID"": ""{Session["CompanyID"]}""
            }}";

                    var customerRequest = new HttpRequestMessage(HttpMethod.Put, customerPutUrl)
                    {
                        Content = new StringContent(customerContent, Encoding.UTF8, "application/json")
                    };

                    customerRequest.Headers.Add("X-Version", "1");

                    var customerResponse = await client.SendAsync(customerRequest);

                    if (!customerResponse.IsSuccessStatusCode)
                        return Json(new { success = false, message = "Customer Update Failed" });

                    var customerResult = await customerResponse.Content.ReadAsStringAsync();
                    var customerObj = JsonConvert.DeserializeObject<CustomerResponse>(customerResult);

                    if (customerObj.Status != "Y")
                        return Json(new { success = false, message = customerObj.Message });

                    // ===========================
                    // 2️⃣ INVOICE UPDATE API
                    // ===========================
                    var invoiceContent = $@"{{     
                ""siH_RECID"": ""{Session["invoicerecid"]}"",
                ""siH_SPRECID"": ""{salesheader.SIH_SPRECID}"",
                ""siH_LTRECID"": ""{salesheader.SIH_LTRECID}"",
                ""siH_INVOICENO"": ""{salesheader.SIH_INVOICENO}"",
                ""siH_INVOICEAMOUNT"": ""{salesheader.SIH_INVOICEAMOUNT}"",
                ""siH_INVOICEDATE"": ""{salesheader.SIH_INVOICEDATE}"",
                ""siH_DISABLE"": ""{(salesheader.IsDisabled ? "Y" : "N")}"",
                ""siH_SORTORDER"": ""{salesheader.SIH_SORTORDER}"",
                ""siH_CRECID"": ""{Session["CompanyID"]}"",
                ""siH_CURECID"": ""{Session["CU_RECID"]}""
            }}";

                    var invoiceRequest = new HttpRequestMessage(HttpMethod.Put, invoicePutUrl)
                    {
                        Content = new StringContent(invoiceContent, Encoding.UTF8, "application/json")
                    };

                    invoiceRequest.Headers.Add("X-Version", "1");

                    var response = await client.SendAsync(invoiceRequest);

                    if (!response.IsSuccessStatusCode)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Invoice Update Failed"
                        });
                    }

                    var responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ProjectMasterObjects>(responseBody);

                    if (apiResponse.Status == "Y")
                        return Json(new { success = true, message = apiResponse.Message });

                    return Json(new { success = false, message = apiResponse.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<ActionResult> SalesheaderDelete(int? Recid)
        {
            string ProjectmasterDeleteUrl = ConfigurationManager.AppSettings["INVOICEDELETE"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            string strparams = "recid=" + Recid + "&cmprecid=" + Session["CompanyID"];
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

                                string redirectUrl = Url.Action("SalesheaderList", "AccountantCustomerInvoice", new { });
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

        [HttpGet]
        public async Task<ActionResult> GetCustomerByEmailFrom(string email, string mobile)
        {
            try
            {
                string apiUrl = ConfigurationManager.AppSettings["GETCUSTOMERBYEMAIL"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                string url = apiUrl + "?companyId=" + Session["CompanyID"] +
             "&email=" + Uri.EscapeDataString(email ?? "") +
             "&mobileno=" + Uri.EscapeDataString(mobile ?? "");

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
                            var json = await response.Content.ReadAsStringAsync();

                            return Content(json, "application/json");
                        }
                        else
                        {
                            return Json(new
                            {
                                Status = "N",
                                Message = "API Error"
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = "N",
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Projectmaster projectmaster)
        {
            try
            {
                var ProjectmasterUpdateURL = ConfigurationManager.AppSettings["CUSTOMERPUT"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                DateTime invoiceDate = Convert.ToDateTime(projectmaster.CU_INVOICEDATE);
                int durationMonths = Convert.ToInt32(Session["P_DURATION"]);
                DateTime warrantyUpto = invoiceDate.AddMonths(durationMonths);
                string warrantyUptoDate = warrantyUpto.ToString("yyyy-MM-dd");

                var content = $@"{{           
            ""cU_RECID"": ""{Session["Productrecid"]}"",    
            ""cU_PRODUCTSERIALNUMBER"": ""{projectmaster.CU_PRODUCTSERIALNUMBER}"", 
            ""cU_ADMINRECID"": ""{projectmaster.CU_ADMINRECID}"",      
            ""cU_CODE"": ""{projectmaster.CU_CODE}"",           
            ""cU_NAME"": ""{projectmaster.CU_NAME}"",
            ""cU_EMAIL"": ""{projectmaster.CU_EMAIL}"",
            ""cU_MOBILENO"": ""{projectmaster.CU_MOBILENO}"",
            ""cU_URECID"": ""{ projectmaster.CU_URECID}"", 
            ""cU_PRECID"": ""{projectmaster.SelectedProduct}"",                
            ""cU_SPRECID"": ""{ projectmaster.CU_SPRECID}"",                    
            ""cU_LTRECID"": ""{ projectmaster.CU_LTRECID}"",    
            ""cU_INVOICENO"": ""{projectmaster.CU_INVOICENO}"",
            ""cU_INVOICEAMOUNT"": ""{ projectmaster.CU_INVOICEAMOUNT}"",                    
            ""cU_INVOICEDATE"": ""{ projectmaster.CU_INVOICEDATE}"",       
            ""cU_WARRANTYUPTO"": ""{warrantyUptoDate}"",
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
                          
            ""cU_CRECID"": ""{ Session["CompanyID"]}"",                              
            ""cU_CTRECID"": ""{"0"}""                              
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

        public async Task<ActionResult> ProductInvoiceList(int productid, string searchPharse)
        {
            ProductInvoice obj = new ProductInvoice();

            int SerialNo = obj.SerialNumber;

            if (SerialNo == 0)
            {
                SerialNo = 1;
            }

            string Weburl = ConfigurationManager.AppSettings["PRODUCTINVOICEGET"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<ProductInvoice> productList = new List<ProductInvoice>();

            string strparams = "productid=" + productid + "&cmprecid=" + Session["CompanyID"];
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
                            var root = JsonConvert.DeserializeObject<ProductInvoiceRoot>(jsonString);

                            productList = root.Data ?? new List<ProductInvoice>();

                            // ✅ Serial Number
                            if (productList.Count > 0)
                            {
                                for (int i = 0; i < productList.Count; i++)
                                {
                                    productList[i].SerialNumber = i + 1;
                                }
                            }

                            // 🔍 Search Filter
                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                productList = productList
                                    .Where(r =>
                                        (r.SIH_INVOICENO != null && r.SIH_INVOICENO.ToLower().Contains(searchPharse.ToLower())) ||
                                        r.SIP_PRODUCTSERIALNUMBER.ToLower().Contains(searchPharse.ToLower()) ||
                                        r.SIH_INVOICEAMOUNT.ToString().Contains(searchPharse) ||
                                        r.SIH_INVOICEDATE.ToString().Contains(searchPharse)
                                    ).ToList();
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

            return View(productList);
        }

        public async Task<ActionResult> ViewInvoicePdf(int invoiceRecid)
        {
            string Weburl = ConfigurationManager.AppSettings["PROCESSINVOICEPDF"];
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

                        // 🔹 Request Body
                        var requestData = new
                        {
                            CompanyRecID = Convert.ToInt32(Session["CompanyID"]),
                            InvoiceRecid = invoiceRecid
                        };

                        var json = JsonConvert.SerializeObject(requestData);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        // 🔹 POST CALL
                        var response = await client.PostAsync(Weburl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            // 🔹 Get PDF bytes
                            var pdfBytes = await response.Content.ReadAsByteArrayAsync();

                            // 🔹 Return PDF
                            return File(pdfBytes, "application/pdf", "Invoice.pdf");
                        }
                        else
                        {
                            return Content("Error: " + response.ReasonPhrase);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("Exception: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ProcessInvoice(int Recid)
        {
            try
            {
                var url = ConfigurationManager.AppSettings["SALESINVOICEHEADERPROCESS"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"]?.ToString();

                var content = $@"{{
            ""SIH_RECID"": ""{Recid}"",
            ""SIH_CRECID"": ""{Session["CompanyID"]}""
        }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Put,
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
                    var result = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<dynamic>(result);

                    TempData["SuccessMessage"] = apiResponse.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = "Process failed";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("SalesheaderList", "AccountantCustomerInvoice");
        }

        [HttpGet]
        public async Task<ActionResult> UnlockInvoice(int Recid, string reason)
        {
            try
            {
                var url = ConfigurationManager.AppSettings["SALESINVOICEHEADERUNLOCK"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"]?.ToString();

                var content = $@"{{
            ""SIH_RECID"": ""{Recid}"",
            ""SIH_CRECID"": ""{Session["CompanyID"]}"",
            ""LL_CRECID"": ""{Session["CompanyID"]}"",
            ""LL_REASON"": ""{reason}"",
            ""LL_UNLOCKBY"": ""{Session["UserRECID"]}""
        }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Put,
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
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
                    TempData["SuccessMessage"] = "Unlocked Successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Unlock failed";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("SalesheaderAdminListView");
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

            string webUrlGet = ConfigurationManager.AppSettings["PRODUCTGETCOMBO"];
            //string webUrlGet = ConfigurationManager.AppSettings["GETCOMBOPRODUCTBASEDPRODUCT"];
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
                                    //Text = t.P_DURATION.ToString()
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

            string webUrlGet = ConfigurationManager.AppSettings["PRODUCTGETCOMBO"];
            //string webUrlGet = ConfigurationManager.AppSettings["GETCOMBOPRODUCTBASEDPRODUCT"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            //string strparams = "cmprecid=" + Session["CompanyID"] ;
            string strparams = "cmprecid=" + Session["CompanyID"] + "&productrecid=" + Session["P_RECID"];
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
                                    Recid = t.U_RECID,
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

        [HttpGet]
        public async Task<ActionResult> GetProductAdmins(int cmprecid, int precid)
        {
            if (cmprecid == 0 || precid == 0)
                return Json(new { Status = "N", Message = "Invalid data." });

            string webUrlGet = ConfigurationManager.AppSettings["GETPRODUCTADMIN"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();


            //string url = $"{webUrlGet}?cmprecid={cmprecid}&precid={precid}";
            string url = $"{webUrlGet}?cmprecid={cmprecid}&precid={precid}";

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                    client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(url);
                    var json = await response.Content.ReadAsStringAsync();

                    return Content(json, "application/json");
                }
            }



        }


        public async Task<ViewlistRoot> GetWarrantyDetails()
        {
            ViewlistRoot model = new ViewlistRoot();

            string Weburl = ConfigurationManager.AppSettings["VIEWLIST"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            string strparams = "cmprecid=" + Session["CompanyID"] + "&Productid=" + Session["P_RECID"];
            string url = Weburl + "?" + strparams;

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

                        var root = JsonConvert.DeserializeObject<ViewlistRoot>(jsonString);

                        if (root != null)
                        {
                            model.Product = root.Product;

                            model.Inclusions = root.Inclusions;
                            model.Exclusions = root.Exclusions;
                        }
                    }
                }
            }

            return model;
        }


        [HttpPost]
        public async Task<ActionResult> WarrantyPDF(Prioritywise prioritywise,int? CU_RECID)
        {
            List<Prioritywise> list = new List<Prioritywise>();

            
            string Weburl = ConfigurationManager.AppSettings["WARRANTYPDF"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string APIKey = Session["APIKEY"]?.ToString();


            string url = $"{Weburl}?cmprecid={Session["CompanyId"]}&cusrecid={CU_RECID}";


            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                using (HttpClient client = new HttpClient(handler))
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                    client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                        return Content("Error fetching data: " + response.ReasonPhrase);

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var rootObjects = JsonConvert.DeserializeObject<Prioritywisepdfobjects>(jsonString);

                    if (rootObjects == null || rootObjects.Status != "Y")
                        return Content(rootObjects?.Message ?? "No data found for the selected criteria.");
                  
                        // The API already returns a PDF URL
                        string pdfUrl = rootObjects.fileUrl;
                        var fileBytes = await client.GetByteArrayAsync(pdfUrl);
                        var fileName = Path.GetFileName(pdfUrl); 

                        // Download
                        return File(fileBytes, "application/pdf", fileName);

                }
            }
            catch (Exception ex)
            {
                return Content("Exception occurred: " + ex.Message);
            }
        }


        [HttpPost]
        public async Task<ActionResult> WarrantyPDFToEmail(int? CU_RECID, string Email)
        {
            try
            {
                string baseUrl = ConfigurationManager.AppSettings["WARRANTYPDFTOEMAIL"];
                string AuthKey = ConfigurationManager.AppSettings["Authkey"];
                string APIKey = Session["APIKEY"]?.ToString();
                string CompanyId = Session["CompanyId"]?.ToString();

                string url = $"{baseUrl}?cmprecid={CompanyId}&cusrecid={CU_RECID}&cusmailid={Email}";

                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback =
                        (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                        client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                        // POST request (no body needed)
                        var response = await client.PostAsync(url, null);

                        var jsonString = await response.Content.ReadAsStringAsync();

                        var result = JsonConvert.DeserializeObject<pdfdetail>(jsonString);

                        return Json(new
                        {
                            status = result.Status,
                            message = result.Message
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = "N",
                    message = ex.Message
                });
            }
        }



        public async Task<ActionResult> ExcelUserDownload()
        {
            string Weburl = ConfigurationManager.AppSettings["ExcelCustomer"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();


            List<Projectmaster> projectmasterlist = new List<Projectmaster>();

            string strparams = "CompanyRecID=" + Session["CompanyID"] + "&ProductRecid=" + Session["P_RECID"];
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



    }

}