using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TourneyHub.Feature.Tournament.Models;
using TourneyHub.Feature.Tournament.Services;

namespace TourneyHub.Feature.Tournament.Controllers
{
    public class TournamentController : Controller
    {
        private readonly TournamentService tournamentService = new TournamentService();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Overview()
        {
            List<TournamentModel> tournamentModels = tournamentService.GetTournaments();
            Item currentUser = tournamentService.GetCurrentUserItem();
            TournamentOverview tournamentOverview = new TournamentOverview
            {
                CurrentUserId = currentUser.ID.ToString(),
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

            if(rendering.Item!=null)
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

        public ActionResult GetTournamentFormData(TournamentFormData tournamentFormData=null)
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

    }
}