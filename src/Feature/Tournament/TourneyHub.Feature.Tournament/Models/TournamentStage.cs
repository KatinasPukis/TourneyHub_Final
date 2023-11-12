using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TourneyHub.Feature.Tournament.Models
{
    public class TournamentStage
    {
        public string StageName { get; set; }
        public List<TournamentMatch> TournamentMatches { get; set; }

    }
}