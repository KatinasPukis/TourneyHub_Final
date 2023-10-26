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

            Item item = rendering.Item;
            return View();
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