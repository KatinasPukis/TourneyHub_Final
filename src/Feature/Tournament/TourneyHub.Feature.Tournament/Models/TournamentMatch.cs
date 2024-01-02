using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TourneyHub.Feature.Tournament.Models
{
    public class TournamentMatch
    {
        public string Id { get; set; }
        public string MatchName { get; set; }
        public TournamentParticipant FirstParticipant { get; set; }
        public TournamentParticipant SecondParticipant { get; set; }
        public List<ParticipantScore> ParticipantScores { get; set; }
        public TournamentTeam FirstTeam { get; set; }
        public TournamentTeam SecondTeam { get; set; }
        public int Score { get; set; }
        public string Winner { get; set; }
        public DateTime DateOfMatch { get; set; }
    }
}