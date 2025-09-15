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
    public class ContractController : Controller
    {
        public async Task<ActionResult> ContractDashboard()
        {

            string WEBURLGET = ConfigurationManager.AppSettings["CONTRACTDASHBOARD"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "CompanyRecID=" + Session["CompanyID"];
            string finalurl = WEBURLGET + "?" + strparams;
            Contract contract  = null;

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
                            contract = JsonConvert.DeserializeObject<Contract>(jsonString);

                            ViewBag.TotalContractAmount = contract.TotalContractAmount;
                            ViewBag.TotalPaidAmount = contract.TotalPaidAmount;
                            ViewBag.PendingAmount = contract.PendingAmount;
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
            await StackedBarChartCustomer();           
            return View(contract);
        }

        public async Task<ActionResult> StackedBarChartCustomer()
        {
            string WEBURLGET = ConfigurationManager.AppSettings["CONTRACTSTACKEDBAR"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "cmprecid=" + Session["CompanyID"];
            string finalurl = WEBURLGET + "?" + strparams;
            List<Contract> dashboardData = new List<Contract>();

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
                            var rootObjects = JsonConvert.DeserializeObject<RootObjectsContract>(jsonString);
                            dashboardData = rootObjects.Data;

                            ViewBag.CustomerNames = JsonConvert.SerializeObject(dashboardData.Select(d => d.CustomerName));
                            ViewBag.PaidAmount = JsonConvert.SerializeObject(dashboardData.Select(d => d.PaidAmount));
                            ViewBag.UnpaidAmount = JsonConvert.SerializeObject(dashboardData.Select(d => d.UnpaidAmount));
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
            await YettoInvoiceDashboard();
            return View(dashboardData);
        }

        public async Task<ActionResult> CustomerList(string searchPharse)
        {
            //ProductMaster objproductmaster = new ProductMaster();

            string Weburl = ConfigurationManager.AppSettings["PRODUCTGET"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<ProductMaster> productmasterlist = new List<ProductMaster>();

            string strparams = "cmprecid=" + Session["CompanyID"];
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
                            var rootObjects = JsonConvert.DeserializeObject<ProductMasterRootObject>(jsonString);
                            productmasterlist = rootObjects.Data;

                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                productmasterlist = productmasterlist
                                    .Where(r => r.P_NAME.ToLower().Contains(searchPharse.ToLower()) ||
                                                r.P_SORTORDER.ToString().Contains(searchPharse.ToLower()))
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
            return View(productmasterlist);
        }

        public async Task<ActionResult> ContractList(int? Recid,string name,string searchPharse)
        {
            if (Recid!=null && name!=null)
            {
                Session["Recid"] = Recid;
                Session["name"] = name;
            }
            string WEBURLGET = ConfigurationManager.AppSettings["CONTRACTGET"];
            string Authkey = ConfigurationManager.AppSettings["Authkey"];

            List<Contract> contractsList = new List<Contract>();


            string APIKey = Session["APIKEY"].ToString();


            string strparams = "CompanyRecID=" + Session["CompanyID"] + "&Recid=" + Session["Recid"];
            string finalurl = WEBURLGET + "?" + strparams;
            try
            {


                // Prepare header parameters as per RSGT inputs
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                        client.DefaultRequestHeaders.Add("Authorization", Authkey);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                        var response = await client.GetAsync(finalurl);


                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            //GlobalVariables.ResponseStructure = jsonString;
                            var content = JsonConvert.DeserializeObject<RootObjectsContract>(jsonString);
                            contractsList = content.Data;


                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                contractsList = contractsList
                                    .Where(r => r.CT_CONTRACTREFERENCENUMBER.ToLower().Contains(searchPharse.ToLower()) ||
                                   r.CT_FROMDATE.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                                   r.CT_TODATE.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                                   r.CT_CONTRACTAMOUNT.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                                   r.CT_TOTALPAIDAMOUNT.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                                   r.CT_CONTRACTCREATEDBY.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                                   r.CT_CONTRACTAPPROVEDBY.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                                   r.CT_ANYREFERENCE.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                                   r.CT_TOTALPAIDAMOUNT.ToString().ToLower().Contains(searchPharse.ToLower()))
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
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }
            return View(contractsList);

        }

        public ActionResult CreateContract()
        {
            return View();

        }

        [HttpPost]
        public async Task<ActionResult> CreateContract(Contract contract)
        {
            try
            {
                var ContractPostURL = ConfigurationManager.AppSettings["CONTRACTPOST"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{           
            ""cT_PRECID"": ""{Session["Recid"]}"",           
            ""cT_CONTRACTREFERENCENUMBER"": ""{contract.CT_CONTRACTREFERENCENUMBER}"",           
            ""cT_FROMDATE"": ""{contract.CT_FROMDATE}"",           
            ""cT_TODATE"": ""{ contract.CT_TODATE}"",                    
            ""cT_CONTRACTAMOUNT"": ""{ contract.CT_CONTRACTAMOUNT}"",                                       
            ""cT_CONTRACTCREATEDBY"": ""{ contract.CT_CONTRACTCREATEDBY}"",                    
            ""cT_CONTRACTAPPROVEDBY"": ""{ contract.CT_CONTRACTAPPROVEDBY}"",                    
            ""cT_CONTRACTAPPROVEDDATE"": ""{ contract.CT_CONTRACTAPPROVEDDATE}"",                    
            ""cT_ANYREFERENCE"": ""{ contract.CT_ANYREFERENCE}"",                    
            ""cT_SORTORDER"": ""{(contract.CT_SORTORDER)}"",        
            ""cT_CRECID"": ""{Session["CompanyID"]}""           
        }}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(ContractPostURL),
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
                    var apiResponse = JsonConvert.DeserializeObject<ContractMasterObject>(responseBody);

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

        public async Task<ActionResult> EditContract(int? id, string Name,int? Precid)
        {
            Session["Precid"] = Precid;
            Session["Contractrecid"] = id;
            Session["Name"] = Name;
            string WEBURLGETBYID = ConfigurationManager.AppSettings["CONTRACTGETBYID"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            Contract contract  = null;

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
                            var content = JsonConvert.DeserializeObject<ContractMasterObject>(jsonString);
                            contract = content.Data;
                           
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
            return View(contract);
        }
        [HttpPost]
        public async Task<ActionResult> EditContract(Contract contract)
        {
            try
            {
                var UpdateURL = ConfigurationManager.AppSettings["CONTRACTPUT"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{           
            ""cT_RECID"": ""{Session["Contractrecid"]}"",           
            ""cT_PRECID"": ""{Session["Precid"]}"",           
            ""cT_CONTRACTREFERENCENUMBER"": ""{contract.CT_CONTRACTREFERENCENUMBER}"",
            ""cT_FROMDATE"": ""{contract.CT_FROMDATE}"",
            ""cT_TODATE"": ""{contract.CT_TODATE}"",
            ""cT_CONTRACTAMOUNT"": ""{contract.CT_CONTRACTAMOUNT}"",
            ""cT_TOTALPAIDAMOUNT"": ""{contract.CT_TOTALPAIDAMOUNT}"",
            ""cT_CONTRACTCREATEDBY"": ""{contract.CT_CONTRACTCREATEDBY}"",
            ""cT_CONTRACTAPPROVEDBY"": ""{contract.CT_CONTRACTAPPROVEDBY}"",
            ""cT_CONTRACTAPPROVEDDATE"": ""{contract.CT_CONTRACTAPPROVEDDATE}"",
            ""cT_ANYREFERENCE"": ""{(contract.CT_ANYREFERENCE)}"",                              
            ""cT_SORTORDER"": ""{(contract.CT_SORTORDER)}"",                              
            ""cT_CRECID"": ""{ Session["CompanyID"]}""                              
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
                    var apiResponse = JsonConvert.DeserializeObject<ContractMasterObject>(responseBody);

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

        public async Task<ActionResult> DeleteContract(int id)

        {

            string WEBURLDELETE = ConfigurationManager.AppSettings["CONTRACTDELETE"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string strparams = "companyId=" + Session["CompanyID"] + "&RecordId=" + id;
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
                            var apiResponse = JsonConvert.DeserializeObject<ContractMasterObject>(responseBody);

                            if (apiResponse.Status == "Y")
                            {

                                string redirectUrl = Url.Action("ContractList", "Contract", new { });
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

        public async Task<ActionResult> YettoInvoiceDashboard()
        {

            string WEBURLGET = ConfigurationManager.AppSettings["YETTOINVOICE"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "CompanyRecID=" + Session["CompanyID"];
            string finalurl = WEBURLGET + "?" + strparams;
            Contract contract = null;

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
                            contract = JsonConvert.DeserializeObject<Contract>(jsonString);

                            ViewBag.ExpiredCustomerWarrantyCount = contract.ExpiredCustomerWarrantyCount;
                            ViewBag.ExpiredProductWarrantyCount = contract.ExpiredProductWarrantyCount;
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
            return View(contract);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ChangePassword(Changepassword changepassword)
        {
            try
            {
                var ChnagepasswordURL = ConfigurationManager.AppSettings["CHANGEPASSWORD"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{           
            ""userId"": ""{Session["UserRECID"]}"",           
            ""newpassword"": ""{changepassword.U_NewPassword}"",  
 ""confirmpassword"": ""{changepassword.U_Changepassword}"",     
            ""compRecordID"": ""{ Session["CompanyID"]}""                              
        }}";

                // Create the HTTP request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(ChnagepasswordURL),
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
                    var apiResponse = JsonConvert.DeserializeObject<UserGroupObjects>(responseBody);

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
    }
}