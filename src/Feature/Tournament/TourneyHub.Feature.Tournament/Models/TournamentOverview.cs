using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TourneyHub.Feature.Registration.Models;

namespace TourneyHub.Feature.Tournament.Models
{
    public class TournamentOverview 
    {
        public UserViewModel UserData { get; set; }
        public List<TournamentModel> tournaments { get; set; }
    }
}