using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;


namespace TourneyHub.Feature.Footer.Models
{
    public class FooterModel
    {
        public List<Item> Links { get; set; }
        public List<Item> Images { get; set; }
    }
}