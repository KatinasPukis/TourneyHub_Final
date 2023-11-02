using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TourneyHub.Feature.Tournament.Models
{
    public class TournamentTeam
    {
        public string Id { get; set; }
        public string TeamName { get; set; }
        public string TeamDescription { get; set; }
        public string LogoUrl { get; set; }
        public List<TournamentParticipant> TeamMembers { get; set; }
        public string LinkToSelf { get; set; }
    }
}