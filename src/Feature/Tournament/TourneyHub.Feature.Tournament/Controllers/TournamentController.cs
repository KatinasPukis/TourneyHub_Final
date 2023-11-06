using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TourneyHub.Feature.Registration.Models;
using TourneyHub.Feature.Tournament.Models;
using TourneyHub.Feature.Tournament.Services;

namespace TourneyHub.Feature.Tournament.Controllers
{
    public class TournamentController : Controller
    {
        private readonly TournamentService tournamentService = new TournamentService();
        private readonly TournamentEditService tournamentEditService = new TournamentEditService();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Overview()
        {
            List<TournamentModel> tournamentModels = tournamentService.GetTournaments();

            UserViewModel userData = tournamentService.GetUserData();

            TournamentOverview tournamentOverview = new TournamentOverview
            {
                UserData = userData,
                tournaments = tournamentModels
            };

            return View(tournamentOverview);
        }
        public ActionResult Detailed()
        {
            Rendering rendering = RenderingContext.Current.Rendering;

            TournamentModel tournamentModel = tournamentService.GetTournament(rendering.Item);

            return View(tournamentModel);
        }

        public ActionResult Participants()
        {
            Rendering rendering = RenderingContext.Current.Rendering;

            TournamentParticipants tournamentParticipants = tournamentService.GetParticipants(rendering.Item);

            return View(tournamentParticipants);
        }

        public ActionResult Participant()
        {
            Rendering rendering = RenderingContext.Current.Rendering;

            TournamentParticipant tournamentParticipant = tournamentService.GetParticipant(rendering.Item);

            return View(tournamentParticipant);

        }

        public ActionResult Team()
        {
            Rendering rendering = RenderingContext.Current.Rendering;

            if (rendering.Item != null)
            {
                TournamentTeam tournamentTeam = tournamentService.GetTournamentTeam(rendering.Item);
                return View(tournamentTeam);
            }

            return View();
        }
        public ActionResult Matches()
        {
            Rendering rendering = RenderingContext.Current.Rendering;

            TournamentMatches tournamentMatches = tournamentService.GetTournamentMatches(rendering.Item);

            return View(tournamentMatches);
        }

        public ActionResult GetTournamentFormData(TournamentFormData tournamentFormData = null)
        {

            tournamentService.CreateTournament(tournamentFormData);

            Item currentUser = tournamentService.GetCurrentUserItem();

            bool success = true;

            string redirectUrl = null;

            if (currentUser != null)
            {
                redirectUrl = Sitecore.Links.LinkManager.GetItemUrl(currentUser);
            }

            // Create a response object
            var response = new
            {
                success = success,
                redirectUrl = redirectUrl
            };

            // Return the response as JSON
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditTournamentParticipant(TournamentParticipant participantData = null)
        {
            try
            {
                tournamentEditService.EditTournamentParticipantItem(participantData);
                return Json(new { success = true, message = "Changes saved" });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "There was a problem saving your changes" });
            }
        }
        public ActionResult EditTournamentTeam(TournamentTeam tournamentTeam = null)
        {
            try
            {
                tournamentEditService.EditTournamentTeamItem(tournamentTeam);
                return Json(new { success = true, message = "Changes saved" });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "There was a problem saving your changes" });
            }
        }
        public ActionResult EditUserProfile(UserViewModel user = null)
        {
            try
            {
                tournamentEditService.EditUser(user);
                return Json(new { success = true, message = "Changes saved" });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "There was a problem saving your changes" });
            }
        }
        public ActionResult DeleteTournament(string tournamentId = null)
        {
            try
            {
                tournamentEditService.DeleteTournament(tournamentId);
                return Json(new { success = true, message = "Tournament Deleted" });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "There was a problem deleting the tournament" });
            }
        }


    }
}