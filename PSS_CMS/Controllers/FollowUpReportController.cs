using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Newtonsoft.Json;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    public class FollowUpReportController : Controller
    {
        // GET: FollowUpReport
        public async Task<ActionResult> Report() {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Report(
    DateTime? fromDate,
    DateTime? toDate,
    string status)
        {
            string WEBURL = ConfigurationManager.AppSettings["GetCustomerNotificationFollowUpReport"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<Customernotification> notifications = new List<Customernotification>();

            string strparams = "cmprecid=" + Session["CompanyID"];

            if (fromDate.HasValue)
                strparams += "&fromDate=" + fromDate.Value.ToString("yyyy-MM-dd");

            if (toDate.HasValue)
                strparams += "&toDate=" + toDate.Value.ToString("yyyy-MM-dd");

            if (!string.IsNullOrWhiteSpace(status))
                strparams += "&status=" + status;

            string finalurl = WEBURL + "?" + strparams;

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback +=
                        (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                        client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                        client.DefaultRequestHeaders.Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var response = await client.GetAsync(finalurl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var content =
                                JsonConvert.DeserializeObject<CustomernotificationRootObjects>(jsonString);

                            if (content != null && content.Status == "Y")
                            {
                                notifications = content.Data;
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty,
                                "Error: " + response.ReasonPhrase);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty,
                    "Exception occured: " + ex.Message);
            }

            return View(notifications);
        }
            
        public async Task<ActionResult> FollowUpReportPdfDownload(
    DateTime? fromDate,
    DateTime? toDate,
    string status)
        {
            string Weburl = ConfigurationManager
                .AppSettings["GenerateFollowUpReportPdf"];

            string apiKey = Session["APIKEY"]?.ToString();
            string authKey = ConfigurationManager.AppSettings["AuthKey"];

            int companyId = Convert.ToInt32(Session["CompanyID"]);

            // Build query string
            string finalUrl = $"{Weburl}?cmprecid={companyId}";

            if (fromDate.HasValue)
                finalUrl += $"&fromDate={fromDate.Value:yyyy-MM-dd}";

            if (toDate.HasValue)
                finalUrl += $"&toDate={toDate.Value:yyyy-MM-dd}";

            if (string.IsNullOrWhiteSpace(status))
            {
                finalUrl += $"&status=All";
            }
            else {
           
                finalUrl += $"&status={status}";
            }
            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback +=
                        (s, c, ch, e) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("ApiKey", apiKey);
                        client.DefaultRequestHeaders.Add("Authorization", authKey);

                        var response = await client.GetAsync(finalUrl);

                        if (!response.IsSuccessStatusCode)
                            return Content("PDF download failed.");

                        byte[] pdfBytes = await response.Content.ReadAsByteArrayAsync();

                        return File(
                            pdfBytes,
                            "application/pdf",
                            $"FollowUpReport_{DateTime.Now:yyyyMMddHHmmss}.pdf"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }













        //    [HttpPost]
        //    public async Task<ActionResult> FollowUpReportPdf(
        //DateTime? fromDate,
        //DateTime? toDate,
        //string status)
        //    {
        //        string WEBURL = ConfigurationManager.AppSettings["GetCustomerNotificationFollowUpReport"];
        //        string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
        //        string APIKey = Session["APIKEY"].ToString();

        //        List<Customernotification> data = new List<Customernotification>();

        //        string strparams = "cmprecid=" + Session["CompanyID"];

        //        if (fromDate.HasValue)
        //            strparams += "&fromDate=" + fromDate.Value.ToString("yyyy-MM-dd");

        //        if (toDate.HasValue)
        //            strparams += "&toDate=" + toDate.Value.ToString("yyyy-MM-dd");

        //        if (!string.IsNullOrWhiteSpace(status))
        //            strparams += "&status=" + status;

        //        string finalurl = WEBURL + "?" + strparams;

        //        try
        //        {
        //            using (HttpClientHandler handler = new HttpClientHandler())
        //            {
        //                handler.ServerCertificateCustomValidationCallback +=
        //                    (sender, cert, chain, sslPolicyErrors) => true;

        //                using (HttpClient client = new HttpClient(handler))
        //                {
        //                    client.DefaultRequestHeaders.Add("ApiKey", APIKey);
        //                    client.DefaultRequestHeaders.Add("Authorization", AuthKey);
        //                    client.DefaultRequestHeaders.Accept
        //                        .Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //                    var response = await client.GetAsync(finalurl);

        //                    if (response.IsSuccessStatusCode)
        //                    {
        //                        var json = await response.Content.ReadAsStringAsync();
        //                        var apiResponse =
        //                            JsonConvert.DeserializeObject<CustomernotificationRootObjects>(json);

        //                        if (apiResponse?.Status == "Y")
        //                            data = apiResponse.Data;
        //                    }
        //                }
        //            }

        //            // Generate PDF
        //            byte[] pdfBytes = GenerateFollowUpPdf(data, fromDate, toDate, status);

        //            return File(
        //                pdfBytes,
        //                "application/pdf",
        //                $"FollowUpReport_{DateTime.Now:yyyyMMddHHmmss}.pdf");
        //        }
        //        catch (Exception ex)
        //        {
        //            TempData["Error"] = ex.Message;
        //            return RedirectToAction("FollowUpReport");
        //        }
        //    }

        //    public byte[] GenerateFollowUpPdf(
        //        List<Customernotification> data,
        //        DateTime? fromDate,
        //        DateTime? toDate,
        //        string status)
        //    {
        //        using (var ms = new MemoryStream())
        //        {
        //            var writer = new PdfWriter(ms);
        //            var pdf = new PdfDocument(writer);
        //            var doc = new Document(pdf);

        //            // Title
        //            doc.Add(new Paragraph("Follow-Up Report")
        //                .SetFontSize(16)
        //                .SimulateBold()
        //                .SetTextAlignment(TextAlignment.CENTER));

        //            // Filters
        //            doc.Add(new Paragraph(
        //                $"From: {fromDate?.ToString("dd-MM-yyyy") ?? "All"}   " +
        //                $"To: {toDate?.ToString("dd-MM-yyyy") ?? "All"}   " +
        //                $"Status: {status ?? "All"}")
        //                .SetFontSize(10)
        //                .SetMarginBottom(10));

        //            // ✅ Correct number of columns (7)
        //            Table table = new Table(7).UseAllAvailableWidth();

        //            AddHeader(table, "Invoice No");
        //            AddHeader(table, "Customer Name");
        //            AddHeader(table, "Email ID");
        //            AddHeader(table, "Mobile No");
        //            AddHeader(table, "Remarks");
        //            AddHeader(table, "Next Follow-Up");
        //            AddHeader(table, "Status");

        //            foreach (var item in data)
        //            {
        //                table.AddCell(item.CN_INVOICENO ?? "");
        //                table.AddCell(item.CU_NAME ?? "");
        //                table.AddCell(item.CU_EMAIL ?? "");
        //                table.AddCell(item.CU_MOBILENO ?? "");
        //                table.AddCell(item.CN_COMMENTS ?? "");
        //                table.AddCell(item.CN_FOLLOWUPDATE ?? "");
        //                table.AddCell(item.CN_STATUS ?? "");
        //            }

        //            doc.Add(table);
        //            doc.Close();

        //            return ms.ToArray();
        //        }
        //    }

        //    private void AddHeader(Table table, string text)
        //    {
        //        table.AddHeaderCell(
        //            new Cell()
        //                .Add(new Paragraph(text).SimulateBold())
        //                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
        //        );
        //    }


    }
}