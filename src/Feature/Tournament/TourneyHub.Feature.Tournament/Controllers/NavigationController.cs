using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TourneyHub.Feature.Registration.Models;
using TourneyHub.Feature.Tournament.Fields;
using TourneyHub.Feature.Tournament.Models;
using TourneyHub.Feature.Tournament.Services;

namespace TourneyHub.Feature.Tournament.Controllers
{
    public class NavigationController : Controller
    {
        public ActionResult Index()
        {
            Rendering rendering = RenderingContext.Current.Rendering;

            Item item = rendering.Item;

            List<NavigationModel> navigationModels = new List<NavigationModel>();

            while (item != null && item.TemplateID != TournamentFields.MainTemplateID)
            {
                item = item.Parent;
            }

            if (item != null)
            {
                NavigationModel navigationModel = new NavigationModel
                {
                    Title = item.Name,
                    Url = Sitecore.Links.LinkManager.GetItemUrl(item)
                };

                navigationModels.Add(navigationModel);

                foreach (Item child in item.Children)
                {
                    NavigationModel childNavigation = new NavigationModel
                    {
                        Title = child.Name,
                        Url = Sitecore.Links.LinkManager.GetItemUrl(child)
                    };

                    navigationModels.Add(childNavigation);
                }
            }

            return View(navigationModels);
        }


    }
}