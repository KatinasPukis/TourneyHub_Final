using System.Linq;
using System.Web.Mvc;
using Sitecore.Data.Items;
using TourneyHub.Feature.Footer.Controllers.Base;
using System.Collections.Generic;
using TourneyHub.Feature.Footer.Models;

namespace TourneyHub.Feature.Footer.Controllers
{
    public class FooterController : BaseController
    {
        public ActionResult Index()
        {
            var footerModel = new FooterModel
            {
                Images = GetImages(),
                Links = GetLinkFields()
            };

            return View(footerModel);
        }

        private List<Item> GetLinkFields()
        {
            List<Item> footerLinksFolders = ContextItem.Axes.GetDescendants().Where(d => d.TemplateID.Equals(FooterFields.Templates.Footer.FooterLinksFolder)).ToList();

            return footerLinksFolders;
        }

        private List<Item> GetImages()
        {
            List<Item> footerImagesFolders = ContextItem.Axes.GetDescendants().Where(m => m.TemplateID.Equals(FooterFields.Templates.Footer.FooterImagesFolder)).ToList();

            return footerImagesFolders;
        }
    }
}