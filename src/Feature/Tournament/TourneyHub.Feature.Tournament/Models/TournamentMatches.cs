using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TourneyHub.Feature.Tournament.Models
{
    public class TournamentMatches
    {
        public string Id { get; set; }
        public bool IsIndividual { get; set; }
        public List<TournamentStage> TournamentStages { get; set; }
    }
}