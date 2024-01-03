using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TourneyHub.Feature.Tournament.Models
{
    public class TournamentStatistics
    {
        public int TotalParticipants { get; set; }
        public int TotalNumberOfMatches { get; set; }
        public int TotalNumberOfRounds { get; set; }
        public int OverallNumberOfWins { get; set; }
        public int OverallNumberOfLosses { get; set; }
        public double AverageParticipantAge { get; set; }
        public string TournamentType { get; set; }

        public List<TournamentParticipantStatistics> ParticipantStatistics { get; set; }
    }
}