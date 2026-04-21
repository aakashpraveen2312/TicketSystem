using Newtonsoft.Json;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    public class ContractProductController : Controller
    {
        // GET: ContractProduct
        public async Task<ActionResult> Create(ContractServiceProduct contractserviceproduct)
        {
            await ComboProductSelection();
            int selectedProductRecId = contractserviceproduct.SelectedProduct;

            var productList = ViewBag.Product as List<SelectListItem>;

            string productName = productList
                .FirstOrDefault(x => x.Value == contractserviceproduct.SelectedProduct.ToString())?.Text;
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Create(ContractServiceProduct contractserviceproduct, string A)
        {

            try
            {
                await ComboProductSelection();

                int selectedProductRecId = contractserviceproduct.SelectedProduct;

                var productList = ViewBag.Product as List<SelectListItem>;

                string productName = productList
                    .FirstOrDefault(x => x.Value == contractserviceproduct.SelectedProduct.ToString())?.Text;



                var ContractPostURL = ConfigurationManager.AppSettings["PostContractProduct"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var data = new
                {

                    CSP_PRECID = selectedProductRecId,
                    CSP_PRODUCTNAME = productName,
                    CSP_ARECID = contractserviceproduct.CSP_ARECID ?? 0,
                    CSP_ADMINNAME = "admin",
                    CSP_FREESERVICE = contractserviceproduct.CSP_FREESERVICE,
                    CSP_PRODUCTAMOUNT = contractserviceproduct.CSP_PRODUCTAMOUNT,
                    CSP_PAIDAMOUNT = 0.00,
                    CSP_INVOICEAMOUNT = 0.00,
                    CSP_CRECID = Session["CompanyID"],
                    CSP_PRODUCTUNIQUENUMBER = contractserviceproduct.CSP_PRODUCTUNIQUENUMBER,
                    CSP_CTRECID = Session["CTS_RECID"],
                    CSP_USERTYPE = Session["CT_EXISTINGUSER"],
                    CSP_CURECID = Session["CT_URECID"]
                };

                var content = JsonConvert.SerializeObject(data);



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

        public async Task<ActionResult> List(int? ContractRecid, int? CustomerRecid,string CP_CODE,string CP_STATUS)
        {
            if (CP_CODE != null)
            {

                Session["CP_CODE"] = CP_CODE;
            }
            if (CP_STATUS != null)
            {

                Session["CP_STATUS"] = CP_STATUS;
            }

            ContractServiceProduct objproduct = new ContractServiceProduct();

            //int SerialNo = objproduct.SerialNumber;

            //if (SerialNo == 0)
            //{
            //    SerialNo = 1; // Initialize to 1 if it's 0
            //}

            string WEBURLGET = ConfigurationManager.AppSettings["GETContractProduct"];
            string Authkey = ConfigurationManager.AppSettings["Authkey"];

            List<ContractServiceProduct> ContractServiceProductList = new List<ContractServiceProduct>();


            string APIKey = Session["APIKEY"].ToString();

            if (ContractRecid != 0 && ContractRecid!=null)
            {
                Session["CTS_RECID"] = ContractRecid.ToString();
                
            }
           

            string strparams = "CompanyRecID=" + Session["CompanyID"] + "&ContractRecid=" + Session["CTS_RECID"] + "&CustomerRecid=" + Session["CT_URECID"];
            //CompanyRecID=1&ContractRecid=1&CustomerRecid=1
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
                            var content = JsonConvert.DeserializeObject<RootObjectsContractServiceProduct>(jsonString);
                            ContractServiceProductList = content.Data;

                            //if (ContractServiceProductList.Count > 0)
                            //{
                            //    // Assign serial numbers
                            //    for (int i = 0; i < ContractServiceProductList.Count; i++)
                            //    {
                            //        ContractServiceProductList[i].SerialNumber = i + 1;
                            //    }
                            //}
                            //if (!string.IsNullOrEmpty(searchPharse))
                            //{
                            //    contractsList = contractsList
                            //        .Where(r => r.CT_CONTRACTREFERENCENUMBER.ToLower().Contains(searchPharse.ToLower()) ||
                            //       r.CT_FROMDATE.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                            //       r.CT_TODATE.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                            //       r.CT_CONTRACTAMOUNT.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                            //       r.CT_TOTALPAIDAMOUNT.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                            //       r.CT_CONTRACTCREATEDBY.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                            //       r.CT_CONTRACTAPPROVEDBY.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                            //       r.CT_ANYREFERENCE.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                            //       r.CT_TOTALPAIDAMOUNT.ToString().ToLower().Contains(searchPharse.ToLower()))
                            //        .ToList();

                            //}

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
            return View(ContractServiceProductList);
        }

        public async Task<ActionResult> Edit(int? CSP_RECID)
        {
           
            string WEBURLGETBYID = ConfigurationManager.AppSettings["GETContractProductById"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            ContractServiceProduct contractServiceProduct = null;

            string strparams = "RecID=" + CSP_RECID + "&CompanyRecID=" + Session["CompanyID"];
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
                            var content = JsonConvert.DeserializeObject<RootObjectContractServiceProduct>(jsonString);
                            contractServiceProduct = content.Data;

                            Session["CSP_RECID"] = content.Data.CSP_RECID;
                            Session["CSP_PRECID"] = content.Data.CSP_PRECID;
                            Session["CSP_ARECID"] = content.Data.CSP_ARECID;
                            Session["CSP_INVOICEAMOUNT"] = content.Data.CSP_INVOICEAMOUNT;
                            Session["CSP_PAIDAMOUNT"] = content.Data.CSP_PAIDAMOUNT;
                            Session["CSP_USERTYPE"] = content.Data.CSP_USERTYPE;
                            Session["csP_CTRECID"] = content.Data.CSP_CTRECID;
                            Session["csP_CURECID"] = content.Data.CSP_CURECID;

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
            //await LoadProductCombo(contractServiceProduct?.CSP_PRECID ?? 0);

            return View(contractServiceProduct);
        }
        public async Task LoadProductCombo(int ProductId)
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
                                    Value = t.P_RECID.ToString(),
                                    Text = t.P_NAME,
                                    Selected = (t.P_RECID == ProductId)
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

            ViewBag.Product = Product;
        }

        public async Task<ActionResult> Delete(int? CSP_RECID,int? CSP_CRECID,int? CSP_CURECID,int? CSP_PRECID,int? CSP_ARECID)
        {

            string WEBURLDELETE = ConfigurationManager.AppSettings["DELETEContractProduct"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string strparams = "cmprecid=" + CSP_CRECID + "&RECID=" + CSP_RECID + "&userrecid=" +CSP_CURECID + "&productrecid=" + CSP_PRECID+ "&adminrecid=" + CSP_ARECID;

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

                                string redirectUrl = Url.Action("List", "ContractProduct", new { });
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



        [HttpPost]
        public async Task<ActionResult> Edit(ContractServiceProduct contractServiceProduct)
        {
            try
            {
               

                var UpdateURL = ConfigurationManager.AppSettings["PUTContractProduct"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();
               
                var content = $@"{{           
            ""csP_RECID"": ""{Session["CSP_RECID"]}"",           
            ""csP_PRECID"": ""{ Session["CSP_PRECID"]}"",           
            ""csP_PRODUCTNAME"": ""{contractServiceProduct.CSP_PRODUCTNAME}"",           
            ""csP_PRODUCTAMOUNT"": ""{contractServiceProduct.CSP_PRODUCTAMOUNT}"",           
            ""csP_ARECID"": ""{Session["CSP_ARECID"]}"",           
            ""csP_ADMINNAME"": ""{contractServiceProduct.CSP_ADMINNAME}"",           
            ""csP_FREESERVICE"": ""{contractServiceProduct.CSP_FREESERVICE}"",           
            ""csP_INVOICEAMOUNT"": ""{Session["CSP_INVOICEAMOUNT"]}"",           
            ""csP_PAIDAMOUNT"": ""{Session["CSP_PAIDAMOUNT"]}"",           
            ""csP_CRECID"": ""{Session["CompanyID"]}"",           
            ""csP_PRODUCTUNIQUENUMBER"": ""{contractServiceProduct.CSP_PRODUCTUNIQUENUMBER}"",           
            ""csP_USERTYPE"": ""{Session["CSP_USERTYPE"]}"",           
            ""csP_CTRECID"": ""{Session["csP_CTRECID"]}"",           
            ""csP_CURECID"": ""{ Session["csP_CURECID"]}""         
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
                    var apiResponse = JsonConvert.DeserializeObject<RootObjectContractServiceProduct>(responseBody);

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




        public async Task<ActionResult> ContractPDF(int? CP_CTURECID, int? CP_CRECID)
        {
            var Weburl = ConfigurationManager.AppSettings["ContractPDF"];

            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string APIKey = Session["APIKEY"]?.ToString();


            string url = $"{Weburl}?cmprecid={CP_CRECID}&userid={CP_CTURECID}";

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
                    var fileName = Path.GetFileName(pdfUrl); // GstInReport_20250924052413.pdf

                    // Download
                    return File(fileBytes, "application/pdf", fileName);

                }
            }
            catch (Exception ex)
            {
                return Content("Exception occurred: " + ex.Message);
            }
        }



        public async Task<ActionResult> View(int? CSP_RECID)
        {

            string WEBURLGETBYID = ConfigurationManager.AppSettings["GETContractProductById"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            ContractServiceProduct contractServiceProduct = null;

            string strparams = "RecID=" + CSP_RECID + "&CompanyRecID=" + Session["CompanyID"];
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
                            var content = JsonConvert.DeserializeObject<RootObjectContractServiceProduct>(jsonString);
                            contractServiceProduct = content.Data;

                            Session["CSP_RECID"] = content.Data.CSP_RECID;
                            Session["CSP_PRECID"] = content.Data.CSP_PRECID;
                            Session["CSP_ARECID"] = content.Data.CSP_ARECID;
                            Session["CSP_INVOICEAMOUNT"] = content.Data.CSP_INVOICEAMOUNT;
                            Session["CSP_PAIDAMOUNT"] = content.Data.CSP_PAIDAMOUNT;
                            Session["CSP_USERTYPE"] = content.Data.CSP_USERTYPE;
                            Session["csP_CTRECID"] = content.Data.CSP_CTRECID;
                            Session["csP_CURECID"] = content.Data.CSP_CURECID;

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
            //await LoadProductCombo(contractServiceProduct?.CSP_PRECID ?? 0);

            return View(contractServiceProduct);
        }


        public async Task<ActionResult> ListView(int? ContractRecid, int? CustomerRecid, string CP_CODE, string CP_STATUS)
        {
            if (CP_CODE != null)
            {

                Session["CP_CODE"] = CP_CODE;
            }
            if (CP_STATUS != null)
            {

                Session["CP_STATUS"] = CP_STATUS;
            } if (CustomerRecid != null)
            {

                Session["CT_URECID"] = CustomerRecid;
            }

            ContractServiceProduct objproduct = new ContractServiceProduct();

            //int SerialNo = objproduct.SerialNumber;

            //if (SerialNo == 0)
            //{
            //    SerialNo = 1; // Initialize to 1 if it's 0
            //}

            string WEBURLGET = ConfigurationManager.AppSettings["GETContractProductList"];
            string Authkey = ConfigurationManager.AppSettings["Authkey"];

            List<ContractServiceProduct> ContractServiceProductList = new List<ContractServiceProduct>();


            string APIKey = Session["APIKEY"].ToString();

            if (ContractRecid != 0 && ContractRecid != null)
            {
                Session["CTS_RECID"] = ContractRecid.ToString();

            }


            string strparams = "CompanyRecID=" + Session["CompanyID"] + "&ContractRecid=" + Session["CTS_RECID"] + "&CustomerRecid=" + Session["CT_URECID"];
            //CompanyRecID=1&ContractRecid=1&CustomerRecid=1
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
                            var content = JsonConvert.DeserializeObject<RootObjectsContractServiceProduct>(jsonString);
                            ContractServiceProductList = content.Data;

                            //if (ContractServiceProductList.Count > 0)
                            //{
                            //    // Assign serial numbers
                            //    for (int i = 0; i < ContractServiceProductList.Count; i++)
                            //    {
                            //        ContractServiceProductList[i].SerialNumber = i + 1;
                            //    }
                            //}
                            //if (!string.IsNullOrEmpty(searchPharse))
                            //{
                            //    contractsList = contractsList
                            //        .Where(r => r.CT_CONTRACTREFERENCENUMBER.ToLower().Contains(searchPharse.ToLower()) ||
                            //       r.CT_FROMDATE.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                            //       r.CT_TODATE.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                            //       r.CT_CONTRACTAMOUNT.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                            //       r.CT_TOTALPAIDAMOUNT.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                            //       r.CT_CONTRACTCREATEDBY.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                            //       r.CT_CONTRACTAPPROVEDBY.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                            //       r.CT_ANYREFERENCE.ToString().ToLower().Contains(searchPharse.ToLower()) ||
                            //       r.CT_TOTALPAIDAMOUNT.ToString().ToLower().Contains(searchPharse.ToLower()))
                            //        .ToList();

                            //}

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
            return View(ContractServiceProductList);
        }



    }
}