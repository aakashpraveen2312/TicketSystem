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
    public class ContractExpiryReportController : Controller
    {
        // GET: ContractExpiryReport
        public ActionResult ContractexpiryReport()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ContractexpiryReport(Prioritywise prioritywise, DateTime? FromDate, DateTime? ToDate, string Type)
        {
            bool hasDateRange = FromDate.HasValue && ToDate.HasValue;
            bool hasType = !string.IsNullOrWhiteSpace(Type);

            if (!hasDateRange && !hasType)
            {
                TempData["ErrorMessage"] = "Please select either a Date Range or a Type";
                return RedirectToAction("ContractexpiryReport");
            }


            string Weburl = ConfigurationManager.AppSettings["CONTRACTEXPIRYREPORT"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string APIKey = Session["APIKEY"]?.ToString();


            string url = $"{Weburl}?companyRecID={Session["CompanyId"]}&fromDate={FromDate:yyyy-MM-dd}&toDate={ToDate:yyyy-MM-dd}&type={Type}";

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
        public async Task<ActionResult> ProductFinanceReport()
        {
            await ProductCombo();
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ProductFinanceReport(Prioritywise prioritywise,string Type,string Productrecid,string contractrecid)
        {
           
            string Weburl = ConfigurationManager.AppSettings["PRODUCTFINANCEREPORT"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string APIKey = Session["APIKEY"]?.ToString();


            string url = $"{Weburl}?companyRecID={Session["CompanyId"]}&productRecid={Productrecid}&contractRecid={contractrecid}&type={Type}";


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
        public async Task<ActionResult> CustomerFinanceReport()
        {
            await ContractCombo();
            await ProductCombo();
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CustomerFinanceReport(Prioritywise prioritywise, string Type, string CustomerRecid, string ContractRecid,string ProductRecid)
        {

            string Weburl = ConfigurationManager.AppSettings["CUSTOMERFINANCEREPORT"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string APIKey = Session["APIKEY"]?.ToString();


            string url = $"{Weburl}?companyRecID={Session["CompanyId"]}&customerRecid={CustomerRecid}&contractRecid={ContractRecid}&type={Type}&ProductRecid={ProductRecid}";


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


        //ProductCombo
        public async Task<ActionResult> ProductCombo()
        {
            List<SelectListItem> Product = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["PRODUCTGETCOMBO"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"]?.ToString();
            string strParams = "cmprecid=" + Session["CompanyID"];
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
                            var rootObjects = JsonConvert.DeserializeObject<ProductMasterRootObject>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                Product = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.P_RECID.ToString(),
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

            ViewBag.Product = Product;
          
            return View();
        }

        public async Task<JsonResult> ProductBasedContract(string ProductRecid)
        {
            List<SelectListItem> Contractdata = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["PRODUCTGETCONTRACT"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"]?.ToString();
            string strParams = $"Recid={ProductRecid}&CompanyRecID={Session["CompanyID"]}";
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
                            var rootObjects = JsonConvert.DeserializeObject<RootObjectsContract>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                Contractdata = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.CT_RECID.ToString(),
                                    Text = t.CT_CONTRACTREFERENCENUMBER
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

            return Json(new { data = Contractdata }, JsonRequestBehavior.AllowGet);
        }

        //Contract Combo
        public async Task<ActionResult> ContractCombo()
        {
            List<SelectListItem> Contractnumber = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["CONTRACTGETCOMBO"];
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
                            var rootObjects = JsonConvert.DeserializeObject<RootObjectsContract>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                Contractnumber = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.CT_RECID.ToString(),
                                    Text = t.CT_CONTRACTREFERENCENUMBER,
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

            ViewBag.Contractnumber = Contractnumber;

            return View();
        }


        public async Task<JsonResult>CustomerBasedContract(string Productrecid)
        {
            List<SelectListItem> Customerdata = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["CUSTOMERGETCONTRACT"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"]?.ToString();
            string strParams = $"ContractRecID={Productrecid}&CompanyRecID={Session["CompanyID"]}";
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
                                Customerdata = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.CU_RECID.ToString(),
                                    Text = t.CU_NAME
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

            return Json(new { data = Customerdata }, JsonRequestBehavior.AllowGet);
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

        //Material Combo
        public async Task<ActionResult> MaterialCombo()
        {
            List<SelectListItem> Material = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["GETMATERIALSCOMBO"];
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
                            var rootObjects = JsonConvert.DeserializeObject<IRootObjects>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                Material = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.I_RECID.ToString(),
                                    Text = t.I_DESCRIPTION,
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

            ViewBag.Material = Material;

            return View();
        }
        public async Task<ActionResult> ServiceProductReport()
        {
            await CustomerCombo();
            await MaterialCombo();
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ServiceProductReport(Prioritywise prioritywise, string Type, string Productrecid, string contractrecid,string Materialrecid)
        {

            string Weburl = ConfigurationManager.AppSettings["SERVICEPRODUCTREPORT"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string APIKey = Session["APIKEY"]?.ToString();


            string url = $"{Weburl}?companyRecID={Session["CompanyId"]}&customerRecid={Productrecid}&productRecid={contractrecid}&type={Type}&MaterialRecid={Materialrecid}";


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


    }
}