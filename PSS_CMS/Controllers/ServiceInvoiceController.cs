using Newtonsoft.Json;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    public class ServiceInvoiceController : Controller
    {
        // GET: ServiceInvoice
    //    public ActionResult Index()
    //    {
    //        var invoices = new List<ServiceInvoice>
    //{
    //    new ServiceInvoice
    //    {
    //        InvoiceID = 1001,
    //        TicketRef = "TKT123",
    //        CustomerID = "CUST01",
    //        InvoiceDate = DateTime.Today,
    //        Amount = 1000,
    //        CGST = 50,
    //        SGST = 50,
    //        Status = "Paid"
    //    },

    //};

    //        return View(invoices);
    //    }


        public async Task<ActionResult> Index(string searchPharse)
        {
            Projectmaster objprojectmaster = new Projectmaster();

            string Weburl = ConfigurationManager.AppSettings["SERVICEINVOICELIST"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<ServiceInvoice> ServiceInvoiceList = new List<ServiceInvoice>();

            string strparams = "cmprecid=" + Session["CompanyID"];
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
                            ServiceInvoiceList = rootObjects.Data;

                            //if (!string.IsNullOrEmpty(searchPharse))
                            //{
                            //    projectmasterlist = projectmasterlist
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




        public async Task<ActionResult> GeneratePDF(int ticketRecID)
        {
            ServiceInvoiceData invoiceData = new ServiceInvoiceData();

            Session["TicketRecID"] = ticketRecID;
       
            string Weburl = ConfigurationManager.AppSettings["GENERATEPDF"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            
            string strparams = "cmprecid=" + Session["CompanyID"] + "&ticketRecID=" + ticketRecID;
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

                            // Deserialize to your root object
                            var rootObjects = JsonConvert.DeserializeObject<ServiceInvoiceRootObjects>(jsonString);

                            if (rootObjects != null && rootObjects.Status == "Y")
                            {
                                // Billable and NonBillable materials
                                var billable = rootObjects.Data?.BillableMaterials ?? new List<ServiceMaterials>();
                                var nonBillable = rootObjects.Data?.NonBillableMaterials ?? new List<ServiceMaterials>();

                                // Pass to View
                                ViewBag.Billable = billable;
                                ViewBag.NonBillable = nonBillable;

                                // If you want to strongly bind whole object (instead of only ViewBag)
                                return View(rootObjects.Data);
                            }
                            else
                            {
                                // API returned Status != "Y"
                                ViewBag.Billable = new List<ServiceMaterials>();
                                ViewBag.NonBillable = new List<ServiceMaterials>();
                                return View(new ServiceInvoiceData());
                            }
                        }
                        else
                        {
                            // API failure case
                            ViewBag.Billable = new List<ServiceMaterials>();
                            ViewBag.NonBillable = new List<ServiceMaterials>();
                            return View(new ServiceInvoiceData());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
            }

            return View(invoiceData);
        }

        //Download PDF

        public async Task<ActionResult> DownaloadPDF()
        {
            string Weburl = ConfigurationManager.AppSettings["DOWNLOADPDF"];
            string AuthKey = ConfigurationManager.AppSettings["Authkey"];
            string APIKey = Session["APIKEY"]?.ToString();

            int companyId = Convert.ToInt32(Session["CompanyId"]);
            int ticketId = Convert.ToInt32(Session["TicketRecID"]);

            try
            {
                // Build the request body as per JSON structure
                var requestBody = new
                {
                    companyRecID = companyId,
                    ticketRecID = ticketId,
                    materialName = "", 
                    quantity =0,
                    price = 0,
                    cgst = 0,
                    sgst = 0,
                    amount = 0,
                    discount = 0,
                    netAmount = 0,
                    billableType = ""
                };

                using (HttpClientHandler handler = new HttpClientHandler())
                using (HttpClient client = new HttpClient(handler))
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                    client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // 🔑 Send POST request with JSON body
                    var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(Weburl, content);

                    if (!response.IsSuccessStatusCode)
                        return Content("Error fetching data: " + response.ReasonPhrase);

                    var jsonString = await response.Content.ReadAsStringAsync();
                    var rootObjects = JsonConvert.DeserializeObject<ApiPdfResponse>(jsonString);

                    if (rootObjects == null || rootObjects.Status != "Y")
                        return Content(rootObjects?.Message ?? "No data found for the selected criteria.");

                    if (string.IsNullOrEmpty(rootObjects.FileUrl))
                        return Content("PDF URL not returned from API.");

                    // ✅ Fetch the actual PDF bytes
                    var fileBytes = await client.GetByteArrayAsync(rootObjects.FileUrl);
                    var fileName = Path.GetFileName(rootObjects.FileUrl);

                    return File(fileBytes, "application/pdf", fileName); // Forces download/open in browser
                }
            }
            catch (Exception ex)
            {
                return Content("Exception occurred: " + ex.Message);
            }
        }




        public ActionResult UpdatePayment(PaymentUpdate model)
        {
            return View();
        }
        
        //public ActionResult GeneratePDF(PaymentUpdate model)
        //{
        //    return View();
        //}

    }
}