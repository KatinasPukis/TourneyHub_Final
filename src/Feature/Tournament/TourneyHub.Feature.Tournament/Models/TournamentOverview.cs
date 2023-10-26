using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TourneyHub.Feature.Tournament.Models
{
    public class TournamentOverview
    {
        public string CurrentUserId { get; set; }
        public List<TournamentModel> tournaments { get; set; }
    }
}