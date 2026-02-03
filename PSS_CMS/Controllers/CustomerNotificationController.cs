using Newtonsoft.Json;
using PSS_CMS.Models;
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

namespace PSS_CMS.Controllers
{
    public class CustomerNotificationController : Controller
    {
        // GET: CustomerNotification
        public async Task<ActionResult> List(int? CustomerRecID , string Name,string InvoiceNo)
        {
            if (CustomerRecID != null && Name != null)
            {
                Session["CustomerRecID"] = CustomerRecID;
                Session["Customername"] = Name;
                Session["InvoiceNo"] = InvoiceNo;
            }

            string Weburl = ConfigurationManager.AppSettings["GetCustomerNotification"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<Customernotification> Customernotificationlist = new List<Customernotification>();

            string strparams = "cmprecid=" + Session["CompanyID"] + "&CustomerRecID=" + Session["CustomerRecID"];
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
                            var rootObjects = JsonConvert.DeserializeObject<CustomernotificationRootObjects>(jsonString);

                            Customernotificationlist = rootObjects.Data;

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

        public ActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Customernotification Customernotification, string CI_INVOICEAMOUNT)
        {
            try
            {
                var ContractinvoicePostURL = ConfigurationManager.AppSettings["CustomerNotification"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{           
            ""cN_INVOICENO"": ""{Customernotification.CN_INVOICENO}"",           
            ""cN_FOLLOWUPDATE"": ""{Customernotification.CN_FOLLOWUPDATE}"",    
            ""cN_STATUS"": ""{ Customernotification.CN_STATUS}"",                    
            ""cN_COMMENTS"": ""{ Customernotification.CN_COMMENTS}"",          
            ""cN_CURECID"": ""{Session["CustomerRecID"]}"",       
            ""cN_CRECID"": ""{Session["CompanyID"]}"",   
            ""cN_CTRECID"": ""{Session["CU_CTRECID"]}""
             
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


        public async Task<ActionResult> Edit(int? id)
        {
            Session["CN_RECID"] = id;

            string WEBURLGETBYID = ConfigurationManager.AppSettings["GetCustomerNotificationbyId"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            Customernotification contractInvoice = null;

            string strparams = "recID=" + id + "&cmprecid=" + Session["CompanyID"];
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
                            var content = JsonConvert.DeserializeObject<CustomernotificationRootObject>(jsonString);
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
        public async Task<ActionResult> Edit(Customernotification Customernotification)
        {
            try
            {
                var UpdateURL = ConfigurationManager.AppSettings["PutCustomerNotification"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{           
            ""cN_RECID"": ""{Session["CN_RECID"]}"",           
            ""cN_INVOICENO"": ""{Customernotification.CN_INVOICENO}"",           
            ""cN_FOLLOWUPDATE"": ""{Customernotification.CN_FOLLOWUPDATE}"",
            ""cN_STATUS"": ""{Customernotification.CN_STATUS}"",
            ""cN_COMMENTS"": ""{Customernotification.CN_COMMENTS}"",
            ""cN_CURECID"": ""{Session["CustomerRecID"]}"",
            ""cN_CTRECID"": ""{Session["CU_CTRECID"]}"",                     
            ""cN_CRECID"": ""{Session["CompanyID"]}""                     
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
            string WEBURLDELETE = ConfigurationManager.AppSettings["DeleteCustomerNotification"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string strparams = "cmprecid=" + Session["CompanyID"] + "&RECID=" + id ;
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

                                string redirectUrl = Url.Action("List", "CustomerNotification", new { CustomerRecID = Session["CustomerRecID"], Name= Session["Customername"], InvoiceNo = Session["InvoiceNo"] });
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