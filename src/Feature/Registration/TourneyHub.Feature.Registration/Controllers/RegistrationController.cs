using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TourneyHub.Feature.Registration.Models;
using TourneyHub.Feature.Registration.Services;

namespace TourneyHub.Feature.Registration.Controllers
{
    public class RegistrationController : Controller
    {
        private RegistrationService registrationService = new RegistrationService();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserData(string username = null, string name = null, string surname = null, string email = null, string password = null)
        {
            try
            {
                UserViewModel userViewModel = new UserViewModel
                {
                    Username = username,
                    Name = name,
                    Surname = surname,
                    Email = email,
                    Password = password
                };

                RegistrationResult registrationResult = registrationService.RegisterUser(userViewModel);

                if (registrationResult == RegistrationResult.Success)
                {
                    return Json(new { success = true, redirectUrl = "https://tourneyhub.sc/login" });
                }
                else
                {
                    if (registrationResult == RegistrationResult.UserExists)
                    {
                        return Json(new { success = false, message = "User already exists with the provided username or email." });
                    }
                    else
                    {
                        return Json(new { success = false, message = "An error occurred during registration." });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while processing your request." });
            }
        }


    }

}