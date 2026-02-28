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
using Newtonsoft.Json;
using PSS_CMS.Fillter;
using PSS_CMS.Models;

namespace PSS_CMS.Controllers
{
    [ApiKeyAuthorize]
    public class PriorityWiseReportController : Controller
    {
        // GET: PriorityWiseReport
        public ActionResult PrioritywiseReport()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> PrioritywiseReport(Prioritywise prioritywise,DateTime? FromDate, DateTime? ToDate,string Type,string ActionType)
        {

            if (FromDate.HasValue && ToDate.HasValue && ToDate < FromDate)
            {
                TempData["ErrorMessage"] = "To Date must be greater than or equal to From Date";
                return RedirectToAction("PrioritywiseReport");
            }
            if (string.IsNullOrWhiteSpace(Type))
            {
                TempData["ErrorMessage"] = "Please select the Type";
                return RedirectToAction("PrioritywiseReport"); // or return same view
            }

            List<Prioritywise> list = new List<Prioritywise>();

            string Weburl = ConfigurationManager.AppSettings[
     ActionType == "PDF" ? "PRIORITYREPORT" : "PRIORITYREPORTLISTVIEW"];


            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string APIKey = Session["APIKEY"]?.ToString();


            string url = $"{Weburl}?companyRecID={Session["CompanyId"]}&userRecID={Session["UserRECID"]}&fromDate={FromDate:yyyy-MM-dd}&toDate={ToDate:yyyy-MM-dd}&type={Type}&userType={Session["UserRole"]}";

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
                    if (ActionType== "Filter")
                    {
                        if (rootObjects != null && rootObjects.Status == "Y")
                        {
                            list = rootObjects.Data;
                        }
                        return View(list);
                    }
                    else
                    {
                        // The API already returns a PDF URL
                        string pdfUrl = rootObjects.fileUrl;
                        var fileBytes = await client.GetByteArrayAsync(pdfUrl);
                        var fileName = Path.GetFileName(pdfUrl); // GstInReport_20250924052413.pdf

                        // Download
                        return File(fileBytes, "application/pdf", fileName);


                    }




                }
            }
            catch (Exception ex)
            {
                return Content("Exception occurred: " + ex.Message);
            }
        }
    
    
    
    
    }
}