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
                // Perform any necessary validation on the input data here

                // Assuming 'registrationService.RegisterUser' returns a RegistrationResult

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
                    // Successful registration
                    return Json(new { success = true, redirectUrl = "https://tourneyhub.sc/login" });
                }
                else
                {
                    // Handle the different registration result cases
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
                // Log the exception for debugging and error handling
                // Handle any other exceptions as needed
                return Json(new { success = false, message = "An error occurred while processing your request." });
            }
        }


    }

}