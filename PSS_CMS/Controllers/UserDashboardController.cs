using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using PSS_CMS.Models;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PSS_CMS.Controllers
{
    public class UserDashboardController : Controller
    {
        // GET: UserDashboard
        public async Task<ActionResult> UserDashboardCount()
        {
            string WEBURLGET = ConfigurationManager.AppSettings["DASHBOARDGETUSER"];
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
            string WEBURLGET = ConfigurationManager.AppSettings["DASHBOARDMTDandWTDUSER"];
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
            string WEBURLGET = ConfigurationManager.AppSettings["DASHBOARDSTACKEDBARCHARTUSER"];
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

        public async Task<ActionResult> DashboardListTotalTicketUser(string searchPharse, string status)
        {
            DashboardListUser objtotallist = new DashboardListUser();

            string Weburl = ConfigurationManager.AppSettings["DASHBOARDLISTTOTALTICKETSUSER"];

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            List<DashboardListUser> totalticketlist = new List<DashboardListUser>();

            string strparams = "Userid=" + Session["UserRECID"] + "&cmprecid=" + Session["CompanyID"];

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
                            var rootObjects = JsonConvert.DeserializeObject<ApiResponseDashboardListUserResponse>(jsonString);

                            if (status == "TotalTickets")
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
                                        (r.P_NAME.ToString()?.ToLower().Contains(lowerSearch) ?? false) ||
                                        (r.AdminNameDisplay?.ToLower().Contains(lowerSearch) ?? false) ||
                                        (r.TC_SUBJECT?.ToLower().Contains(lowerSearch) ?? false) ||
                                        (r.TC_PRIORITYTYPE?.ToLower().Contains(lowerSearch) ?? false) ||
                                        (r.TC_TICKETTYPE?.ToLower().Contains(lowerSearch) ?? false) ||
                                        (r.TC_STATUS_DISPLAY?.ToLower().Contains(lowerSearch) ?? false) || // 🔁 Use display value
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
    }
}