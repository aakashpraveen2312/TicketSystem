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
using PSS_CMS.Models;
namespace PSS_CMS.Controllers
{
    public class DashBoardController : Controller
    {
       

        public async Task<ActionResult> Dashboard()
        {
            string WEBURLGET = ConfigurationManager.AppSettings["DASHBOARDGET"];
            string strparams = "Userid=" + Session["UserID"]+ "&type="+ Session["UserRole"];
            string finalurl = WEBURLGET + "?" + strparams;
            Dashborardchart dashboardData = null;

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
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
            string Authkey = ConfigurationManager.AppSettings["Authkey"];
            string strparams = "Userid=" + Session["UserID"] + "&type=" + Session["UserRole"]; ;
            string finalurl = WEBURLGET + "?" + strparams;
            Dashborardchart wtdMtdData = null;

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", Authkey);
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

            return wtdMtdData;
        }

        public async Task<ActionResult> TimelineChart(int ?recid3)
        {
            int recid = recid3 ?? 0;
            string webUrlGet = ConfigurationManager.AppSettings["TIMELINECHART"];
            string authKey = ConfigurationManager.AppSettings["Authkey"];
            string finalUrl = $"{webUrlGet}?recid={recid}";

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authKey);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response = await client.GetAsync(finalUrl);

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


    }
}