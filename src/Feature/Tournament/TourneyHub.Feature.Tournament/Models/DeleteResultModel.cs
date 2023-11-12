using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TourneyHub.Feature.Tournament.Models
{
    public class DeleteResultModel
    {
        public string MatchId { get; set; }
        public string FirstParticipantId { get; set; }
        public string SecondParticipantId { get; set; }
    }
}