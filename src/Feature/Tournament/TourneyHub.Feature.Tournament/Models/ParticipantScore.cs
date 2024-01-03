using System.Collections.Generic;

namespace TourneyHub.Feature.Tournament.Models
{
    public class ParticipantScore
    {
        public string ParticipantId { get; set; }
        public List<int> Scores { get; set; }
        public int WinCount { get; set; }
        public int LossCount { get; set; }
    }
}