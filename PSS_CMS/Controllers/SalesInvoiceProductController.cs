using Newtonsoft.Json;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class SalesInvoiceProductController : Controller
    {
        // GET: SalesInvoiceProduct
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

        //[HttpGet]
        //public async Task<ActionResult> GetCustomerByEmailFrom(string email)
        //{
        //    try
        //    {
        //        string apiUrl = ConfigurationManager.AppSettings["GETCUSTOMERBYEMAIL"];
        //        string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
        //        string APIKey = Session["APIKEY"].ToString();

        //        string url = apiUrl + "?companyId=" + Session["CompanyID"] + "&email=" + email;

        //        using (HttpClientHandler handler = new HttpClientHandler())
        //        {
        //            handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

        //            using (HttpClient client = new HttpClient(handler))
        //            {
        //                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
        //                client.DefaultRequestHeaders.Add("Authorization", AuthKey);

        //                var response = await client.GetAsync(url);

        //                if (response.IsSuccessStatusCode)
        //                {
        //                    var json = await response.Content.ReadAsStringAsync();

        //                    return Content(json, "application/json");
        //                }
        //                else
        //                {
        //                    return Json(new
        //                    {
        //                        Status = "N",
        //                        Message = "API Error"
        //                    }, JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            Status = "N",
        //            Message = ex.Message
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public async Task<ActionResult> ProductAdminView(int? SalesinvoiceRecID, int? customerid, string status)
        {
            SalesInvoiceProducts objexclusion = new SalesInvoiceProducts();

            if (status != null)
            {
                Session["status"] = status;
            }
            ViewBag.InvoiceStatus = Session["status"];

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

        //        [HttpPost]
        //        public async Task<ActionResult> Create(SalesInvoiceProducts model)
        //        {
        //            try
        //            {

        //                string customerPostUrl = ConfigurationManager.AppSettings["CUSTOMERPRODUCTPOST"];
        //                string productPostUrl = ConfigurationManager.AppSettings["SALESPRODUCTPOST"];

        //                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
        //                string APIKey = Session["APIKEY"].ToString();

        //                int? customerRecid = 0;
        //                string access1 = model.CU_CONTACTACCESS1_BOOL ? "Y" : "N";
        //                string access2 = model.CU_CONTACTACCESS2_BOOL ? "Y" : "N";
        //                string access3 = model.CU_CONTACTACCESS3_BOOL ? "Y" : "N";
        //                // =====================================
        //                // ✅ ALWAYS CALL CUSTOMER API
        //                // =====================================

        //                var customerContent = $@"{{           
        //    ""cU_RECID"": ""{model.HiddenCustomerRecid}"",             
        //    ""cU_PRECID"":  ""{(string.IsNullOrWhiteSpace(model.SelectedProduct) ? "0" : model.SelectedProduct)}"",        
        //    ""cU_ADMINRECID"": ""{model.SIP_ADMINRECID}"",            
        //    ""cU_NAME"": ""{model.CU_NAME}"",           
        //    ""cU_EMAIL"": ""{model.CU_EMAIL}"",                    
        //    ""cU_MOBILENO"": ""{model.CU_MOBILENO}"",                    
        //    ""cU_ADDRESS"": ""{model.CU_ADDRESS}"",                    
        //    ""cU_GST"": ""{model.CU_GST}"",
        //    ""cU_PANNUMBER"": ""{model.CU_PANNUMBER}"",
        //    ""cU_TANNUMBER"": ""{model.CU_TANNUMBER}"",

        //    ""cU_CONTACTPERSONNAME1"": ""{model.CU_CONTACTPERSONNAME1}"",
        //    ""cU_CONTACTPERSONEMAILID1"": ""{model.CU_CONTACTPERSONEMAILID1}"",
        //    ""cU_CONTACTPERSONMOBILE1"": ""{model.CU_CONTACTPERSONMOBILE1}"",
        //    ""cU_CONTACTPERSONDESIGINATION1"": ""{model.CU_CONTACTPERSONDESIGINATION1}"",
        //    ""cU_CONTACTACCESS1"": ""{access1}"",

        //    ""cU_CONTACTPERSONNAME2"": ""{model.CU_CONTACTPERSONNAME2}"",
        //    ""cU_CONTACTPERSONEMAILID2"": ""{model.CU_CONTACTPERSONEMAILID2}"",
        //    ""cU_CONTACTPERSONMOBILE2"": ""{model.CU_CONTACTPERSONMOBILE2}"",
        //    ""cU_CONTACTPERSONDESIGINATION2"": ""{model.CU_CONTACTPERSONDESIGINATION2}"",
        //     ""cU_CONTACTACCESS2"": ""{access2}"",

        //    ""cU_CONTACTPERSONNAME3"": ""{model.CU_CONTACTPERSONNAME3}"",
        //    ""cU_CONTACTPERSONEMAILID3"": ""{model.CU_CONTACTPERSONEMAILID3}"",
        //    ""cU_CONTACTPERSONMOBILE3"": ""{model.CU_CONTACTPERSONMOBILE3}"",
        //    ""cU_CONTACTPERSONDESIGINATION3"": ""{model.CU_CONTACTPERSONDESIGINATION3}"",
        //    ""cU_CONTACTACCESS3"": ""{access3}"",               
        //    ""cU_CRECID"": ""{Session["CompanyID"]}""         
        //}}";

        //                var customerRequest = new HttpRequestMessage
        //                {
        //                    RequestUri = new Uri(customerPostUrl),
        //                    Method = HttpMethod.Post,
        //                    Headers =
        //    {
        //        { "X-Version", "1" },
        //        { HttpRequestHeader.Accept.ToString(), "application/json" }
        //    },
        //                    Content = new StringContent(customerContent, System.Text.Encoding.UTF8, "application/json")
        //                };

        //                var handler = new HttpClientHandler
        //                {
        //                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        //                };

        //                var client = new HttpClient(handler);
        //                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
        //                client.DefaultRequestHeaders.Add("Authorization", AuthKey);

        //                var customerResponse = await client.SendAsync(customerRequest);

        //                if (!customerResponse.IsSuccessStatusCode)
        //                {
        //                    return Json(new { success = false, message = "Customer API Failed" });
        //                }

        //                string customerResult = await customerResponse.Content.ReadAsStringAsync();
        //                //dynamic customerObj = JsonConvert.DeserializeObject(customerResult);
        //                var customerObj = JsonConvert.DeserializeObject<CustomerResponse>(customerResult);

        //                if (customerObj.Status != "Y")
        //                {
        //                    return Json(new { success = false, message = customerObj.Message });
        //                }

        //                customerRecid = customerObj.CustomerRecid;

        //                // =====================================
        //                // ✅ STEP 3: INSERT PRODUCT
        //                // =====================================

        //                var productContent = $@"{{           
        //            ""sIP_CURECID"": ""{customerRecid}"",           
        //            ""sIP_PRECID"": ""{(string.IsNullOrWhiteSpace(model.SelectedProduct) ? "0" : model.SelectedProduct)}"",           
        //            ""sIP_PRODUCTSERIALNUMBER"": ""{model.SIP_PRODUCTSERIALNUMBER}"",           
        //            ""sIP_ADMINRECID"": ""{model.SIP_ADMINRECID}"",           
        //            ""sIP_WARRANTYUPTO"": ""{model.SIP_WARRANTYUPTO}"",           
        //            ""sIP_WARRANTYFREECALLS"": ""{model.SIP_WARRANTYFREECALLS}"",           
        //            ""sIP_SORTORDER"": ""{model.SIP_SORTORDER}"",           
        //            ""sIP_SIHRECID"": ""{Session["SalesinvoiceRecID"]}"",           
        //            ""sIP_DISABLE"": ""N"",           
        //            ""sIP_CRECID"": ""{Session["CompanyID"]}""         
        //        }}";

        //                var productRequest = new HttpRequestMessage
        //                {
        //                    RequestUri = new Uri(productPostUrl),
        //                    Method = HttpMethod.Post,
        //                    Headers =
        //            {
        //                { "X-Version", "1" },
        //                { HttpRequestHeader.Accept.ToString(), "application/json" }
        //            },
        //                    Content = new StringContent(productContent, System.Text.Encoding.UTF8, "application/json")
        //                };

        //                var productHandler = new HttpClientHandler
        //                {
        //                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        //                };

        //                var productClient = new HttpClient(productHandler);
        //                productClient.DefaultRequestHeaders.Add("ApiKey", APIKey);
        //                productClient.DefaultRequestHeaders.Add("Authorization", AuthKey);

        //                var productResponse = await productClient.SendAsync(productRequest);

        //                if (productResponse.IsSuccessStatusCode)
        //                {
        //                    string responseBody = await productResponse.Content.ReadAsStringAsync();
        //                    var apiResponse = JsonConvert.DeserializeObject<ProjectMasterObjects>(responseBody);

        //                    if (apiResponse.Status == "Y")
        //                    {
        //                        return Json(new { success = true, message = "Saved Successfully" });
        //                    }
        //                    else
        //                    {
        //                        return Json(new { success = false, message = apiResponse.Message });
        //                    }
        //                }
        //                else
        //                {
        //                    return Json(new
        //                    {
        //                        success = false,
        //                        message = "Product API Failed"
        //                    });
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                return Json(new { success = false, message = ex.Message });
        //            }
        //        }

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
        ""siP_ADMINRECID"": ""{model.SIP_ADMINRECID}""    
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

        [HttpGet]
        public async Task<ActionResult> UpdatePayment(int? TC_Recid, decimal Amount, int? TC_URECID, int? TC_PRECID)
        {
            ViewBag.Amount = Amount;
            Session["tC_RECID"] = TC_Recid;
            Session["TC_URECID"] = TC_URECID;
            Session["TC_PRECID"] = TC_PRECID;

            string WEBURLGETBYID = ConfigurationManager.AppSettings["GETPAYMENT"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            PaymentUpdate paymentupdate = null;

            string strparams = "TC_Recid=" + TC_Recid + "&TC_CRECID=" + Session["CompanyID"];
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
                            var content = JsonConvert.DeserializeObject<PaymentUpdateRootObject>(jsonString);
                            paymentupdate = content.Data;
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

            return View(paymentupdate);
        }

        public async Task<ActionResult> PaymentList(int? SalesinvoiceRecID)
        {
            InvoicePayment objexclusion = new InvoicePayment();

            int SerialNo = objexclusion.SerialNumber;

            if (SerialNo == 0)
            {
                SerialNo = 1; // Initialize to 1 if it's 0
            }
            if (SalesinvoiceRecID != null)
            {
                Session["SalesinvoiceRecID"] = SalesinvoiceRecID;
            } 

            string Weburl = ConfigurationManager.AppSettings["INVOICEPAYMENT"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<InvoicePayment> Customernotificationlist = new List<InvoicePayment>();

            string strparams = "InvoiceRecID=" + Session["SalesinvoiceRecID"] + "&CompanyRecID=" + Session["CompanyID"];
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
                            var rootObjects = JsonConvert.DeserializeObject<InvoicePaymentRootObjects>(jsonString);

                            Customernotificationlist = rootObjects.Data ?? new List<InvoicePayment>();
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

        public async Task<ActionResult> PaymentCreate()
        {
            InvoicePayment model = new InvoicePayment();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> PaymentCreate(InvoicePayment paymentupdate)
        {
            try
            {
                var UpdatePaymentURL = ConfigurationManager.AppSettings["CREATEINVOICEPAYMENT"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{
    ""PP_CRECID"": ""{Session["CompanyID"]}"",
    ""TC_InvoiceNumber"": ""{paymentupdate.PP_INVOICENUMBER}"",
    ""PP_MODEOFPAYMENT"": ""{paymentupdate.PP_MODEOFPAYMENT}"",
    ""PP_DATEOFPAYMENT"": ""{paymentupdate.PP_DATEOFPAYMENT:yyyy-MM-ddTHH:mm:ss}"",
    ""PP_TOTALAMOUNT"": ""{paymentupdate.PP_TOTALAMOUNT}"",
    ""PP_PAIDAMOUNT"": ""{paymentupdate.PP_PAIDAMOUNT}"",
    ""PP_BALANCEAMOUNT"": ""{paymentupdate.PP_BALANCEAMOUNT}"",
    ""PP_PAYMENTSTATUS"": ""{paymentupdate.PP_PAYMENTSTATUS}"",
    ""PP_INVOICEDATE"": ""{paymentupdate.PP_INVOICEDATE}""
}}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(UpdatePaymentURL),
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
                    var apiResponse = JsonConvert.DeserializeObject<InvoicePaymentRootObject>(responseBody);

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
                        message = "Please enter Mode of Payment and Date of Payment"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Exception: " + ex.Message });
            }
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
    }
    public class CustomerResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public int? CustomerRecid { get; set; }
    }
}