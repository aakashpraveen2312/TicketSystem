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
    public class InvoiceandContractsController : Controller
    {
        // GET: InvoiceandContracts
        [HttpGet]
    
        public async Task<ActionResult> List(int Userid)
        {
            try
            {
                // ================================
                // VALIDATE COMPANY
                // ================================

                if (Session["CompanyID"] == null ||
                    Convert.ToInt32(Session["CompanyID"]) <= 0)
                {
                    TempData["ErrorMessage"] = "Invalid Company ID.";

                    return View(
                        new List<ContractInvoiceList>()
                    );
                }


                // ================================
                // VALIDATE USER
                // ================================

                if (Userid <= 0)
                {
                    TempData["ErrorMessage"] = "Invalid User ID.";

                    return View(
                        new List<ContractInvoiceList>()
                    );
                }


                // ================================
                // GET CONFIG VALUES
                // ================================

                var ProductContractGetURL =
                    ConfigurationManager.AppSettings[
                        "CustomerContractList"
                    ];

                string AuthKey =
                    ConfigurationManager.AppSettings["AuthKey"];

                string APIKey =
                    Session["APIKEY"]?.ToString();


                // ================================
                // VALIDATE API KEY
                // ================================

                if (string.IsNullOrEmpty(APIKey))
                {
                    TempData["ErrorMessage"] = "API Key not found.";

                    return View(
                        new List<ContractInvoiceList>()
                    );
                }


                // ================================
                // BUILD API URL
                // ================================

                int companyId =
                    Convert.ToInt32(Session["CompanyID"]);

                string URL =
                    ProductContractGetURL
                    + "?cmprecid=" + companyId
                    + "&Userid=" + Userid;


                // ================================
                // CREATE REQUEST
                // ================================

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(URL),
                    Method = HttpMethod.Get
                };

                request.Headers.Add("X-Version", "1");

                request.Headers.Add(
                    HttpRequestHeader.Accept.ToString(),
                    "application/json"
                );


                // ================================
                // HTTP HANDLER
                // ================================

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        (sender, cert, chain, sslPolicyErrors) => true
                };


                using (var client = new HttpClient(handler))
                {
                    // ================================
                    // API HEADERS
                    // ================================

                    client.DefaultRequestHeaders.Add(
                        "ApiKey",
                        APIKey
                    );

                    client.DefaultRequestHeaders.Add(
                        "Authorization",
                        AuthKey
                    );


                    // ================================
                    // CALL API
                    // ================================

                    var response =
                        await client.SendAsync(request);

                    string responseBody =
                        await response.Content.ReadAsStringAsync();


                    // ================================
                    // CHECK HTTP RESPONSE
                    // ================================

                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] =
                            "Error while getting contract products.";

                        return View(
                            new List<ContractInvoiceList>()
                        );
                    }


                    // ================================
                    // DESERIALIZE API RESPONSE
                    // ================================

                    var apiResponse =
                        JsonConvert.DeserializeObject<
                            ContractProductResponse
                        >(responseBody);


                    // ================================
                    // SUCCESS
                    // ================================

                    if (apiResponse != null &&
                        apiResponse.Status == "Y")
                    {
                        return View(
                            apiResponse.Data
                            ?? new List<ContractInvoiceList>()
                        );
                    }


                    // ================================
                    // NO DATA / API ERROR
                    // ================================

                    //TempData["ErrorMessage"] =
                    //    apiResponse?.Message
                    //    ?? "No Data Available";

                    return View(
                        new List<ContractInvoiceList>()
                    );
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Exception: " + ex.Message;

                return View(
                    new List<ContractInvoiceList>()
                );
            }
        }



        public async Task<ActionResult> Index(int? Userid , string searchPharse)
        {
            ServiceInvoice objproduct = new ServiceInvoice();

            int SerialNo = objproduct.SerialNumber;

            if (SerialNo == 0)
            {
                SerialNo = 1; // Initialize to 1 if it's 0
            }
            Projectmaster objprojectmaster = new Projectmaster();

            string Weburl = ConfigurationManager.AppSettings["CustomerInvoice"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            Session["Userid"] = Userid;
            List<ServiceInvoice> ServiceInvoiceList = new List<ServiceInvoice>();

            string strparams = "cmprecid=" + Session["CompanyID"]
                    + "&userrecid=" + Session["Userid"];
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
                            var rootObjects = JsonConvert.DeserializeObject<ServiceInvoiceRootObject>(jsonString);
                            //ServiceInvoiceList = rootObjects.Data;
                            // Always keep ServiceInvoiceList non-null
                            ServiceInvoiceList = rootObjects?.Data ?? new List<ServiceInvoice>();


                            if (ServiceInvoiceList.Count > 0)
                            {
                                // Assign serial numbers
                                for (int i = 0; i < ServiceInvoiceList.Count; i++)
                                {
                                    ServiceInvoiceList[i].SerialNumber = i + 1;
                                }
                            }
                            //if (!string.IsNullOrEmpty(searchPharse))
                            //{
                            //    ServiceInvoiceList = projectmasterlist
                            //        .Where(r => r.CU_CODE.ToLower().Contains(searchPharse.ToLower()) ||
                            //                    r.CU_EMAIL.ToString().Contains(searchPharse.ToLower()) ||
                            //                    r.CU_NAME.ToString().Contains(searchPharse.ToLower()) ||
                            //                    r.CU_MOBILENO.ToString().Contains(searchPharse.ToLower()) ||
                            //                    r.CU_INVOICENO.ToString().Contains(searchPharse.ToLower()) ||
                            //                    r.CU_WARRANTYFREECALLS.ToString().Contains(searchPharse.ToLower()) ||
                            //                    r.CU_WARRANTYUPTO.ToString().Contains(searchPharse.ToLower()) ||
                            //                    r.CU_SORTORDER.ToString().Contains(searchPharse.ToLower()))
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
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }
            return View(ServiceInvoiceList);
        }


        [HttpGet]
        public async Task<ActionResult> UserContractInvoice(
int companyId,
int CIH_CONTRACTRECID,
decimal invoiceAmt)
        {
            try
            {
                var GetInvoiceHeaderURL =
                    ConfigurationManager.AppSettings["GETSMCONTRACTINVOICEHEADER"];

                string AuthKey =
                    ConfigurationManager.AppSettings["AuthKey"];

                string APIKey =
                    Session["APIKEY"]?.ToString();

                string URL =
                    GetInvoiceHeaderURL
                    + "?companyId=" + companyId
                    + "&CIH_CONTRACTRECID=" + CIH_CONTRACTRECID;

                Session["companyId"] = companyId;
                Session["CIH_CONTRACTRECID"] = CIH_CONTRACTRECID;
                Session["invoiceAmt"] = invoiceAmt;

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(URL),
                    Method = HttpMethod.Get
                };

                request.Headers.Add("X-Version", "1");
                request.Headers.Add(
                    HttpRequestHeader.Accept.ToString(),
                    "application/json"
                );

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        (sender, cert, chain, sslPolicyErrors) => true
                };

                using (var client = new HttpClient(handler))
                {
                    if (!string.IsNullOrEmpty(APIKey))
                    {
                        client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                    }

                    if (!string.IsNullOrEmpty(AuthKey))
                    {
                        client.DefaultRequestHeaders.Add(
                            "Authorization",
                            AuthKey
                        );
                    }

                    var response = await client.SendAsync(request);

                    // IMPORTANT:
                    // View expects IEnumerable<ContractInvoiceHeader>
                    var lstInvoiceHeader =
                        new List<ContractInvoiceHeader>();

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody =
                            await response.Content.ReadAsStringAsync();

                        var apiResponse =
                            JsonConvert.DeserializeObject<ContractInvoiceHeaderObject>(
                                responseBody
                            );

                        if (apiResponse != null &&
                            apiResponse.Status == "Y" &&
                            apiResponse.Data != null)
                        {
                            lstInvoiceHeader = apiResponse.Data
     .Where(x => x.CIH_STATUS == "Processed")
     .ToList();
                        }
                    }

                    return View(
                        "UserContractInvoice",
                        lstInvoiceHeader
                    );
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                // FIX:
                // Return the SAME model type expected by the View
                return View(
                    "UserContractInvoice",
                    new List<ContractInvoiceHeader>()
                );
            }
        }


        public async Task<ActionResult> UserContractPayment()
        {
            return View();
        }

    }
}