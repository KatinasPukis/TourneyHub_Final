using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TourneyHub.Feature.Tournament.Models
{
    public class ParticipantScore
    {
        public string ParticipantId { get; set; }
        public List<int> Scores { get; set; }
    }
}