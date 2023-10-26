using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TourneyHub.Feature.Tournament.Models
{
    public class TournamentParticipants
    {
        public List<TournamentParticipant> Participants { get; set; }
        public List<TournamentTeam> Teams { get; set; }
    }
}