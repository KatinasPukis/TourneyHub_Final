using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TourneyHub.Feature.Login.Models;
using TourneyHub.Feature.Login.Services;

namespace TourneyHub.Feature.Login.Controllers
{
    public class LoginController : Controller
    {
        private readonly LoginService loginService = new LoginService();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserData(string username = null, string password = null)
        {
           
            string redirectUrl = null;
            try
            {
                if (loginService.LogInUser(username, password))
                {
                    Item currentUser = loginService.GetCurrentUserItem();
                    if (currentUser != null)
                    {
                        redirectUrl = Sitecore.Links.LinkManager.GetItemUrl(currentUser);
                    }
                    return Json(new { success = true, redirectUrl = redirectUrl });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid username or password" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while processing your request." });
            }
        }



    }
}