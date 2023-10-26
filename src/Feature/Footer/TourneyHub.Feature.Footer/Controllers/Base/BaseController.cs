using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using System.Web.Mvc;

namespace TourneyHub.Feature.Footer.Controllers.Base
{
    public class BaseController : Controller
    {
        public Item ContextItem
        {
            get
            {
                var dataSource = RenderingContext.CurrentOrNull.Rendering.Item;
                return dataSource ?? Sitecore.Context.Item;
            }
        }
    }
}