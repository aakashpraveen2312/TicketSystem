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
    public class ManagerSubordinateReportController : Controller
    {
        // GET: ManagerSubordinateReport
        public async Task<ActionResult> ManagersubordinateReport()
        {
            await CustomerCombo();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ManagersubordinateReport(Prioritywise prioritywise, string Type, string Productrecid, string contractrecid,string Status,string ActionType)
        {
            List<Prioritywise> list = new List<Prioritywise>();
            string Weburl = ConfigurationManager.AppSettings[
     ActionType == "PDF" ? "SUBORDINATEREPORT" : "SUBORDINATEREPORTLISTVIEW"];

            //string Weburl = ConfigurationManager.AppSettings["SUBORDINATEREPORT"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string APIKey = Session["APIKEY"]?.ToString();


            string url = $"{Weburl}?companyRecID={Session["CompanyId"]}&customerRecid={Productrecid}&productRecid={contractrecid}&type={Type}&status={Status}";


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

                    if (ActionType == "Filter")
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

        //Customer Combo
        public async Task<ActionResult> CustomerCombo()
        {
            List<SelectListItem> Customer = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["CUSTOMERGETCOMBO"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"]?.ToString();
            string strParams = "CompanyRecID=" + Session["CompanyID"];
            string finalUrl = $"{webUrlGet}?{strParams}";

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

                        var response = await client.GetAsync(finalUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var rootObjects = JsonConvert.DeserializeObject<ProjectMasterRootObject>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                Customer = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.CU_RECID.ToString(),
                                    Text = t.CU_NAME,
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

            ViewBag.Customer = Customer;

            return View();
        }

        public async Task<JsonResult> CustomerBasedProduct(string Productrecid)
        {
            List<SelectListItem> Productdata = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["CUSTOMERGETPRODUCT"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"]?.ToString();
            string strParams = $"CustomerRecid={Productrecid}&CompanyRecID={Session["CompanyID"]}";
            string finalUrl = $"{webUrlGet}?{strParams}";

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

                        var response = await client.GetAsync(finalUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var rootObjects = JsonConvert.DeserializeObject<ProjectMasterRootObject>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                Productdata = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.P_RECID.ToString(),
                                    Text = t.P_NAME
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

            return Json(new { data = Productdata }, JsonRequestBehavior.AllowGet);
        }
    }
}