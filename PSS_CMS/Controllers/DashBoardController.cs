using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class DashBoardController : Controller
    {
        public async Task<ActionResult> Dashboard()
        {
            string WEBURLGET = ConfigurationManager.AppSettings["DASHBOARDGET"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "Userid=" + Session["UserRECID"] + "&type="+ Session["UserRole"] + "&cmprecid=" + Session["CompanyID"];
            string finalurl = WEBURLGET + "?" + strparams;
            Dashborardchart dashboardData = null;

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
                            dashboardData = JsonConvert.DeserializeObject<Dashborardchart>(jsonString);

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
            Dashborardchart wtdMtdData = await WTDANDMTDCHART();
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

        public async Task<Dashborardchart> WTDANDMTDCHART()
        {
            string WEBURLGET = ConfigurationManager.AppSettings["DASHBOARDMTDandWTD"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams = "Userid=" + Session["UserRECID"] + "&type=" + Session["UserRole"] + "&cmprecid=" + Session["CompanyID"];
            string finalurl = WEBURLGET + "?" + strparams;
            Dashborardchart wtdMtdData = null;

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
                            wtdMtdData = JsonConvert.DeserializeObject<Dashborardchart>(jsonString);
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
            await StackedBarChart();
            return wtdMtdData;
        }

        public async Task<ActionResult> TimelineChart(int ?recid3)
        {
            int recid = recid3 ?? 0;
            string webUrlGet = ConfigurationManager.AppSettings["TIMELINECHART"];
            
            string APIKey = Session["APIKEY"].ToString();
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string strparams = "recid=" + recid3 + "&cmprecid=" + Session["CompanyID"];
            string finalurl = webUrlGet + "?" + strparams;
           

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                        client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response = await client.GetAsync(finalurl);

                        if (!response.IsSuccessStatusCode)
                        {
                            TempData["ErrorMessage"] = $"API Error: {response.StatusCode} - {response.ReasonPhrase}";
                            return View("Error");
                        }

                        var jsonString = await response.Content.ReadAsStringAsync();
                        TimelineResponse timelineData = JsonConvert.DeserializeObject<TimelineResponse>(jsonString);

                        return View(timelineData);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Exception occurred: {ex.Message}";
                return View("Error");
            }
        }

        public async Task<ActionResult> DashboardListTotalTicket(string searchPharse,string status)
        {
            DashBoardList objtotallist = new DashBoardList();

            string Weburl = ConfigurationManager.AppSettings["DASHBOARDLISTTOTALTICKETS"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<DashBoardList> totalticketlist = new List<DashBoardList>();

            string strparams = "Userid=" + Session["UserRECID"] + "&type=" + Session["UserRole"] + "&cmprecid=" + Session["CompanyID"];

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
                            var rootObjects = JsonConvert.DeserializeObject<ObjectsDashBoardList>(jsonString);

                            if(status== "TotalTickets")
                            {
                                Session["Name"] = "TotalTickets";
                                totalticketlist = rootObjects.TotalTickets;
                            }
                            if (status == "OpenTickets")
                            {
                                Session["Name"] = "OpenTickets";
                                totalticketlist = rootObjects.OpenTickets;
                            }
                            if (status == "ResolvedTickets")
                            {
                                Session["Name"] = "ResolvedTickets";
                                totalticketlist = rootObjects.ResolvedTickets;
                            }
                            if (status == "CloseTickets")
                            {
                                Session["Name"] = "CloseTickets";
                                totalticketlist = rootObjects.CloseTickets;
                            }

                            if (status == "OpenDate")
                            {
                                Session["Name"] = "OpenDate";
                                totalticketlist = rootObjects.OpenDate;
                            }
                            if (status == "OpenLastweek")
                            {
                                Session["Name"] = "OpenLastweek";
                                totalticketlist = rootObjects.OpenLastweek;
                            }
                            if (status == "OpenLastMonth")
                            {
                                Session["Name"] = "OpenLastMonth";
                                totalticketlist = rootObjects.OpenLastMonth;
                            }
                            if (status != null)
                            {
                                Session["status"] = status;
                            }
                            // Apply Search Filter
                            if (!string.IsNullOrEmpty(searchPharse))
                            {
                                var lowerSearch = searchPharse.ToLower();
                                totalticketlist = totalticketlist
                                    .Where(r =>
                                        (r.P_RECID.ToString()?.ToLower().Contains(lowerSearch) ?? false) ||
                                        (r.TC_USERNAME?.ToLower().Contains(lowerSearch) ?? false) ||
                                        (r.TC_COMMENTS?.ToLower().Contains(lowerSearch) ?? false) ||
                                        (r.TC_PRIORITYTYPE?.ToLower().Contains(lowerSearch) ?? false) ||
                                        (r.TC_TICKETTYPE?.ToLower().Contains(lowerSearch) ?? false) ||
                                        (r.TC_STATUS?.ToLower().Contains(lowerSearch) ?? false) ||
                                        (r.TC_TICKETDATES?.ToLower().Contains(lowerSearch) ?? false)
                                    )
                                    .ToList();
                            }

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
            return View(totalticketlist);
        }

        public async Task<ActionResult> StackedBarChart()
        {
            string WEBURLGET = ConfigurationManager.AppSettings["DASHBOARDSTACKEDBARCHART"];
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

        public async Task<ActionResult> SuperAdminCountDashboard()
        {
            string WEBURLGET = ConfigurationManager.AppSettings["DASHBOARDGETSUPERADMIN"];
            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();
            string strparams ="cmprecid=" + Session["CompanyID"];
            string finalurl = WEBURLGET + "?" + strparams;
            DashboardSA dashboardData = null;

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
                            dashboardData = JsonConvert.DeserializeObject<DashboardSA>(jsonString); 
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

        
            return View(dashboardData);
        }
    }
}