using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PSS_CMS.Fillter
{
    public class ApiKeyAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var apiKey = HttpContext.Current.Session["APIKEY"] as string;

            if (string.IsNullOrEmpty(apiKey))
            {
                filterContext.Controller.TempData["SessionExpired"] = "Your session has expired. Please log in again.";
                // Redirect to Login page
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new { controller = "Login", action = "Index" } // Change as needed
                    )
                );
            }

            base.OnActionExecuting(filterContext);
        }
    }
}