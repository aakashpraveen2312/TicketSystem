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
    public class HelpdeskSalesInvoiceController : Controller
    {
        // GET: HelpdeskSalesInvoice
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

            return RedirectToAction("SalesheaderList", "HelpdeskSalesInvoice");
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
                int? UsercustomerRecid = 0;

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
                    UsercustomerRecid = customerObj.CU_URECID;

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
                ""siH_CURECID"": ""{customerRecid}"",
                ""siH_URECID"": ""{UsercustomerRecid}""
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
                            Session["SIH_URECID"] = salesheader.SIH_URECID;
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
                ""siH_CURECID"": ""{Session["CU_RECID"]}"",
                ""siH_URECID"": ""{Session["SIH_URECID"]}""
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
                                }).ToList();
                                ViewBag.ProductDurationList = rootObjects.Data.ToDictionary(
    x => x.P_RECID,
    x => x.P_DURATION
);
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
                                ViewBag.ProductDurationList = rootObjects.Data.ToDictionary(
    x => x.P_RECID,
    x => x.P_DURATION
);
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



        public async Task<ActionResult> List(int? SalesinvoiceRecID, int? customerid, string status, string invoicedate)
        {
            SalesInvoiceProducts objexclusion = new SalesInvoiceProducts();

            if (status != null)
            {
                Session["status"] = status;
            }
            if (invoicedate != null)
            {
                Session["invoicedate"] = invoicedate;
            }
            ViewBag.InvoiceStatus = status ?? "";
            ViewBag.invoicedate = invoicedate ?? "";

            int SerialNo = objexclusion.SerialNumber;

            if (SerialNo == 0)
            {
                SerialNo = 1; // Initialize to 1 if it's 0
            }
            if (SalesinvoiceRecID != null)
            {
                Session["SalesinvoiceRecID"] = SalesinvoiceRecID;
            }

            if (customerid != null)
            {
                Session["customerid"] = customerid;
            }

            string Weburl = ConfigurationManager.AppSettings["INVOICEPRODUCTGET"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<SalesInvoiceProducts> Customernotificationlist = new List<SalesInvoiceProducts>();

            string strparams = "SalesinvoiceID=" + Session["SalesinvoiceRecID"];
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
                            var rootObjects = JsonConvert.DeserializeObject<SalesInvoiceRootObjects>(jsonString);

                            Customernotificationlist = rootObjects.Data ?? new List<SalesInvoiceProducts>();
                            if (Customernotificationlist.Count > 0)
                            {
                                // Assign serial numbers
                                for (int i = 0; i < Customernotificationlist.Count; i++)
                                {
                                    Customernotificationlist[i].SerialNumber = i + 1;
                                }
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
            return View(Customernotificationlist);
        }

 
        public async Task<ActionResult> Create()
        {
            ViewBag.invoicedate = Session["invoicedate"] ?? "";
            SalesInvoiceProducts model = new SalesInvoiceProducts();
            await ComboProductSelection();
            return View(model);
        }


        [HttpPost]
        public async Task<ActionResult> Create(SalesInvoiceProducts model)
        {
            try
            {
                string productPostUrl = ConfigurationManager.AppSettings["SALESPRODUCTPOST"];
                string authKey = ConfigurationManager.AppSettings["AuthKey"];
                string apiKey = Session["APIKEY"]?.ToString();

                // ✅ Use existing customer recid
                int customerRecid = model.HiddenCustomerRecid;

                var productContent = $@"{{           
            ""sIP_CURECID"": ""{Session["customerid"]}"",           
            ""sIP_PRECID"": ""{(string.IsNullOrWhiteSpace(model.SelectedProduct) ? "0" : model.SelectedProduct)}"",           
            ""sIP_PRODUCTSERIALNUMBER"": ""{model.SIP_PRODUCTSERIALNUMBER}"",           
            ""sIP_ADMINRECID"": ""{model.SIP_ADMINRECID}"",           
            ""sIP_WARRANTYUPTO"": ""{model.SIP_WARRANTYUPTO}"",           
            ""sIP_WARRANTYFREECALLS"": ""{model.SIP_WARRANTYFREECALLS}"",           
            ""sIP_SORTORDER"": ""{model.SIP_SORTORDER}"",           
            ""sIP_PRODUCTAMOUNT"": ""{model.SIP_PRODUCTAMOUNT}"",           
            ""sIP_SIHRECID"": ""{Session["SalesinvoiceRecID"]}"",           
            ""sIP_DISABLE"": ""N"",           
            ""sIP_CRECID"": ""{Session["CompanyID"]}""         
        }}";

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Add("ApiKey", apiKey);
                    client.DefaultRequestHeaders.Add("Authorization", authKey);

                    var request = new HttpRequestMessage(HttpMethod.Post, productPostUrl)
                    {
                        Content = new StringContent(productContent, Encoding.UTF8, "application/json")
                    };

                    request.Headers.Add("X-Version", "1");

                    var response = await client.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {
                        return Json(new { success = false, message = "Product API Failed" });
                    }

                    var responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ProjectMasterObjects>(responseBody);

                    if (apiResponse.Status == "Y")
                    {
                        return Json(new { success = true, message = "Saved Successfully" });
                    }

                    return Json(new { success = false, message = apiResponse.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<ActionResult> Edit(int? Recid)
        {
            ViewBag.invoicedate = Session["invoicedate"] ?? "";
            Session["Productrecid"] = Recid;
            string apiUrl = ConfigurationManager.AppSettings["GETSALESPRODUCTWITHCUSTOMER"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            SalesInvoiceProducts model = null;

            string finalurl = apiUrl + "?recid=" + Recid;

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

                            var content = JsonConvert.DeserializeObject<SalesInvoiceRootObject>(jsonString);

                            if (content.Status == "Y")
                            {
                                model = content.Data;
                                Session["SalesProductRecid"] = model.SIP_RECID;
                            }
                            else
                            {
                                ModelState.AddModelError("", content.Message);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "API Error: " + response.ReasonPhrase);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Exception: " + ex.Message);
            }
            //await ComboUserEdit(model.CU_NAME);
            await ComboProductSelectionEdit(model.SIP_PRECID);
            return View(model);
        }

        public async Task<ActionResult> View(int? Recid)
        {
            Session["Productrecid"] = Recid;
            string apiUrl = ConfigurationManager.AppSettings["GETSALESPRODUCTWITHCUSTOMER"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            SalesInvoiceProducts model = null;

            string finalurl = apiUrl + "?recid=" + Recid;

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

                            var content = JsonConvert.DeserializeObject<SalesInvoiceRootObject>(jsonString);

                            if (content.Status == "Y")
                            {
                                model = content.Data;
                                Session["SalesProductRecid"] = model.SIP_RECID;
                            }
                            else
                            {
                                ModelState.AddModelError("", content.Message);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "API Error: " + response.ReasonPhrase);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Exception: " + ex.Message);
            }
            //await ComboUserEdit(model.CU_NAME);
            await ComboProductSelectionEdit(model.SIP_PRECID);
            return View(model);
        }

        public async Task<ActionResult> ProductDetailView(int? Recid)
        {
            Session["Productrecid"] = Recid;
            string apiUrl = ConfigurationManager.AppSettings["GETSALESPRODUCTWITHCUSTOMER"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            SalesInvoiceProducts model = null;

            string finalurl = apiUrl + "?recid=" + Recid;

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

                            var content = JsonConvert.DeserializeObject<SalesInvoiceRootObject>(jsonString);

                            if (content.Status == "Y")
                            {
                                model = content.Data;
                                Session["SalesProductRecid"] = model.SIP_RECID;
                            }
                            else
                            {
                                ModelState.AddModelError("", content.Message);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "API Error: " + response.ReasonPhrase);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Exception: " + ex.Message);
            }
            //await ComboUserEdit(model.CU_NAME);
            await ComboProductSelectionEdit(model.SIP_PRECID);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(SalesInvoiceProducts model)
        {
            try
            {
                string apiUrl = ConfigurationManager.AppSettings["UPDATESALES"]; // 🔥 new key
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                // =========================
                // JSON CONTENT
                // =========================
                var content = $@"{{    
       ""siP_SIHRECID"": ""{Session["SalesinvoiceRecID"]}"",
        ""siP_CURECID"": ""{Session["customerid"]}"",
        ""siP_RECID"": ""{Session["SalesProductRecid"]}"",
        ""siP_PRECID"": ""{(string.IsNullOrWhiteSpace(model.SelectedProduct) ? "0" : model.SelectedProduct)}"",
        ""siP_PRODUCTSERIALNUMBER"": ""{model.SIP_PRODUCTSERIALNUMBER}"",
        ""siP_WARRANTYUPTO"": ""{model.SIP_WARRANTYUPTO}"",
        ""siP_WARRANTYFREECALLS"": ""{model.SIP_WARRANTYFREECALLS}"",
        ""siP_SORTORDER"": ""{model.SIP_SORTORDER}"",
        ""siP_PRODUCTAMOUNT"": ""{model.SIP_PRODUCTAMOUNT}"",
        ""siP_ADMINRECID"": ""{model.SIP_ADMINRECID}"",
        ""siP_CRECID"": ""{Session["CompanyID"]}""    
    }}";

                // =========================
                // REQUEST
                // =========================
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(apiUrl),
                    Method = HttpMethod.Put,
                    Headers =
            {
                { "X-Version", "1" },
                { HttpRequestHeader.Accept.ToString(), "application/json" }
            },
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
                        message = "Update API Failed"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<ActionResult> Delete(int? Recid)
        {
            string ProjectmasterDeleteUrl = ConfigurationManager.AppSettings["INVOICEPRODUCTDELETE"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            string strparams = "recid=" + Recid;
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

                                string redirectUrl = Url.Action("List", "SalesInvoiceProduct", new { });
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


        public class CustomerResponse
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public int? CustomerRecid { get; set; }
            public int? CU_URECID { get; set; }
        }
    }
}