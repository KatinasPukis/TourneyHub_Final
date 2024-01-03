using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TourneyHub.Feature.Tournament.Models
{
    public class TournamentParticipantStatistics
    {
        public double AverageScore { get; set; }
        public int TotalScore { get; set; }
        public int NumberOfWins { get; set; }
        public int NumberOfLosses { get; set; }
        public TournamentParticipant Participant { get; set; }
        public TournamentTeam Team { get; set; }
    }
}