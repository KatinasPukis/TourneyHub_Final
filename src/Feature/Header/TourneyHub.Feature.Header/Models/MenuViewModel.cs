using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Links;
using System.Collections.Generic;

namespace TourneyHub.Feature.Header.Models
{
    public class MenuViewModel
    {
        public Item MenuItem { get; set; }
        public string ItemLink { get; set; }
        public List<MenuViewModel> Children { get; set; }

        public MenuViewModel(Item menuItem)
        {
            this.MenuItem = menuItem;
            var children = menuItem.Children;
            this.ItemLink = LinkManager.GetItemUrl(menuItem);
            this.Children = new List<MenuViewModel>();
            foreach (Item item in children)
            {
                if (item.TemplateID == HeaderFields.Template.Home.HomePageTemplatesID)
                {
                    this.Children.Add(new MenuViewModel(item));
                }

            }
        }
    }
}