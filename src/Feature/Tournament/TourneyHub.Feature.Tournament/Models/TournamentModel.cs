using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TourneyHub.Feature.Tournament.Models
{
    public class TournamentModel
    {
        public string TournamentType { get; set; }
        public string TournamentFormat { get; set; }
        public string SportName { get; set; }
        public string TournamentName { get; set; }
        public string LinkToSelf { get; set; }
        public string CreatedByUser { get; set; }
    }

}