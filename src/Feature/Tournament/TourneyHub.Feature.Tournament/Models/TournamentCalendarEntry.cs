using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TourneyHub.Feature.Tournament.Models
{
    public class TournamentCalendarEntry
    {
        public string EntryId { get; set; }
        public string MatchId { get; set; }
        public DateTime MatchDate { get; set; }
        public string MatchLocation { get; set; }
        public string MatchReferee { get; set; }
        public string FirstParticipantId { get; set; }
        public string SecondParticipantId { get; set; }
    }
}