using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using PSS_CMS.Fillter;
using PSS_CMS.Models;

namespace PSS_CMS.Controllers
{
    [ApiKeyAuthorize]
    public class ManagerCalenderDashboardController : Controller
    {
        public async Task<ActionResult> ManagerCalender()
        {
            string WEBURLGET = "";
            if (Session["UserRole"].ToString() == "Admin")
            {
                 WEBURLGET = ConfigurationManager.AppSettings["CALENDERDASHBOARDADMIN"];
            }
            else
            {
                 WEBURLGET = ConfigurationManager.AppSettings["CALENDERDASHBOARDMANAGER"];
            }
                

            

            string AuthKey = ConfigurationManager.AppSettings["AuthKey"];
            string APIKey = Session["APIKEY"].ToString();

            string strparams = "Userid=" + Session["UserRECID"] + "&type=" + Session["UserRole"] + "&cmprecid=" + Session["CompanyID"];

            string finalurl = WEBURLGET + "?" + strparams;

            Dashborardchart dashboardData = null;

            try
            {
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ServerCertificateCustomValidationCallback += (s, c, ch, e) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("ApiKey", APIKey);
                        client.DefaultRequestHeaders.Add("Authorization", AuthKey);
                        client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));

                        var response = await client.GetAsync(finalurl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();

                            dashboardData = JsonConvert
                                .DeserializeObject<Dashborardchart>(jsonString);


                            // Dashboard counts
                            int total = dashboardData.TotalTickets;
                            int open = dashboardData.OpenTickets;
                            int closed = dashboardData.CloseTickets;
                            int resolved = dashboardData.ResolvedTickets;
                            int assigned = dashboardData.AssignedTickets;
                            int unassigned = dashboardData.UnAssignedTickets;

                            int openDate = dashboardData.OpenDate;
                            int openWeek = dashboardData.OpenLastweek;
                            int openMonth = dashboardData.OpenLastMonth;

                            int overdue = openWeek;


                            // Closed percentage
                            double closedPct = total > 0
                                ? Math.Round((closed / (double)total) * 100, 1)
                                : 0;


                            // ViewBag counts
                            ViewBag.TotalTickets = total;
                            ViewBag.OpenTickets = open;
                            ViewBag.ClosedTickets = closed;
                            ViewBag.ResolvedTickets = resolved;
                            ViewBag.AssignedTickets = assigned;
                            ViewBag.UnassignedCount = unassigned;
                            ViewBag.OverdueTickets = overdue;

                            ViewBag.OpenCurrentDay = openDate;
                            ViewBag.OpenLastWeek = openWeek;
                            ViewBag.OpenLastMonth = openMonth;

                            ViewBag.ClosedPercentage = closedPct;

                            ViewBag.totalAssignedCount = assigned;
                            ViewBag.totalUnAssignedCount = unassigned;

                            ViewBag.totalPickedTicketCount =
                                dashboardData.PickedTicket;

                            ViewBag.totalUnpickedTicketCount =
                                dashboardData.UnpickedTicket;


                            // ===============================
                            // Calendar Data from API
                            // ===============================

                            var calItems = new List<object>();

                            if (dashboardData.CalendarCounts != null)
                            {
                                foreach (var item in dashboardData.CalendarCounts)
                                {
                                    DateTime currentDate = item.Date;


                                    var tickets =
                                        (dashboardData.CalendarTickets ??
                                         new List<CalendarTicket>())

                                        .Where(x =>
                                            x.Date.HasValue &&
                                            x.Date.Value.Date == currentDate.Date)

                                        .Select(x => new
                                        {
                                            id = x.TicketNo,
                                            title = x.Subject,

                                            status = GetStatusName(x.Status),

                                            engineer =
                                                string.IsNullOrEmpty(x.AssignedTo)
                                                ? "Unassigned"
                                                : x.AssignedTo,

                                            priority = x.Priority,
                                            customer = x.CustomerName,
                                            product = x.Product
                                        })

                                        .ToList();


                                    int dayOpen =
                                        tickets.Count(x => x.status == "Open");


                                    int dayAssigned =
                                        tickets.Count(x =>
                                            x.engineer != "Unassigned");


                                    int dayResolved =
                                        tickets.Count(x =>
                                            x.status == "Resolved");



                                    calItems.Add(new
                                    {
                                        date = currentDate
                                            .ToString("yyyy-MM-dd"),

                                        count = item.TicketCount,

                                        open = dayOpen,

                                        assigned = dayAssigned,

                                        resolved = dayResolved,

                                        overdue = 0,

                                        tickets = tickets
                                    });
                                }
                            }


                            ViewBag.CalendarTickets =
                                JsonConvert.SerializeObject(calItems);
                        }
                        else
                        {
                            ModelState.AddModelError(
                                string.Empty,
                                "API Error : " + response.ReasonPhrase);

                            SetEmptyViewBag();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception : " + ex.Message);

                SetEmptyViewBag();
            }


            return View();
        }



        // Convert API status code to text
        private string GetStatusName(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return "Unknown";


            switch (status.Trim())
            {
                case "O":
                    return "Open";

                case "R":
                    return "Resolved";

                case "C":
                    return "Closed";

                case "S":
                    return "Assigned";

                default:
                    return status;
            }
        }



        // Reset ViewBag when API fails
        private void SetEmptyViewBag()
        {
            ViewBag.TotalTickets = 0;
            ViewBag.OpenTickets = 0;
            ViewBag.ClosedTickets = 0;
            ViewBag.ResolvedTickets = 0;

            ViewBag.AssignedTickets = 0;
            ViewBag.UnassignedCount = 0;

            ViewBag.OverdueTickets = 0;

            ViewBag.OpenCurrentDay = 0;
            ViewBag.OpenLastWeek = 0;
            ViewBag.OpenLastMonth = 0;

            ViewBag.ClosedPercentage = 0;

            ViewBag.totalAssignedCount = 0;
            ViewBag.totalUnAssignedCount = 0;

            ViewBag.totalPickedTicketCount = 0;
            ViewBag.totalUnpickedTicketCount = 0;

            ViewBag.CalendarTickets = "[]";

            ViewBag.UpcomingTickets = "[]";

            ViewBag.EngineerWorkload = "[]";
        }
    }
}