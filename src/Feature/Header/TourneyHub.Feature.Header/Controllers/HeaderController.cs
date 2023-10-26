using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TourneyHub.Feature.Header.Models;

namespace TourneyHub.Feature.Header.Controllers
{
    public class HeaderController : Controller
    {
        public ActionResult Index()
        {
            var homeItem = Sitecore.Context.Database.GetItem(HeaderFields.Template.Home.HomeItemID);
            var headerModel = new HeaderModel
            {
                Page = new MenuViewModel(homeItem),
                LogoID = HeaderFields.Template.Header.Fields.LogoFieldID.ToString()
            };


            return View(headerModel);
        }
        //public ActionResult Logout()
        //{
        //    AuthenticationManager.Logout();
        //    return Redirect("/Beers");
        //}
    }
}