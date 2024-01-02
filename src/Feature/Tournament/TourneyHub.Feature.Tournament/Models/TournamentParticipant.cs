using System.Web;

namespace TourneyHub.Feature.Tournament.Models
{
    public class TournamentParticipant
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Info { get; set; }
        public int Age { get; set; }
        public string Image { get; set; }
        public string LinkToSelf { get; set; }
        public HttpPostedFileBase EditImage { get; set; }
    }
}