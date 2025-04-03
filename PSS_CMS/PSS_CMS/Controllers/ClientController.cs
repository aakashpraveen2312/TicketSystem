using Newtonsoft.Json;
using PSS_CMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace PSS_CMS.Controllers
{
    public class ClientController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var HomeController = new HomeController();

            var data = await HomeController.Index() as ViewResult;
            // If you absolutely must do this, extract the ViewBag data like this
            if (data != null)
            {
                ViewBag.Menus = data.ViewBag.Menus;
                ViewBag.SubMenusContact = data.ViewBag.SubMenusContact;
                ViewBag.SubMenusService = data.ViewBag.SubMenusService;
                ViewBag.SubMenusClient = data.ViewBag.SubMenusClient;
                ViewBag.SubMenusHRMS = data.ViewBag.SubMenusHRMS;
                ViewBag.HOMECONTACTUS = data.ViewBag.HOMECONTACTUS;

                // Transfer other ViewBag data as needed
            }

            return View(data);
        }

        //// GET: Clients
        //public ActionResult Index()
        //{
        //    List<MainMenu> mainMenuList = new List<MainMenu>();
        //    string connectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;
        //    string query = "SELECT PS_RECID,PS_ACCESSID,PS_PAGENAME, PS_CONTENTTYPE, PS_PARENT,PS_ID , PS_Values, PS_NAME FROM PSSCONTENTACCESS WHERE PS_ID = 'PS001_Root_Menu'"; // Adjust the query if needed

        //    using (SqlConnection sqlCon = new SqlConnection(connectionString))
        //    {
        //        sqlCon.Open();
        //        using (SqlCommand sqlCmd = new SqlCommand(query, sqlCon))
        //        {
        //            SqlDataReader sqlReader = sqlCmd.ExecuteReader();
        //            while (sqlReader.Read())
        //            {
        //                var mainMenu = new MainMenu
        //                {
        //                    PS_RECID = sqlReader["PS_RECID"].ToString(),
        //                    PS_ACCESSID = sqlReader["PS_ACCESSID"].ToString(),
        //                    PS_PAGENAME = sqlReader["PS_PAGENAME"].ToString(),
        //                    PS_CONTENTTYPE = sqlReader["PS_CONTENTTYPE"].ToString(),
        //                    PS_PARENT = sqlReader["PS_PARENT"].ToString(),
        //                    PS_ID = sqlReader["PS_ID"].ToString(),
        //                    PS_NAME = sqlReader["PS_NAME"].ToString(),
        //                    PS_VALUES = new List<string>()
        //                };

        //                string psValuesJson = sqlReader["PS_VALUES"].ToString();

        //                // Check if PS_VALUES is a valid JSON array or not

        //                try
        //                {
        //                    //mainMenu.PS_VALUES = JsonConvert.DeserializeObject<List<string>>(psValuesJson);
        //                    List<string> menuItems = JsonConvert.DeserializeObject<List<string>>(psValuesJson);
        //                    ViewBag.Menus = menuItems;
        //                }
        //                catch (JsonSerializationException ex)
        //                {
        //                    // Log the error (replace this with a proper logging mechanism)
        //                    Console.WriteLine($"Error deserializing PS_VALUES for {mainMenu.PS_NAME}: {ex.Message}");
        //                }

        //                // Add "Home" to the PS_VALUES list as the first item

        //                //mainMenuList.Add(mainMenu);
        //            }
        //        }
        //    }

        //    // Store the list of main menu items in ViewBag
        //    // ViewBag.Menus = menuItems;
        //    SubmenuForContact();

        //    // Return the view with the menu data
        //    return View();
        //}

        //public ActionResult SubmenuForContact()
        //{
        //    //HomeController obj = new HomeController();
        //    //obj.SubmenuForServices();
        //    List<SubMenuContact> SubMenuContactList = new List<SubMenuContact>();
        //    string connectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;
        //    string query = "SELECT PS_RECID,PS_ACCESSID,PS_PAGENAME, PS_CONTENTTYPE, PS_PARENT,PS_ID , PS_Values, PS_NAME FROM PSSCONTENTACCESS WHERE PS_ID = 'PS001_Menu_ContactUs'"; // Adjust the query if needed

        //    using (SqlConnection sqlCon = new SqlConnection(connectionString))
        //    {
        //        sqlCon.Open();
        //        using (SqlCommand sqlCmd = new SqlCommand(query, sqlCon))
        //        {
        //            SqlDataReader sqlReader = sqlCmd.ExecuteReader();
        //            while (sqlReader.Read())
        //            {
        //                var SubMenuContact = new SubMenuContact
        //                {
        //                    PS_RECID = sqlReader["PS_RECID"].ToString(),
        //                    PS_ACCESSID = sqlReader["PS_ACCESSID"].ToString(),
        //                    PS_PAGENAME = sqlReader["PS_PAGENAME"].ToString(),
        //                    PS_CONTENTTYPE = sqlReader["PS_CONTENTTYPE"].ToString(),
        //                    PS_PARENT = sqlReader["PS_PARENT"].ToString(),
        //                    PS_ID = sqlReader["PS_ID"].ToString(),
        //                    PS_NAME = sqlReader["PS_NAME"].ToString(),
        //                    PS_VALUES = new List<string>()
        //                };

        //                string psValuesJson = sqlReader["PS_VALUES"].ToString();

        //                // Check if PS_VALUES is a valid JSON array or not

        //                try
        //                {

        //                    ViewBag.SubMenusContact = (List<string>)JsonConvert.DeserializeObject<List<string>>(psValuesJson);

        //                }
        //                catch (JsonSerializationException ex)
        //                {
        //                    // Log the error (replace this with a proper logging mechanism)
        //                    Console.WriteLine($"Error deserializing PS_VALUES for {SubMenuContact.PS_NAME}: {ex.Message}");
        //                }

        //                // Add "Home" to the PS_VALUES list as the first item

        //                //SubMenuContactList.Add(SubMenuContact);
        //            }
        //        }
        //    }

        //    // Store the list of main menu items in ViewBag
        //    //ViewBag.SubMenusContact = SubMenuContactList;
        //    SubmenuForServices();

        //    // Return the view with the menu data
        //    return View(SubMenuContactList);
        //}


        //public ActionResult SubmenuForServices()
        //{

        //    List<SubMenuServices> SubMenuContactList = new List<SubMenuServices>();
        //    string connectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;
        //    string query = "SELECT PS_RECID,PS_ACCESSID,PS_PAGENAME, PS_CONTENTTYPE, PS_PARENT,PS_ID , PS_Values, PS_NAME FROM PSSCONTENTACCESS WHERE PS_ID = 'PS001_Menu_Services'"; // Adjust the query if needed

        //    using (SqlConnection sqlCon = new SqlConnection(connectionString))
        //    {
        //        sqlCon.Open();
        //        using (SqlCommand sqlCmd = new SqlCommand(query, sqlCon))
        //        {
        //            SqlDataReader sqlReader = sqlCmd.ExecuteReader();
        //            while (sqlReader.Read())
        //            {
        //                var SubMenuContact = new SubMenuContact
        //                {
        //                    PS_RECID = sqlReader["PS_RECID"].ToString(),
        //                    PS_ACCESSID = sqlReader["PS_ACCESSID"].ToString(),
        //                    PS_PAGENAME = sqlReader["PS_PAGENAME"].ToString(),
        //                    PS_CONTENTTYPE = sqlReader["PS_CONTENTTYPE"].ToString(),
        //                    PS_PARENT = sqlReader["PS_PARENT"].ToString(),
        //                    PS_ID = sqlReader["PS_ID"].ToString(),
        //                    PS_NAME = sqlReader["PS_NAME"].ToString(),
        //                    PS_VALUES = new List<string>()
        //                };

        //                string psValuesJson = sqlReader["PS_VALUES"].ToString();

        //                // Check if PS_VALUES is a valid JSON array or not

        //                try
        //                {

        //                    ViewBag.SubMenusServices = (List<string>)JsonConvert.DeserializeObject<List<string>>(psValuesJson);
        //                    List<string> SubMenuService = ViewBag.SubMenusServices as List<string>;
        //                    ViewBag.SubMenusService = SubMenuService;
        //                }
        //                catch (JsonSerializationException ex)
        //                {
        //                    // Log the error (replace this with a proper logging mechanism)
        //                    Console.WriteLine($"Error deserializing PS_VALUES for {SubMenuContact.PS_NAME}: {ex.Message}");
        //                }



        //            }
        //        }
        //    }
        //    SubmenuForClients();
        //    SubmenuForHRMS();
        //    // Return the view with the menu data
        //    return View();
        //}


        ////Clients submenu list
        //public ActionResult SubmenuForClients()
        //{
        //    List<SubMenuClients> SubMenuClientList = new List<SubMenuClients>();
        //    string connectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;
        //    string query = "SELECT PS_RECID,PS_ACCESSID,PS_PAGENAME, PS_CONTENTTYPE, PS_PARENT,PS_ID , PS_Values, PS_NAME FROM PSSCONTENTACCESS WHERE PS_ID = 'PS001_Menu_Client'"; // Adjust the query if needed

        //    using (SqlConnection sqlCon = new SqlConnection(connectionString))
        //    {
        //        sqlCon.Open();
        //        using (SqlCommand sqlCmd = new SqlCommand(query, sqlCon))
        //        {
        //            SqlDataReader sqlReader = sqlCmd.ExecuteReader();
        //            while (sqlReader.Read())
        //            {
        //                var SubMenuClients = new SubMenuClients
        //                {
        //                    PS_RECID = sqlReader["PS_RECID"].ToString(),
        //                    PS_ACCESSID = sqlReader["PS_ACCESSID"].ToString(),
        //                    PS_PAGENAME = sqlReader["PS_PAGENAME"].ToString(),
        //                    PS_CONTENTTYPE = sqlReader["PS_CONTENTTYPE"].ToString(),
        //                    PS_PARENT = sqlReader["PS_PARENT"].ToString(),
        //                    PS_ID = sqlReader["PS_ID"].ToString(),
        //                    PS_NAME = sqlReader["PS_NAME"].ToString(),
        //                    PS_VALUES = new List<string>()
        //                };

        //                string psValuesJson = sqlReader["PS_VALUES"].ToString();

        //                // Check if PS_VALUES is a valid JSON array or not

        //                try
        //                {

        //                    ViewBag.SubMenusClients = (List<string>)JsonConvert.DeserializeObject<List<string>>(psValuesJson);
        //                    List<string> SubMenuClient = ViewBag.SubMenusClients as List<string>;
        //                    ViewBag.SubMenusClient = SubMenuClient;
        //                }
        //                catch (JsonSerializationException ex)
        //                {
        //                    // Log the error (replace this with a proper logging mechanism)
        //                    Console.WriteLine($"Error deserializing PS_VALUES for {SubMenuClients.PS_NAME}: {ex.Message}");
        //                }



        //            }
        //        }
        //    }


        //    // Return the view with the menu data
        //    return View();
        //}

        //public ActionResult SubmenuForHRMS()
        //{
        //    List<SubMenuHRMS> SubMenuHRMSList = new List<SubMenuHRMS>();
        //    string connectionString = ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString;
        //    string query = "SELECT PS_RECID,PS_ACCESSID,PS_PAGENAME, PS_CONTENTTYPE, PS_PARENT,PS_ID , PS_Values, PS_NAME FROM PSSCONTENTACCESS WHERE PS_ID = 'PS001_Services_HRMS'"; // Adjust the query if needed

        //    using (SqlConnection sqlCon = new SqlConnection(connectionString))
        //    {
        //        sqlCon.Open();
        //        using (SqlCommand sqlCmd = new SqlCommand(query, sqlCon))
        //        {
        //            SqlDataReader sqlReader = sqlCmd.ExecuteReader();
        //            while (sqlReader.Read())
        //            {
        //                var SubMenuHRMSs = new SubMenuHRMS
        //                {
        //                    PS_RECID = sqlReader["PS_RECID"].ToString(),
        //                    PS_ACCESSID = sqlReader["PS_ACCESSID"].ToString(),
        //                    PS_PAGENAME = sqlReader["PS_PAGENAME"].ToString(),
        //                    PS_CONTENTTYPE = sqlReader["PS_CONTENTTYPE"].ToString(),
        //                    PS_PARENT = sqlReader["PS_PARENT"].ToString(),
        //                    PS_ID = sqlReader["PS_ID"].ToString(),
        //                    PS_NAME = sqlReader["PS_NAME"].ToString(),
        //                    PS_VALUES = new List<string>()
        //                };

        //                string psValuesJson = sqlReader["PS_VALUES"].ToString();

        //                // Check if PS_VALUES is a valid JSON array or not

        //                try
        //                {

        //                    ViewBag.SubMenusHRMSs = (List<string>)JsonConvert.DeserializeObject<List<string>>(psValuesJson);
        //                    List<string> SubMenuHRMS = ViewBag.SubMenusHRMSs as List<string>;
        //                    ViewBag.SubMenusHRMS = SubMenuHRMS;
        //                }
        //                catch (JsonSerializationException ex)
        //                {
        //                    // Log the error (replace this with a proper logging mechanism)
        //                    Console.WriteLine($"Error deserializing PS_VALUES for {SubMenuHRMSs.PS_NAME}: {ex.Message}");
        //                }



        //            }
        //        }
        //    }


        //    // Return the view with the menu data
        //    return View();
        //}
    }
}