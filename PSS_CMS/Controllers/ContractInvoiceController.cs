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
    public class ContractInvoiceController : Controller
    {
        // GET: ContractInvoice 
        public async Task<ActionResult> List(int? id, string Name, string ContractAmount)
        {
            if (id != null && Name != null && ContractAmount != null)
            {
                Session["CT_RECID"] = id;
                Session["CT_REFNO"] = Name;
                Session["ContractAmount"] = ContractAmount;
            }

            string Weburl = ConfigurationManager.AppSettings["CONTRACTINVOICEGET"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<ContractInvoice> contractinvoicelist = new List<ContractInvoice>();

            string strparams = "CompanyRecID=" + Session["CompanyID"] + "&Recid=" + Session["CT_RECID"];
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
                            var rootObjects = JsonConvert.DeserializeObject<ContractInvoiceRootObjects>(jsonString);
                            contractinvoicelist = rootObjects.Data;

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
            return View(contractinvoicelist);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(ContractInvoice contractInvoice, string CI_INVOICEAMOUNT)
        {
            try
            {
                var ContractinvoicePostURL = ConfigurationManager.AppSettings["CONTRACTINVOICEPOST"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{           
            ""ci_INVOICEDATE"": ""{contractInvoice.CI_INVOICEDATE}"",           
            ""ci_INVOICENUMBER"": ""{contractInvoice.CI_INVOICENUMBER}"",           
            ""ci_INVOICEAMOUNT"": ""{CI_INVOICEAMOUNT}"",                    
            ""ci_PAYMENTRECEIVEDDATE"": ""{ contractInvoice.CI_PAYMENTRECEIVEDDATE}"",                    
            ""ci_PAYMENTRECEIVEDAMOUNT"": ""{ contractInvoice.CI_PAYMENTRECEIVEDAMOUNT}"",                   
            ""ci_PAYMENTDUEDATE"": ""{ contractInvoice.CI_PAYMENTDUEDATE}"",                    
            ""ci_CTRECID"": ""{Session["CT_RECID"]}"",                    
            ""ci_SORTORDER"": ""{(contractInvoice.CI_SORTORDER)}"",        
            ""ci_CRECID"": ""{Session["CompanyID"]}"",   
            ""ci_USERID"": ""{Session["UserRECID"]}""
             
        }}";


                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(ContractinvoicePostURL),
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
                    var apiResponse = JsonConvert.DeserializeObject<ContractInvoiceObject>(responseBody);

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
                    return Json(new { success = false, message = "Error: " + response.ReasonPhrase });
                }


            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            return View();

        }


        public async Task<ActionResult> Edit(int? id, int? CI_CRECID, string Name, int? CI_CTRECID)
        {
            Session["CI_RECID"] = id;
            Session["CI_CTRECID"] = CI_CTRECID;
            Session["CI_CRECID"] = CI_CRECID;
            Session["CI_INVOICENUMBER"] = Name;

            string WEBURLGETBYID = ConfigurationManager.AppSettings["CONTRACTINVOICEGETBYID"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            ContractInvoice contractInvoice = null;

            string strparams = "Recid=" + id + "&companyId=" + Session["CompanyID"];
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
                            var content = JsonConvert.DeserializeObject<ContractInvoiceObject>(jsonString);
                            contractInvoice = content.Data;

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
            return View(contractInvoice);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(ContractInvoice contractInvoice)
        {
            try
            {
                var UpdateURL = ConfigurationManager.AppSettings["CONTRACTINVOICEPUT"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{           
            ""cI_RECID"": ""{Session["CI_RECID"]}"",           
            ""cI_INVOICEDATE"": ""{contractInvoice.CI_INVOICEDATE}"",           
            ""cI_INVOICENUMBER"": ""{contractInvoice.CI_INVOICENUMBER}"",
            ""cI_INVOICEAMOUNT"": ""{contractInvoice.CI_INVOICEAMOUNT}"",
            ""cI_PAYMENTRECEIVEDDATE"": ""{contractInvoice.CI_PAYMENTRECEIVEDDATE}"",
            ""cI_PAYMENTRECEIVEDAMOUNT"": ""{contractInvoice.CI_PAYMENTRECEIVEDAMOUNT}"",
            ""cI_CRECID"": ""{Session["CompanyID"]}"",
            ""cI_CTRECID"": ""{Session["CI_CTRECID"]}"",
            ""cI_USERID"": ""{Session["UserRECID"]}"",
            ""cI_SORTORDER"": ""{contractInvoice.CI_SORTORDER}"",
            ""cI_PAYMENTDUEDATE"": ""{contractInvoice.CI_PAYMENTDUEDATE}""                          
        }}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(UpdateURL),
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
                    var apiResponse = JsonConvert.DeserializeObject<ContractInvoiceObject>(responseBody);

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
                    return Json(new { success = false, message = "Error: Something went wrong." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Exception: " + ex.Message });
            }
        }

        public async Task<ActionResult> Delete(int id)

        {
            string WEBURLDELETE = ConfigurationManager.AppSettings["CONTRACTINVOICEDELETE"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string strparams = "companyId=" + Session["CompanyID"] + "&RecordId=" + id + "&userrecid=" + Session["UserRECID"];
            string finalurl = WEBURLDELETE + "?" + strparams;
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


                        var request = new HttpRequestMessage
                        {
                            Method = HttpMethod.Delete,
                            RequestUri = new Uri(finalurl)
                        };

                        var response = await client.SendAsync(request);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseBody = await response.Content.ReadAsStringAsync();
                            var apiResponse = JsonConvert.DeserializeObject<ContractInvoiceObject>(responseBody);

                            if (apiResponse.Status == "Y")
                            {

                                string redirectUrl = Url.Action("List", "ContractInvoice", new { });
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