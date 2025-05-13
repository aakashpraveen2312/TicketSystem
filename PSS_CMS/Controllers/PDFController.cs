using PSS_CMS.Fillter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSS_CMS.Models;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace PSS_CMS.Controllers
{
    [ApiKeyAuthorize]
    public class PDFController : Controller
    {
        public ActionResult DownloadPDF()
        {
            return View();
        }
        // GET: PDF
        [HttpPost]
        public async Task<ActionResult> DownloadPDF(int? TRECID)
        {
            try
            {
                var MaterialcategoryPostURL = ConfigurationManager.AppSettings["RECENTTICKETPDF"];
                string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
                string APIKey = Session["APIKEY"].ToString();

                var content = $@"{{
            ""ticketRecID"": ""{TRECID}"",
            ""companyRecID"": ""{Session["CompanyID"]}""
        }}";

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(MaterialcategoryPostURL),
                    Method = HttpMethod.Post,
                    Headers =
            {
                { "X-Version", "1" },
                { HttpRequestHeader.Accept.ToString(), "application/json" }
            },
                    Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
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
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<pdfdetail>(responseString);

                    if (result.Status == "Y" && !string.IsNullOrEmpty(result.fileUrl))
                    {
                        // Extract file name from full local path
                        var fileName = result.fileUrl;

                        // Build web-accessible URL (adjust base URL if needed)
                        //var requestUrl = HttpContext.Request.Url;
                        //var baseUrl = $"{requestUrl.Scheme}://{requestUrl.Authority}";
                        //var fileUrl = $"{baseUrl}/GeneratedPDFs/{fileName}";

                        return Json(new { status = "success", url = fileName });
                    }


                    return Json(new { status = "error", message = result.Message });
                }
                else
                {
                    return Json(new { status = "error", message = "No material consumptions are there" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = "Exception occurred: " + ex.Message });
            }
        }


    }
}