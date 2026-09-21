using Newtonsoft.Json;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    public class SMContractInvoiceHeaderController : Controller
    {
        // GET: SMContractInvoiceHeader

        [HttpGet]
        public async Task<ActionResult> List(
    int companyId,
    int CIH_CONTRACTRECID,
    decimal? invoiceAmt,
    string searchPharse = "")
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

                if (invoiceAmt.HasValue && invoiceAmt.Value > 0)
                {
                    Session["invoiceAmt"] = invoiceAmt.Value;
                }

                decimal currentInvoiceAmt = 0;

                if (Session["invoiceAmt"] != null)
                {
                    currentInvoiceAmt = Convert.ToDecimal(Session["invoiceAmt"]);
                }

                ViewBag.invoiceAmt = currentInvoiceAmt;
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
                            lstInvoiceHeader = apiResponse.Data;
                        }
                    }
                    // Pass search value back to View
                    ViewBag.SearchPhrase = searchPharse;
                    return View(
                        "List",
                        lstInvoiceHeader
                    );
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                // Pass search value back to View
                ViewBag.SearchPhrase = searchPharse;
                // FIX:
                // Return the SAME model type expected by the View
                return View(
                    "List",
                    new List<ContractInvoiceHeader>()
                );
            }
        }
       
        
        public async Task<ActionResult> AddInvoice()
        {
            return View();

        }



        [HttpPost]
        public async Task<ActionResult> AddInvoice(
    ContractInvoiceHeader data)
        {
            try
            {
                var PostInvoiceHeaderURL =
                    ConfigurationManager.AppSettings["PostSMContractInvoiceHeader"];

                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                // Set values from Session
                data.CIH_COMPANYRECID = Convert.ToInt32(Session["CompanyID"]);

                if (data.CIH_CONTRACTUSERRECID == 0)
                {
                    data.CIH_CONTRACTUSERRECID =
                        Convert.ToInt32(Session["UserRECID"]);
                }

                // Convert object to JSON
                var content = JsonConvert.SerializeObject(new
                {
                    CIH_CONTRACTUSERRECID = data.CIH_CONTRACTUSERRECID,
                    CIH_CONTRACTRECID = Session["CIH_CONTRACTRECID"],
                    CIH_COMPANYRECID = data.CIH_COMPANYRECID,
                    CIH_INVOICENO = data.CIH_INVOICENO,
                    CIH_TOTALINVOICEAMOUNT = data.CIH_TOTALINVOICEAMOUNT,
                    CIH_INVOICEDATE = data.CIH_INVOICEDATE,
                    CIH_STATUS = "New"
                });

                // Create HTTP Request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(PostInvoiceHeaderURL),
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

                // API Headers
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                // Call API
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody =
                        await response.Content.ReadAsStringAsync();

                    var apiResponse =
                        JsonConvert.DeserializeObject<UserGroupObjects>(
                            responseBody);

                    if (apiResponse != null && apiResponse.Status == "Y")
                    {
                        TempData["SuccessMessage"] = apiResponse.Message;

                        return RedirectToAction(
                            "List",
                            new
                            {
                                companyId = Session["companyId"],
                                CIH_CONTRACTRECID = Session["CIH_CONTRACTRECID"],
                                invoiceAmt = Session["invoiceAmt"]
                            });
                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            message = apiResponse?.Message ?? "Record not inserted."
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Error: Something went wrong."
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

        [HttpPost]
        public async Task<ActionResult> EditInvoice(
    ContractInvoiceHeader data)
        {
            try
            {
                var UpdateInvoiceHeaderURL =
                    ConfigurationManager.AppSettings[
                        "UpdateSMContractInvoiceHeader"
                    ];

                string AuthKey =
                    ConfigurationManager.AppSettings["AuthKey"];

                string APIKey =
                    Session["APIKEY"].ToString();

                // Set values from Session
                data.CIH_COMPANYRECID =
                    Convert.ToInt32(Session["CompanyID"]);

                data.CIH_CONTRACTRECID =
                    Convert.ToInt32(Session["CIH_CONTRACTRECID"]);

                if (data.CIH_CONTRACTUSERRECID == 0)
                {
                    data.CIH_CONTRACTUSERRECID =
                        Convert.ToInt32(Session["UserRECID"]);
                }

                // Convert Model to JSON
                var content = JsonConvert.SerializeObject(new
                {
                    CIH_RECID = Session["RecordId"],

                    CIH_CONTRACTUSERRECID =
                        data.CIH_CONTRACTUSERRECID,

                    CIH_CONTRACTRECID =
                        data.CIH_CONTRACTRECID,

                    CIH_COMPANYRECID =
                        data.CIH_COMPANYRECID,

                    CIH_INVOICENO =
                        data.CIH_INVOICENO,

                    CIH_INVOICEDATE =
                        data.CIH_INVOICEDATE,

                    CIH_STATUS =
                        data.CIH_STATUS
                });

                // Create HTTP Request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(UpdateInvoiceHeaderURL),

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

                // HTTP Client Handler
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
                    var response = await client.SendAsync(request);

                    string responseBody =
                        await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse =
                            JsonConvert.DeserializeObject<UserGroupObjects>(
                                responseBody
                            );

                        if (apiResponse != null &&
                            apiResponse.Status == "Y")
                        {
                            return RedirectToAction(
                        "List",
                        new
                        {
                            companyId = Session["companyId"],
                            CIH_CONTRACTRECID = Session["CIH_CONTRACTRECID"],
                            invoiceAmt= Session["invoiceAmt"]

                        });
                        }
                        else
                        {
                            return Json(new
                            {
                                success = false,
                                message = apiResponse?.Message ??
                                          "Invoice could not be updated."
                            });
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            message =
                                "Error: Something went wrong while updating the invoice."
                        });
                    }
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
        public async Task<ActionResult> EditInvoice(
    int companyId,
    int RecordId)
        {
            try
            {
                var GetInvoiceByIDURL =
                    ConfigurationManager.AppSettings[
                        "GetSMContractInvoiceHeaderByID"
                    ];

                string AuthKey =
                    ConfigurationManager.AppSettings["AuthKey"];

                string APIKey =
                    Session["APIKEY"].ToString();

                // Add parameters to API URL
                string URL = GetInvoiceByIDURL
                    + "?companyId=" + companyId
                    + "&RecordId=" + RecordId;


                Session["RecordId"] = RecordId;


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

                // Add API Headers
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                // Call API
                var response = await client.SendAsync(request);

                ContractInvoiceHeader invoice =
                    new ContractInvoiceHeader();

                if (response.IsSuccessStatusCode)
                {
                    string responseBody =
                        await response.Content.ReadAsStringAsync();

                    var apiResponse =
                        JsonConvert.DeserializeObject<
                            ContractInvoiceHeaderObject
                        >(responseBody);

                    if (apiResponse != null &&
                        apiResponse.Status == "Y" &&
                        apiResponse.Data != null &&
                        apiResponse.Data.Count > 0)
                    {
                        // API returns List, so get first record
                        invoice = apiResponse.Data.FirstOrDefault();
                    }
                    else
                    {
                        TempData["ErrorMessage"] =
                            apiResponse?.Message ?? "Invoice not found.";

                        return RedirectToAction(
                            "List",
                            new
                            {
                                companyId = companyId,
                                CIH_CONTRACTRECID =
                                    Session["CIH_CONTRACTRECID"],
                                invoiceAmt= Session["invoiceAmt"]
                            });
                    }
                }
                else
                {
                    TempData["ErrorMessage"] =
                        "Error: Unable to get invoice details.";

                    return RedirectToAction(
                        "List",
                        new
                        {
                            companyId = companyId,
                            CIH_CONTRACTRECID =
                                Session["CIH_CONTRACTRECID"]
                        });
                }

                return View("EditInvoice", invoice);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Exception: " + ex.Message;

                return RedirectToAction(
                    "List",
                    new
                    {
                        companyId = companyId,
                        CIH_CONTRACTRECID =
                            Session["CIH_CONTRACTRECID"]
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
                var GetInvoiceByIDURL =
                    ConfigurationManager.AppSettings[
                        "GetSMContractInvoiceHeaderByID"
                    ];

                string AuthKey =
                    ConfigurationManager.AppSettings["AuthKey"];

                string APIKey =
                    Session["APIKEY"].ToString();

                // Add parameters to API URL
                string URL = GetInvoiceByIDURL
                    + "?companyId=" + companyId
                    + "&RecordId=" + RecordId;


                Session["RecordId"] = RecordId;


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

                // Add API Headers
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                // Call API
                var response = await client.SendAsync(request);

                ContractInvoiceHeader invoice =
                    new ContractInvoiceHeader();

                if (response.IsSuccessStatusCode)
                {
                    string responseBody =
                        await response.Content.ReadAsStringAsync();

                    var apiResponse =
                        JsonConvert.DeserializeObject<
                            ContractInvoiceHeaderObject
                        >(responseBody);

                    if (apiResponse != null &&
                        apiResponse.Status == "Y" &&
                        apiResponse.Data != null &&
                        apiResponse.Data.Count > 0)
                    {
                        // API returns List, so get first record
                        invoice = apiResponse.Data.FirstOrDefault();
                    }
                    else
                    {
                        TempData["ErrorMessage"] =
                            apiResponse?.Message ?? "Invoice not found.";

                        return RedirectToAction(
                            "List",
                            new
                            {
                                companyId = companyId,
                                CIH_CONTRACTRECID =
                                    Session["CIH_CONTRACTRECID"]
                            });
                    }
                }
                else
                {
                    TempData["ErrorMessage"] =
                        "Error: Unable to get invoice details.";

                    return RedirectToAction(
                        "List",
                        new
                        {
                            companyId = companyId,
                            CIH_CONTRACTRECID =
                                Session["CIH_CONTRACTRECID"]
                        });
                }

                return View("View", invoice);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Exception: " + ex.Message;

                return RedirectToAction(
                    "List",
                    new
                    {
                        companyId = companyId,
                        CIH_CONTRACTRECID =
                            Session["CIH_CONTRACTRECID"]
                    });
            }
        }


   
        public async Task<ActionResult> DeleteInvoice(
    int InvoiceHeaderRecId,
    int companyId)
        {
            try
            {
                var DeleteInvoiceURL =
                    ConfigurationManager.AppSettings[
                        "DeleteSMContractInvoiceHeader"
                    ];

                string AuthKey =
                    ConfigurationManager.AppSettings["AuthKey"];

                string APIKey =
                    Session["APIKEY"].ToString();

                // Add parameters to API URL
                string URL = DeleteInvoiceURL
                    + "?InvoiceHeaderRecId=" + InvoiceHeaderRecId
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
                client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                client.DefaultRequestHeaders.Add("Authorization", AuthKey);

                // Call API
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody =
                        await response.Content.ReadAsStringAsync();

                    var apiResponse =
                        JsonConvert.DeserializeObject<UserGroupObjects>(
                            responseBody);

                    if (apiResponse != null &&
                        apiResponse.Status == "Y")
                    {
                        //return Json(new
                        //{
                        //    success = true,
                        //    message = apiResponse.Message
                        //},JsonRequestBehavior.AllowGet);

                        return RedirectToAction(
                        "List",
                        new
                        {
                            companyId = Session["companyId"],
                            CIH_CONTRACTRECID = Session["CIH_CONTRACTRECID"],
                            invoiceAmt= Session["invoiceAmt"]

                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            message = apiResponse?.Message ??
                                      "Invoice could not be deleted."
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Error: Something went wrong while deleting the invoice."
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


        //Process
        [HttpGet]
        public async Task<ActionResult> GenerateContractInvoicePdf(
    int companyId,
    int InvoiceHeaderRecId)
        {
            try
            {
                var ContractInvoicePdfURL =
                    ConfigurationManager.AppSettings["CONTRACTINVOICEPDF"];

                string AuthKey =
                    ConfigurationManager.AppSettings["AuthKey"];

                string APIKey =
                    Session["APIKEY"]?.ToString();

                if (string.IsNullOrEmpty(APIKey))
                {
                    return Json(new
                    {
                        success = false,
                        message = "API Key not available. Please login again."
                    }, JsonRequestBehavior.AllowGet);
                }

                // ================================
                // BUILD API URL
                // ================================

                string URL =
                    ContractInvoicePdfURL
                    + "?companyId=" + companyId
                    + "&InvoiceHeaderRecId=" + InvoiceHeaderRecId;


                // ================================
                // CREATE HTTP REQUEST
                // ================================

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(URL),

                    Method = HttpMethod.Get,

                    Headers =
            {
                { "X-Version", "1" },
                {
                    HttpRequestHeader.Accept.ToString(),
                    "application/json"
                }
            }
                };


                // ================================
                // HTTP CLIENT HANDLER
                // ================================

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        (sender, cert, chain, sslPolicyErrors) => true
                };

                using (var client = new HttpClient(handler))
                {
                    // ================================
                    // ADD API HEADERS
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


                    // ================================
                    // READ RESPONSE
                    // ================================

                    string responseBody =
                        await response.Content.ReadAsStringAsync();


                    // ================================
                    // SUCCESS RESPONSE
                    // ================================

                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse =
                            JsonConvert.DeserializeObject<
                                ContractInvoicePdfResponse
                            >(responseBody);

                        if (apiResponse != null &&
                            apiResponse.Status == "Y")
                        {
                            return Json(new
                            {
                                success = true,
                                message = apiResponse.Message,
                                fileUrl = apiResponse.FileUrl
                            }, JsonRequestBehavior.AllowGet);
                        }

                        return Json(new
                        {
                            success = false,
                            message =
                                apiResponse?.Message ??
                                "PDF generation failed."
                        }, JsonRequestBehavior.AllowGet);
                    }


                    // ================================
                    // API ERROR RESPONSE
                    // ================================

                    return Json(new
                    {
                        success = false,
                        message =
                            "Error while generating invoice PDF."
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Exception: " + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }




        [HttpGet]
        public async Task<ActionResult> DownloadContractInvoicePdf(
    string fileUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(fileUrl))
                {
                    return Content("Invalid PDF URL.");
                }

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        (sender, cert, chain, sslPolicyErrors) => true
                };

                using (var client = new HttpClient(handler))
                {
                    var response =
                        await client.GetAsync(fileUrl);

                    if (!response.IsSuccessStatusCode)
                    {
                        return Content(
                            "Unable to download the PDF."
                        );
                    }

                    var pdfBytes =
                        await response.Content.ReadAsByteArrayAsync();

                    string fileName =
                        Path.GetFileName(
                            new Uri(fileUrl).LocalPath
                        );

                    return File(
                        pdfBytes,
                        "application/pdf",
                        fileName
                    );
                }
            }
            catch (Exception ex)
            {
                return Content(
                    "Error downloading PDF: " + ex.Message
                );
            }
        }


    }
}