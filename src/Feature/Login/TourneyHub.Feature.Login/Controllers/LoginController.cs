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
            Item currentUser = loginService.GetCurrentUserItem();

            string redirectUrl = null;

            if (currentUser != null)
            {
                redirectUrl = Sitecore.Links.LinkManager.GetItemUrl(currentUser);
            }

            // Return the response as JSON
            try
            {
                // Perform any necessary validation on the input data here

                // Assuming 'loginService.LogInUser' returns a boolean indicating success
                if (loginService.LogInUser(username, password))
                {
                    // Successful login
                    return Json(new { success = true, redirectUrl = redirectUrl });
                }
                else
                {
                    // Authentication failed
                    return Json(new { success = false, message = "Invalid username or password" });
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging and error handling
                // Handle any other exceptions as needed
                return Json(new { success = false, message = "An error occurred while processing your request." });
            }
        }



    }
}