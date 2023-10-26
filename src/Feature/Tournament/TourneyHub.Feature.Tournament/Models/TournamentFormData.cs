using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TourneyHub.Feature.Tournament.Models
{
    public class TournamentFormData
    {
        public string TournamentType { get; set; }
        public int NumberOfParticipants { get; set; }
        public int NumberOfTeams { get; set; }
        public int NumberOfMembersPerTeam { get; set; }
        public string TournamentFormat { get; set; }
        public string SportName { get; set; }
        public string TournamentName { get; set; }
    }

}