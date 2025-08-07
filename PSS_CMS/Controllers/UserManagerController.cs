using Newtonsoft.Json;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Controllers
{
    public class UserManagerController : Controller
    {
        // GET: CustomerManager
        public async Task<ActionResult> Dashboard()
        {
            string WEBURLGET = ConfigurationManager.AppSettings["CUSTOMERMANAGERCOUNTDASHBOARD"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "Userid=" + Session["UserRECID"] + "&cmprecid=" + Session["CompanyID"];
            string finalurl = WEBURLGET + "?" + strparams;
            DashboardUser dashboardData = null;

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

                        var response = await client.GetAsync(finalurl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            dashboardData = JsonConvert.DeserializeObject<DashboardUser>(jsonString);

                            var totalTickets = dashboardData.TotalTickets;
                            var closedTickets = dashboardData.CloseTickets;
                            var closedPercentage = (closedTickets / (double)totalTickets) * 100;

                            ViewBag.ClosedPercentage = closedPercentage;
                            ViewBag.TotalTickets = totalTickets;
                            ViewBag.ClosedTickets = closedTickets;
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
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }

            // Fetch WTD and MTD chart data
            DashboardUser wtdMtdData = await WTDANDMTDCHARTUSER();
            if (wtdMtdData != null)
            {
                ViewBag.MonthTotalTickets = wtdMtdData.MonthWise?.MonthTotalTickets ?? 0;
                ViewBag.MonthOpenTickets = wtdMtdData.MonthWise?.MonthOpenTickets ?? 0;
                ViewBag.MonthResolvedTickets = wtdMtdData.MonthWise?.MonthResolvedTickets ?? 0;
                ViewBag.MonthCloseTickets = wtdMtdData.MonthWise?.MonthCloseTickets ?? 0;

                ViewBag.WeekTotalTickets = wtdMtdData.WeekWise?.WeekTotalTickets ?? 0;
                ViewBag.WeekOpenTickets = wtdMtdData.WeekWise?.WeekOpenTickets ?? 0;
                ViewBag.WeekResolvedTickets = wtdMtdData.WeekWise?.WeekResolvedTickets ?? 0;
                ViewBag.WeekCloseTickets = wtdMtdData.WeekWise?.WeekCloseTickets ?? 0;
            }

            return View(dashboardData);
        }
        public async Task<DashboardUser> WTDANDMTDCHARTUSER()
        {
            string WEBURLGET = ConfigurationManager.AppSettings["DASHBOARDMTDandWTDCUSTOMERMANAGER"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "Userid=" + Session["UserRECID"] + "&cmprecid=" + Session["CompanyID"];
            string finalurl = WEBURLGET + "?" + strparams;
            DashboardUser wtdMtdData = null;

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

                        var response = await client.GetAsync(finalurl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            wtdMtdData = JsonConvert.DeserializeObject<DashboardUser>(jsonString);
                        }
                        else
                        {
                            Console.WriteLine("Error: " + response.ReasonPhrase);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }
            await StackedBarChartUser();
            return wtdMtdData;
        }
        public async Task<ActionResult> StackedBarChartUser()
        {
            string WEBURLGET = ConfigurationManager.AppSettings["DASHBOARDSTACKEDBARCHARTCUSTOMERMANAGER"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "Userid=" + Session["UserRECID"] + "&cmprecid=" + Session["CompanyID"];
            string finalurl = WEBURLGET + "?" + strparams;
            List<DashboardPriority> dashboardDataPriority = new List<DashboardPriority>();

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

                        var response = await client.GetAsync(finalurl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var rootObjects = JsonConvert.DeserializeObject<DashboardPriorityList>(jsonString);
                            dashboardDataPriority = rootObjects.Data;

                            ViewBag.Labels1 = JsonConvert.SerializeObject(new[] { "Critical", "Emergency", "Urgent", "Normal" });
                            ViewBag.ClosedTickets1 = JsonConvert.SerializeObject(dashboardDataPriority.Select(d => d.ClosedTickets));
                            ViewBag.ResolvedTickets1 = JsonConvert.SerializeObject(dashboardDataPriority.Select(d => d.ResolvedTickets));
                            ViewBag.OpenTickets1 = JsonConvert.SerializeObject(dashboardDataPriority.Select(d => d.OpenTickets));


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
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }

            return View(dashboardDataPriority);
        }
        public async Task<ActionResult> ComboBoxTicketHistory()
        {

            List<SelectListItem> ticketTypes = new List<SelectListItem>();
            string strparams = "cmprecid=" + Session["CompanyID"];
            string webUrlGet = ConfigurationManager.AppSettings["COMBOBOXTICKETTYPE"];

            string url = webUrlGet + "?" + strparams;

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

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
                            var rootObjects = JsonConvert.DeserializeObject<TicketTypeModel>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                ticketTypes = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.TT_TICKETTYPE, // or the appropriate value field
                                    Text = t.TT_TICKETTYPE // or the appropriate text field
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

            // Assuming you are passing ticketTypes to the view
            ViewBag.TicketTypes = ticketTypes;

            return View();
        }
        //new Ticket list view combo project type
        public async Task<ActionResult> ComboBoxTicketHistoryProduct()
        {

            List<SelectListItem> Product = new List<SelectListItem>();

            string webUrlGet = ConfigurationManager.AppSettings["COMBOFORPRODUCTSELECTED"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "companyId=" + Session["CompanyID"] + "&UserID=" + Session["UserRECID"];
            string url = webUrlGet + "?" + strparams;
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
                            var rootObjects = JsonConvert.DeserializeObject<TicketTypeModel>(jsonString);

                            if (rootObjects?.Data != null)
                            {
                                Product = rootObjects.Data.Select(t => new SelectListItem
                                {
                                    Value = t.CU_RECID.ToString(), // or the appropriate value field
                                    Text = t.CU_NAME // or the appropriate text field
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

            // Assuming you are passing ticketTypes to the view
            ViewBag.Product = Product;

            return View();
        }
        public async Task ComboBoxProductNewticket(Tickets viewModel)
        {
            string webUrlGet = ConfigurationManager.AppSettings["COMBOFORPRODUCTSELECTED"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "companyId=" + Session["CompanyID"] + "&UserID=" + Session["UserRECID"];
            string url = webUrlGet + "?" + strparams;

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
                            var rootObjects = JsonConvert.DeserializeObject<ApiResponseTicketsResponseTypes>(jsonString);
                            var ticketTypes2 = rootObjects?.Data ?? new List<TicketComboTypes>();

                            viewModel.TicketCombo2.TicketTypes2 = ticketTypes2.Select(item => new SelectListItem
                            {
                                Value = item.CU_RECID.ToString(),
                                Text = item.CU_NAME
                            }).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Exception occurred: {ex.Message}");
            }
        }


    }
}