using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TourneyHub.Feature.Tournament.Models
{
    public class TournamentMatches
    {
        public string Id { get; set; }
        public List<TournamentMatch> tournamentMatches { get; set; }
    }
}