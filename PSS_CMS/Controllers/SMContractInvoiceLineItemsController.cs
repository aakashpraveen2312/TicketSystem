using Newtonsoft.Json;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    public class SMContractInvoiceLineItemsController : Controller
    {
        // GET: SMContractInvoiceLineItems
        [HttpGet]
        public async Task<ActionResult> List(
     int? companyId,
     int CIL_CIHRECID,string Status,
    
    string searchPharse = "")
        {
            try
            {
             
                var GetInvoiceLineURL =
                    ConfigurationManager.AppSettings[
                        "GetSMContractInvoiceLine"
                    ];

                string AuthKey =
                    ConfigurationManager.AppSettings["AuthKey"];

                string APIKey =
                    Session["APIKEY"].ToString();

                // Build API URL
                string URL = GetInvoiceLineURL
                    + "?companyId=" + Session["CompanyID"]
                    + "&CIL_CIHRECID=" + CIL_CIHRECID;

                // Store values if required for Add/Edit
                //Session["CompanyID"] = companyId;
                Session["CIL_CIHRECID"] = CIL_CIHRECID;
                Session["Status"] = Status;

                // Create HTTP Request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(URL),
                    Method = HttpMethod.Get,
                    Headers =
            {
                { "X-Version", "1" },
                {
                    HttpRequestHeader.Accept.ToString(),
                    "application/json, application/xml"
                }
            }
                };

                // HTTP Client Handler
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        (sender, cert, chain, sslPolicyErrors) => true
                };

                var client = new HttpClient(handler);

                // API Headers
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add(
                    "Authorization",
                    AuthKey
                );

                // Call API
                var response =
                    await client.SendAsync(request);

                List<ContractInvoiceLineItems> lstInvoiceLines =
                    new List<ContractInvoiceLineItems>();

                if (response.IsSuccessStatusCode)
                {
                    string responseBody =
                        await response.Content.ReadAsStringAsync();

                    var apiResponse =
                        JsonConvert.DeserializeObject<
                            ContractInvoiceLineObject
                        >(responseBody);

                    if (apiResponse != null &&
                        apiResponse.Status == "Y" &&
                        apiResponse.Data != null)
                    {
                        lstInvoiceLines = apiResponse.Data;
                    }
                    else if (apiResponse != null)
                    {
                        TempData["ErrorMessage"] =
                            apiResponse.Message;
                    }
                }
                else
                {
                    TempData["ErrorMessage"] =
                        "Unable to retrieve invoice line items.";
                }
                ViewBag.SearchPhrase = searchPharse;
                return View("List", lstInvoiceLines);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ViewBag.SearchPhrase = searchPharse;
                return View(
                    "List",
                    new List<ContractInvoiceLineItems>()
                );
            }
        }

        public ActionResult Create()
        {

            var model = new ContractInvoiceLineItems();

            return View(model);


        }





        [HttpPost]
        public async Task<ActionResult> Create(
        ContractInvoiceLineItems data)
        {
            try
            {
                var PostInvoiceLineURL =
                    ConfigurationManager.AppSettings[
                        "PostSMContractInvoiceLine"
                    ];

                string AuthKey =
                    ConfigurationManager.AppSettings["AuthKey"];

                string APIKey =
                    Session["APIKEY"].ToString();

                // Set values from Session
                data.CIL_COMPANYRECID =
                    Convert.ToInt32(Session["CompanyID"]);
                Session["CompanyID"]= data.CIL_COMPANYRECID;
                data.CIL_CIHRECID =
                    Convert.ToInt32(Session["CIL_CIHRECID"]);

                // Convert object to JSON
                var content = JsonConvert.SerializeObject(new
                {
                    CIL_CIHRECID = Session["CIL_CIHRECID"],

                    CIL_DESCRIPTION =
                        data.CIL_DESCRIPTION,

                    CIL_PRICE =
                        data.CIL_PRICE,

                    CIL_COMPANYRECID =
                        data.CIL_COMPANYRECID
                });

                // Create HTTP Request
                var request = new HttpRequestMessage
                {
                    RequestUri =
                        new Uri(PostInvoiceLineURL),

                    Method = HttpMethod.Post,

                    Headers =
            {
                { "X-Version", "1" },

                {
                    HttpRequestHeader.Accept.ToString(),
                    "application/json, application/xml"
                }
            },

                    Content = new StringContent(
                        content,
                        System.Text.Encoding.UTF8,
                        "application/json"
                    )
                };

                // HTTP Client Handler
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        (sender, cert, chain, sslPolicyErrors) => true
                };

                var client = new HttpClient(handler);

                // Add API Headers
                client.DefaultRequestHeaders.Add(
                    "ApiKey",
                    APIKey
                );

                client.DefaultRequestHeaders.Add(
                    "Authorization",
                    AuthKey
                );

                // Call API
                var response =
                    await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody =
                        await response.Content.ReadAsStringAsync();

                    var apiResponse =
                        JsonConvert.DeserializeObject<ContractInvoiceLineObject>(
                            responseBody
                        );

                    if (apiResponse != null &&
    apiResponse.Status == "Y")
                    {
                        return Json(new
                        {
                            success = true,

                            message = apiResponse.Message
                                      ?? "Invoice line added successfully.",

                            companyRecId = data.CIL_COMPANYRECID,

                            cihRecId = data.CIL_CIHRECID,
                            Status=Session["Status"]

                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,

                            message =
                                apiResponse?.Message
                                ?? "Invoice line could not be added."
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        success = false,

                        message =
                            "Error: Something went wrong while adding the invoice line."
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,

                    message =
                        "Exception: " + ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult> Edit(
         int companyId,
         int RecordId,string Status)
        {
            try
            {

                Session["RecordId"] = RecordId;
                var GetInvoiceLineByIDURL =
                    ConfigurationManager.AppSettings["GetSMContractInvoiceLineByID"];

                string AuthKey =
                    ConfigurationManager.AppSettings["AuthKey"];

                string APIKey =
                    Session["APIKEY"].ToString();

                Session["Status"] = Status;

                string URL = GetInvoiceLineByIDURL
                    + "?companyId=" + companyId
                    + "&RecordId=" + RecordId;

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(URL),
                    Method = HttpMethod.Get
                };

                request.Headers.Add("X-Version", "1");
                request.Headers.Add(
                    HttpRequestHeader.Accept.ToString(),
                    "application/json, application/xml"
                );

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        (sender, cert, chain, sslPolicyErrors) => true
                };

                var client = new HttpClient(handler);

                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody =
                        await response.Content.ReadAsStringAsync();

                    var apiResponse =
                        JsonConvert.DeserializeObject<ContractInvoiceLineObjectbyID>(
                            responseBody
                        );

                    if (apiResponse != null &&
                        apiResponse.Status == "Y" &&
                        apiResponse.Data != null)
                    {
                        return View(apiResponse.Data);
                    }
                }

                return View(new ContractInvoiceLineItems());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return View(new ContractInvoiceLineItems());
            }
        }



        [HttpPost]
        public async Task<ActionResult> Edit(
    ContractInvoiceLineItems data)
        {
            try
            {
                var UpdateInvoiceLineURL =
                    ConfigurationManager.AppSettings[
                        "UpdateSMContractInvoiceLine"
                    ];

                string AuthKey =
                    ConfigurationManager.AppSettings["AuthKey"];

                string APIKey =
                    Session["APIKEY"].ToString();

                // Set Company ID from Session if required
                if (data.CIL_COMPANYRECID == 0)
                {
                    data.CIL_COMPANYRECID =
                        Convert.ToInt32(Session["CompanyID"]);
                }

                // Build JSON data
                var content = JsonConvert.SerializeObject(new
                {
                    CIL_RECID = Session["RecordId"],

                    CIL_CIHRECID = Session["CIL_CIHRECID"],

                    CIL_DESCRIPTION = data.CIL_DESCRIPTION,

                    CIL_PRICE = data.CIL_PRICE,

                    CIL_COMPANYRECID = data.CIL_COMPANYRECID
                });

                // Create HTTP Request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(UpdateInvoiceLineURL),

                    Method = HttpMethod.Put,

                    Headers =
            {
                { "X-Version", "1" },

                {
                    HttpRequestHeader.Accept.ToString(),
                    "application/json, application/xml"
                }
            },

                    Content = new StringContent(
                        content,
                        System.Text.Encoding.UTF8,
                        "application/json"
                    )
                };

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        (sender, cert, chain, sslPolicyErrors) => true
                };

                using (var client = new HttpClient(handler))
                {
                    // API Headers
                    client.DefaultRequestHeaders.Add(
                        "ApiKey",
                        APIKey
                    );

                    client.DefaultRequestHeaders.Add(
                        "Authorization",
                        AuthKey
                    );

                    // Call API
                    var response =
                        await client.SendAsync(request);

                    string responseBody =
                        await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse =
                            JsonConvert.DeserializeObject<ContractInvoiceLineObject>(
                                responseBody
                            );

                        if (apiResponse != null &&
                            apiResponse.Status == "Y")
                        {
                            return Json(new
                            {
                                success = true,
                                message = apiResponse.Message,
                                CIL_CIHRECID = Session["CIL_CIHRECID"],
                                Status=Session["Status"]?.ToString()
                            });
                        }

                        return Json(new
                        {
                            success = false,
                            message = apiResponse?.Message
                                      ?? "Invoice line could not be updated."
                        });
                    }

                    return Json(new
                    {
                        success = false,
                        message = "Error: Something went wrong while updating the invoice line."
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Exception: " + ex.Message
                });
            }
        }



        [HttpGet]
        public async Task<ActionResult> View(
    int companyId,
    int RecordId)
        {
            try
            {

                Session["RecordId"] = RecordId;
                var GetInvoiceLineByIDURL =
                    ConfigurationManager.AppSettings["GetSMContractInvoiceLineByID"];

                string AuthKey =
                    ConfigurationManager.AppSettings["AuthKey"];

                string APIKey =
                    Session["APIKEY"].ToString();

                string URL = GetInvoiceLineByIDURL
                    + "?companyId=" + companyId
                    + "&RecordId=" + RecordId;

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(URL),
                    Method = HttpMethod.Get
                };

                request.Headers.Add("X-Version", "1");
                request.Headers.Add(
                    HttpRequestHeader.Accept.ToString(),
                    "application/json, application/xml"
                );

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        (sender, cert, chain, sslPolicyErrors) => true
                };

                var client = new HttpClient(handler);

                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody =
                        await response.Content.ReadAsStringAsync();

                    var apiResponse =
                        JsonConvert.DeserializeObject<ContractInvoiceLineObjectbyID>(
                            responseBody
                        );

                    if (apiResponse != null &&
                        apiResponse.Status == "Y" &&
                        apiResponse.Data != null)
                    {
                        return View(apiResponse.Data);
                    }
                }

                return View(new ContractInvoiceLineItems());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return View(new ContractInvoiceLineItems());
            }
        }





        [HttpPost]
        public async Task<ActionResult> DeleteInvoiceLine(
    int InvoiceLineRecId,
    int companyId)
        {
            try
            {
                var DeleteInvoiceLineURL =
                    ConfigurationManager.AppSettings[
                        "DeleteSMContractInvoiceLine"
                    ];

                string AuthKey =
                    ConfigurationManager.AppSettings["AuthKey"];

                string APIKey =
                    Session["APIKEY"].ToString();

                // Add parameters to API URL
                string URL = DeleteInvoiceLineURL
                    + "?InvoiceLineRecId=" + InvoiceLineRecId
                    + "&companyId=" + companyId;

                // Create HTTP Request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(URL),

                    Method = HttpMethod.Delete,

                    Headers =
            {
                { "X-Version", "1" },

                {
                    HttpRequestHeader.Accept.ToString(),
                    "application/json, application/xml"
                }
            }
                };

                // HTTP Client Handler
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        (sender, cert, chain, sslPolicyErrors) => true
                };

                var client = new HttpClient(handler);

                // Add API Headers
                client.DefaultRequestHeaders.Add(
                    "ApiKey",
                    APIKey
                );

                client.DefaultRequestHeaders.Add(
                    "Authorization",
                    AuthKey
                );

                // Call API
                var response =
                    await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody =
                        await response.Content.ReadAsStringAsync();

                    var apiResponse =
                        JsonConvert.DeserializeObject<ContractInvoiceLineObject>(
                            responseBody
                        );

                    if (apiResponse != null &&
                        apiResponse.Status == "Y")
                    {
                        return Json(new
                        {
                            success = true,
                            message = apiResponse.Message
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            message = apiResponse?.Message
                                      ?? "Invoice line could not be deleted."
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message =
                            "Error: Something went wrong while deleting the invoice line."
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Exception: " + ex.Message
                });
            }
        }




    }
}