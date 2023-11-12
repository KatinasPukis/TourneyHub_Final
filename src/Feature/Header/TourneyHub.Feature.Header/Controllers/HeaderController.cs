using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TourneyHub.Feature.Tournament.Services;
using TourneyHub.Feature.Header.Models;
using Sitecore.Security.Authentication;

namespace TourneyHub.Feature.Header.Controllers
{
    public class HeaderController : Controller
    {
        private readonly TournamentService tournamentService = new TournamentService();

        public ActionResult Index()
        {
            Item homeItem = Sitecore.Context.Database.GetItem(HeaderFields.Template.Home.HomeItemID);
            Item userItem = tournamentService.GetCurrentUserItem();
            string url = userItem != null ? Sitecore.Links.LinkManager.GetItemUrl(userItem) : null;

            var headerModel = new HeaderModel
            {
                Page = homeItem != null ? new MenuViewModel(homeItem) : null,
                LogoID = HeaderFields.Template.Header.Fields.LogoFieldID?.ToString(), // Added null check
                LinkToUserPage = url
            };

            return View(headerModel);
        }
        public ActionResult Logout()
        {
            AuthenticationManager.Logout();
            return Json(new { success = true });
        }
    }
}