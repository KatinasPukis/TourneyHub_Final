using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TourneyHub.Feature.Tournament.Models
{
    public class MatchData
    {
        public string MatchId { get; set; }
        public List<MatchScores> Scores { get; set; }
        public string WinnerId { get; set; }
    }
}