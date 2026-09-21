using Newtonsoft.Json;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    public class ContractPaymentController : Controller
    {
        // GET: ContractPayment
        [HttpGet]
        public async Task<ActionResult> List(
    int? companyId,
    int? CIP_CIHRECID,
    decimal? PaymentAmount,
       decimal? invoiceAmt,
        string searchPharse = "")
        {
            try
            {

                if (invoiceAmt.HasValue && invoiceAmt.Value > 0)
                {
                    Session["invoiceAmt"] = invoiceAmt.Value;
                }

                decimal currentInvoiceAmt = 0;

                if (Session["invoiceAmt"] != null)
                {
                    currentInvoiceAmt = Convert.ToDecimal(Session["invoiceAmt"]);
                }

                var GetInvoicePaymentURL =
                    ConfigurationManager.AppSettings[
                        "GetSMContractInvoicePayment"
                    ];

                string AuthKey =
                    ConfigurationManager.AppSettings["AuthKey"];

                string APIKey =
                    Session["APIKEY"]?.ToString();

                // Use parameter first, Session as fallback
                int? finalCompanyId = companyId
                    ?? Session["companyId"] as int?;

                int? finalCihRecId = CIP_CIHRECID
                    ?? Session["CIP_CIHRECID"] as int?;

                Session["CIP_CIHRECID"] = finalCihRecId;
                Session["companyId"] = finalCompanyId;
                Session["PaymentAmount"] = PaymentAmount;

                string URL =
                    GetInvoicePaymentURL
                    + "?companyId=" + finalCompanyId
                    + "&CIP_CIHRECID=" + finalCihRecId;

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

                using (var client = new HttpClient(handler))
                {
                    if (!string.IsNullOrEmpty(APIKey))
                    {
                        client.DefaultRequestHeaders.Add(
                            "ApiKey",
                            APIKey
                        );
                    }

                    if (!string.IsNullOrEmpty(AuthKey))
                    {
                        client.DefaultRequestHeaders.Add(
                            "Authorization",
                            AuthKey
                        );
                    }

                    var response =
                        await client.SendAsync(request);

                    var paymentList =
                        new List<ContractInvoicePayment>();

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody =
                            await response.Content.ReadAsStringAsync();

                        var apiResponse =
                            JsonConvert.DeserializeObject<
                                ContractInvoicePaymentObject
                            >(responseBody);

                        if (apiResponse != null &&
                            apiResponse.Status == "Y" &&
                            apiResponse.Data != null)
                        {
                            paymentList = apiResponse.Data;

                            // Avoid Data[0] exception when there are no records
                            if (paymentList.Any())
                            {
                                Session["CIP_PREVIOUSLYPAIDAMOUNT"] =
                                    paymentList[0].CIP_PREVIOUSLYPAIDAMOUNT;
                            }
                            else
                            {
                                // Important: clear old invoice's value
                                Session["CIP_PREVIOUSLYPAIDAMOUNT"] = 0m;
                            }

                            return View(
                                "List",
                                paymentList
                            );
                        }

                        TempData["ErrorMessage"] =
                            apiResponse?.Message
                            ?? "No payment data available.";

                        return View(
                            "List",
                            paymentList
                        );
                    }

                    TempData["ErrorMessage"] =
                        "Something went wrong while retrieving payment details.";

                    return View(
                        "List",
                        paymentList
                    );
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Exception: " + ex.Message;

                return View(
                    "List",
                    new List<ContractInvoicePayment>()
                );
            }
        }
      
        //    [HttpGet]
        //    public async Task<ActionResult> List(
        //int? companyId,
        //int? CIP_CIHRECID,decimal? PaymentAmount,
        //string searchPharse = "")
        //    {
        //        try
        //        {
        //            var GetInvoicePaymentURL =
        //                ConfigurationManager.AppSettings[
        //                    "GetSMContractInvoicePayment"
        //                ];

        //            string AuthKey =
        //                ConfigurationManager.AppSettings["AuthKey"];

        //            string APIKey =
        //                Session["APIKEY"]?.ToString();


        //                Session["CIP_CIHRECID"] = CIP_CIHRECID;
        //                Session["companyId"] = companyId;
        //                Session["PaymentAmount"] = PaymentAmount;

        //            // ================================
        //            // BUILD API URL
        //            // ================================

        //            string URL =
        //                GetInvoicePaymentURL
        //                + "?companyId=" + companyId
        //                + "&CIP_CIHRECID=" + CIP_CIHRECID;


        //            // ================================
        //            // CREATE HTTP REQUEST
        //            // ================================

        //            var request = new HttpRequestMessage
        //            {
        //                RequestUri = new Uri(URL),

        //                Method = HttpMethod.Get,

        //                Headers =
        //        {
        //            { "X-Version", "1" },

        //            {
        //                HttpRequestHeader.Accept.ToString(),
        //                "application/json, application/xml"
        //            }
        //        }
        //            };


        //            // ================================
        //            // HTTP CLIENT HANDLER
        //            // ================================

        //            var handler = new HttpClientHandler
        //            {
        //                ServerCertificateCustomValidationCallback =
        //                    (sender, cert, chain, sslPolicyErrors) => true
        //            };


        //            using (var client = new HttpClient(handler))
        //            {
        //                // ================================
        //                // ADD API HEADERS
        //                // ================================

        //                client.DefaultRequestHeaders.Add(
        //                    "ApiKey",
        //                    APIKey
        //                );

        //                client.DefaultRequestHeaders.Add(
        //                    "Authorization",
        //                    AuthKey
        //                );


        //                // ================================
        //                // CALL API
        //                // ================================

        //                var response =
        //                    await client.SendAsync(request);


        //                if (response.IsSuccessStatusCode)
        //                {
        //                    string responseBody =
        //                        await response.Content.ReadAsStringAsync();


        //                    // ================================
        //                    // DESERIALIZE RESPONSE
        //                    // ================================

        //                    var apiResponse =
        //                        JsonConvert.DeserializeObject<
        //                            ContractInvoicePaymentObject
        //                        >(responseBody);

        //                    Session["CIP_PREVIOUSLYPAIDAMOUNT"] =apiResponse.Data[0].CIP_PREVIOUSLYPAIDAMOUNT;

        //                    if (apiResponse != null &&
        //                        apiResponse.Status == "Y")
        //                    {
        //                        return View(
        //                            apiResponse.Data
        //                            ?? new List<ContractInvoicePayment>()
        //                        );
        //                    }


        //                    TempData["ErrorMessage"] =
        //                        apiResponse?.Message
        //                        ?? "No payment data available.";

        //                    return View(
        //                        new List<ContractInvoicePayment>()
        //                    );
        //                }


        //                // ================================
        //                // API ERROR
        //                // ================================

        //                TempData["ErrorMessage"] =
        //                    "Something went wrong while retrieving payment details.";

        //                return View(
        //                    new List<ContractInvoicePayment>()
        //                );
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            TempData["ErrorMessage"] =
        //                "Exception: " + ex.Message;

        //            return View(
        //                new List<ContractInvoicePayment>()
        //            );
        //        }
        //    }

        public async Task<ActionResult> Create(decimal invoiceAmt)
        {

            Session["PaymentAmount"] = invoiceAmt;
            ContractInvoicePayment payment = new ContractInvoicePayment();

            payment.CIP_INVOICEAMOUNT = (decimal)Session["PaymentAmount"];
            payment.CIP_PREVIOUSLYPAIDAMOUNT =
                Session["CIP_PREVIOUSLYPAIDAMOUNT"] != null
                    ? (decimal)Session["CIP_PREVIOUSLYPAIDAMOUNT"]
                    : 0m;
            return View(payment);

        }


        [HttpPost]
        public async Task<ActionResult> Create(
    ContractInvoicePayment payment)
        {
            try
            {
                var PostInvoicePaymentURL =
                    ConfigurationManager.AppSettings[
                        "PostSMContractInvoicePayment"
                    ];

                string AuthKey =
                    ConfigurationManager.AppSettings["AuthKey"];

                string APIKey =
                    Session["APIKEY"]?.ToString();


                // ================================
                // VALIDATION
                // ================================

                if (payment == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Payment data is required."
                    });
                }


               

                if (payment.CIP_PAYMENTAMOUNT <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Payment Amount must be greater than zero."
                    });
                }

                payment.CIP_CIHRECID = (int)Session["CIP_CIHRECID"];
                payment.CIP_COMPANYRECID = (int)Session["companyId"];

                // ================================
                // SERIALIZE PAYMENT DATA
                // ================================

                string jsonData =
                    JsonConvert.SerializeObject(payment);

                var content =
                    new StringContent(
                        jsonData,
                        Encoding.UTF8,
                        "application/json"
                    );


                // ================================
                // CREATE HTTP CLIENT
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

                    client.DefaultRequestHeaders.Add(
                        "X-Version",
                        "1"
                    );


                    // ================================
                    // CALL API
                    // ================================

                    var response =
                        await client.PostAsync(
                            PostInvoicePaymentURL,
                            content
                        );


                    string responseBody =
                        await response.Content.ReadAsStringAsync();


                    // ================================
                    // DESERIALIZE API RESPONSE
                    // ================================

                    var apiResponse =
                        JsonConvert.DeserializeObject<
                            ContractInvoicePaymentObject
                        >(responseBody);


                    if (response.IsSuccessStatusCode &&
                        apiResponse != null &&
                        apiResponse.Status == "Y")
                    {
                        return Json(new
                        {
                            success = true,

                            message =
                                apiResponse.Message
                                ?? "Payment inserted successfully.",
                                CIP_CIHRECID = apiResponse.CIP_CIHRECID
                        });
                    }


                    return Json(new
                    {
                        success = false,

                        message =
                            apiResponse?.Message
                            ?? "Payment could not be inserted."
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
        public async Task<ActionResult> Edit(
        int companyId,
        int RecordId)
        {
            try
            {
                var GetInvoicePaymentByIDURL =
                ConfigurationManager.AppSettings[
                "GetSMContractInvoicePaymentByID"
                ];

    string AuthKey =
        ConfigurationManager.AppSettings["AuthKey"];

                string APIKey =
                    Session["APIKEY"]?.ToString();


                // ================================
                // VALIDATION
                // ================================

                if (companyId <= 0 || RecordId <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid Data."
                    },
                    JsonRequestBehavior.AllowGet);
                }


                // ================================
                // BUILD API URL
                // ================================

                string URL =
                    GetInvoicePaymentByIDURL
                    + "?companyId=" + companyId
                    + "&RecordId=" + RecordId;


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


                    string responseBody =
                        await response.Content.ReadAsStringAsync();


                    // ================================
                    // DESERIALIZE RESPONSE
                    // ================================

                    var apiResponse =
     JsonConvert.DeserializeObject<
         ContractInvoicePaymentObjectbyID
     >(responseBody);


                    // ================================
                    // SUCCESS
                    // ================================

                    if (response.IsSuccessStatusCode &&
                        apiResponse != null &&
                        apiResponse.Status == "Y" &&
                        apiResponse.Data != null &&
                        apiResponse.Data.Any())
                    {
                        var payment =
                            apiResponse.Data.FirstOrDefault();
                        payment.CIP_PAYMENTAMOUNT = (decimal)Session["CIP_PREVIOUSLYPAIDAMOUNT"];

                        return View(payment);
                    }


                    // ================================
                    // NO DATA / API FAILURE
                    // ================================

                    return Json(new
                    {
                        success = false,

                        message =
                            apiResponse?.Message
                            ?? "No payment data available."
                    },
                    JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,

                    message = "Exception: " + ex.Message
                },
                JsonRequestBehavior.AllowGet);
            }


}

        [HttpPost]
        public async Task<ActionResult> Delete(
    int PaymentRecId,
    int companyId)
        {
            try
            {
                var DeleteInvoicePaymentURL =
                    ConfigurationManager.AppSettings[
                        "DeleteSMContractInvoicePayment"
                    ];

                string AuthKey =
                    ConfigurationManager.AppSettings["AuthKey"];

                string APIKey =
                    Session["APIKEY"]?.ToString();


                // ================================
                // VALIDATION
                // ================================

                if (PaymentRecId <= 0 || companyId <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid Data."
                    });
                }


                // ================================
                // BUILD API URL
                // ================================

                string URL =
                    DeleteInvoicePaymentURL
                    + "?PaymentRecId=" + PaymentRecId
                    + "&companyId=" + companyId;


                // ================================
                // CREATE HTTP REQUEST
                // ================================

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


                    string responseBody =
                        await response.Content.ReadAsStringAsync();


                    // ================================
                    // DESERIALIZE RESPONSE
                    // ================================

                    var apiResponse =
                        JsonConvert.DeserializeObject<
                            ContractInvoicePaymentObject
                        >(responseBody);


                    // ================================
                    // SUCCESS
                    // ================================

                    if (response.IsSuccessStatusCode &&
                        apiResponse != null &&
                        apiResponse.Status == "Y")
                    {
                        return Json(new
                        {
                            success = true,

                            message =
                                apiResponse.Message
                                ?? "Deleted successfully."
                        });
                    }


                    // ================================
                    // API FAILURE
                    // ================================

                    return Json(new
                    {
                        success = false,

                        message =
                            apiResponse?.Message
                            ?? "Payment could not be deleted."
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
        public async Task<ActionResult> DownloadSMContractInvoicePaymentReceipt(
    int companyId,
    int RecordId)
        {
            try
            {
                if (companyId <= 0 || RecordId <= 0)
                {
                    TempData["ErrorMessage"] = "Invalid Data.";

                    return RedirectToAction("List");
                }

                // ==========================================
                // API URL
                // ==========================================

                var ReceiptURL =
                    ConfigurationManager.AppSettings[
                        "DownloadSMContractInvoicePaymentReceipt"
                    ];

                string AuthKey =
                    ConfigurationManager.AppSettings["AuthKey"];

                string APIKey =
                    Session["APIKEY"]?.ToString();

                if (string.IsNullOrEmpty(APIKey))
                {
                    TempData["ErrorMessage"] = "API Key not found.";

                    return RedirectToAction("List");
                }


                // ==========================================
                // BUILD API URL
                // ==========================================

                string URL =
                    ReceiptURL
                    + "?companyId=" + companyId
                    + "&RecordId=" + RecordId;


                // ==========================================
                // CREATE HTTP REQUEST
                // ==========================================

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(URL),

                    Method = HttpMethod.Get
                };

                request.Headers.Add("X-Version", "1");

                request.Headers.Add(
                    HttpRequestHeader.Accept.ToString(),
                    "application/pdf"
                );


                // ==========================================
                // HTTP CLIENT HANDLER
                // ==========================================

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        (sender, cert, chain, sslPolicyErrors) => true
                };


                using (var client = new HttpClient(handler))
                {
                    // ==========================================
                    // ADD HEADERS
                    // ==========================================

                    client.DefaultRequestHeaders.Add(
                        "ApiKey",
                        APIKey
                    );

                    client.DefaultRequestHeaders.Add(
                        "Authorization",
                        AuthKey
                    );


                    // ==========================================
                    // CALL API
                    // ==========================================

                    var response =
                        await client.SendAsync(request);


                    // ==========================================
                    // ERROR RESPONSE
                    // ==========================================

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorResponse =
                            await response.Content.ReadAsStringAsync();

                        TempData["ErrorMessage"] =
                            "Unable to generate payment receipt.";

                        return RedirectToAction("List");
                    }


                    // ==========================================
                    // READ PDF BYTES
                    // ==========================================

                    byte[] pdfBytes =
                        await response.Content.ReadAsByteArrayAsync();


                    // ==========================================
                    // GET FILE NAME
                    // ==========================================

                    string fileName =
                        $"PaymentReceipt_{RecordId}.pdf";


                    // Try to get filename from API response header
                    if (response.Content.Headers.ContentDisposition != null &&
                        !string.IsNullOrEmpty(
                            response.Content.Headers
                                .ContentDisposition
                                .FileName))
                    {
                        fileName =
                            response.Content.Headers
                                .ContentDisposition
                                .FileName
                                .Trim('"');
                    }


                    // ==========================================
                    // RETURN PDF FOR DOWNLOAD
                    // ==========================================

                    return File(
                        pdfBytes,
                        "application/pdf",
                        fileName
                    );
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] =
                    "Exception: " + ex.Message;

                return RedirectToAction("List");
            }
        }




    }
}