using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TourneyHub.Feature.Registration.Models;
using TourneyHub.Feature.Tournament.Models;
using TourneyHub.Feature.Tournament.Services;

namespace TourneyHub.Feature.Tournament.Controllers
{
    public class TournamentController : Controller
    {
        private readonly TournamentService tournamentService = new TournamentService();
        private readonly TournamentEditService tournamentEditService = new TournamentEditService();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Overview()
        {
            List<TournamentModel> tournamentModels = tournamentService.GetTournaments();
            tournamentModels = tournamentModels.OrderBy(t => t.StartDate).ToList();
            UserViewModel userData = tournamentService.GetUserData();

            TournamentOverview tournamentOverview = new TournamentOverview
            {
                UserData = userData,
                tournaments = tournamentModels
            };

            return View(tournamentOverview);
        }
        public ActionResult Detailed()
        {
            Rendering rendering = RenderingContext.Current.Rendering;

            TournamentModel tournamentModel = tournamentService.GetTournament(rendering.Item);

            return View(tournamentModel);
        }

        public ActionResult Participants()
        {
            Rendering rendering = RenderingContext.Current.Rendering;

            TournamentParticipants tournamentParticipants = tournamentService.GetParticipants(rendering.Item);

            return View(tournamentParticipants);
        }

        public ActionResult Participant()
        {
            Rendering rendering = RenderingContext.Current.Rendering;

            TournamentParticipant tournamentParticipant = tournamentService.GetParticipant(rendering.Item);

            return View(tournamentParticipant);

        }

        public ActionResult Team()
        {
            Rendering rendering = RenderingContext.Current.Rendering;

            if (rendering.Item != null)
            {
                TournamentTeam tournamentTeam = tournamentService.GetTournamentTeam(rendering.Item);
                return View(tournamentTeam);
            }

            return View();
        }
        public ActionResult Calendar()
        {
            Rendering rendering = RenderingContext.Current.Rendering;

            if (rendering.Item != null)
            {
                TournamentCalendar tournamentCalendar = tournamentService.GetTournamentCalendar(rendering.Item);
                return View(tournamentCalendar);
            }

            return View();
        }
        public ActionResult Statistics()
        {
            Rendering rendering = RenderingContext.Current.Rendering;

            if (rendering.Item != null)
            {
                TournamentStatistics tournamentStatistics = tournamentService.GetTournamentStatistics(rendering.Item);
                return View(tournamentStatistics);
            }

            return View();
        }
        public ActionResult Matches()
        {
            Rendering rendering = RenderingContext.Current.Rendering;

            TournamentMatches tournamentMatches = tournamentService.GetTournamentMatches(rendering.Item);

            return View(tournamentMatches);
        }

        public ActionResult GetTournamentFormData(TournamentFormData tournamentFormData = null)
        {
            var tempTournamentIdentifier = HttpContext.Request.Cookies["tempTournamentIdentifier"];
            Item currentUser = tournamentService.GetCurrentUserItem();
            bool success = false;
            string redirectUrl = null;

            if (currentUser == null && !Sitecore.Context.User.IsAuthenticated)
            {
                tournamentService.CreateTemporaryTournament(tournamentFormData, tempTournamentIdentifier.Value);

                success = true;
                redirectUrl = $"/TempTournaments/{tempTournamentIdentifier.Value}";
            }
            else
            {
                tournamentService.CreateTournament(tournamentFormData);
                success = true;
                redirectUrl = currentUser != null ? Sitecore.Links.LinkManager.GetItemUrl(currentUser) : null;
            }

            var response = new
            {
                success = success,
                redirectUrl = redirectUrl
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditTournamentParticipant(TournamentParticipant participantData = null)
        {
            try
            {
                tournamentEditService.EditTournamentParticipantItem(participantData);
                return Json(new { success = true, message = "Changes saved" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "There was a problem saving your changes" });
            }
        }
        public ActionResult EditTournamentTeam(TournamentTeam tournamentTeam = null)
        {
            try
            {
                tournamentEditService.EditTournamentTeamItem(tournamentTeam);
                return Json(new { success = true, message = "Changes saved" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "There was a problem saving your changes" });
            }
        }
        public ActionResult EditUserProfile(UserViewModel user = null)
        {
            try
            {
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.Password) && user.Password == user.RepeatPassword)
                    {
                        tournamentEditService.EditUser(user, updatePassword: true);
                    }
                    else
                    {
                        tournamentEditService.EditUser(user, updatePassword: false);
                    }

                    return Json(new { success = true, message = "Changes saved" });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid user data" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "There was a problem saving your changes" });
            }
        }

        public ActionResult EditTournament(TournamentModel tournament = null)
        {
            try
            {
                tournamentEditService.EditTournament(tournament);
                return Json(new { success = true, message = "Tournament Data Updated" });
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "There was a problem deleting the tournament" });
            }
        }
        [HttpDelete]
        public ActionResult DeleteTournament(string tournamentId = null)
        {
            try
            {
                tournamentEditService.DeleteItem(tournamentId);
                return Json(new { success = true, message = "Tournament Deleted" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "There was a problem deleting the tournament" });
            }
        }
        [HttpDelete]
        public ActionResult DeleteCalendarEntry(string entryId = null)
        {
            try
            {
                tournamentEditService.DeleteItem(entryId);
                return Json(new { success = true, message = "Calendar entry Deleted" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "There was a problem deleting the calendar entry" });
            }
        }

        [HttpGet]
        public JsonResult GetParticipantMatchData(string matchId)
        {
            try
            {
                TournamentMatch match = tournamentService.GetMatchData(matchId);

                List<ParticipantScore> scores = tournamentService.GetScoresForMatch(matchId);

                var responseData = new
                {

                    firstParticipantName = match.FirstParticipant.Name,
                    firstParticipantId = match.FirstParticipant.Id,
                    secondParticipantName = match.SecondParticipant.Name,
                    secondParticipantId = match.SecondParticipant.Id,
                    scores = scores,
                };
                Debug.WriteLine(responseData);
                return Json(responseData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetTeamMatchData(string matchId)
        {
            try
            {

                TournamentMatch match = tournamentService.GetMatchData(matchId);
                List<ParticipantScore> scores = tournamentService.GetScoresForMatch(matchId);

                var responseData = new
                {
                    firstParticipantName = match.FirstTeam.TeamName,
                    firstParticipantId = match.FirstTeam.Id,
                    secondParticipantName = match.SecondTeam.TeamName,
                    secondParticipantId = match.SecondTeam.Id,
                    scores = scores,

                };

                return Json(responseData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult ParticipantMatch(MatchData matchResultModel = null)
        {
            try
            {
                tournamentService.CreateNewMatch(matchResultModel);
                return Json(new { success = true, message = "Matches Updated" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "There was a problem saving your match data" });
            }
        }
        [HttpPost]
        public ActionResult TeamMatch(MatchData matchResultModel = null)
        {
            try
            {
                tournamentService.CreateNewMatch(matchResultModel);
                return Json(new { success = true, message = "Matches Updated" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "There was a problem saving your match data" });
            }
        }
        [HttpPost]
        public ActionResult DeleteResult(DeleteResultModel deleteResultModel = null)
        {
            try
            {
                tournamentEditService.DeleteMatchResults(deleteResultModel);
                return Json(new { success = true, message = "Matches Updated" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "There was a problem saving your match data" });
            }
        }
        [HttpPost]
        public ActionResult AddParticipant(string tournamentId = null)
        {
            try
            {
                tournamentEditService.AddNewParticipantToTournament(tournamentId);
                return Json(new { success = true, message = "Participant Created" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "There was a problem adding a new participant" });
            }
        }
        [HttpPost]
        public ActionResult AddParticipantToTeam(string teamItemId = null)
        {
            try
            {
                tournamentEditService.AddNewParticipantToTeam(teamItemId);
                return Json(new { success = true, message = "Participant Created" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "There was a problem adding a new participant to the team" });
            }
        }
        [HttpPost]
        public ActionResult AddTeam(string tournamentId = null)
        {
            try
            {
                tournamentEditService.AddNewTeamtToTournament(tournamentId);
                return Json(new { success = true, message = "Team Created" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "There was a problem adding a new team" });
            }
        }
        [HttpPost]
        public ActionResult AddSchedule(string matchId, DateTime matchDate, string matchLocation, string matchReferee)
        {
            try
            {
                tournamentService.AddNewSchedule(matchId, matchDate, matchLocation, matchReferee);
                return Json(new { success = true, message = "Schedule Created" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "There was a problem adding a schedule to the tournament" });
            }
        }
        [HttpGet]
        public ActionResult GetSchedule(string matchId, DateTime matchDate, string matchLocation, string matchReferee)
        {
            try
            {
                tournamentService.AddNewSchedule(matchId, matchDate, matchLocation, matchReferee);
                return Json(new { success = true, message = "Schedule Created" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "There was a problem adding a schedule to the tournament" });
            }
        }
        [HttpDelete]
        public ActionResult DeleteParticipant(string participantId = null)
        {
            try
            {
                tournamentEditService.DeleteItem(participantId);
                return Json(new { success = true, message = "Participant Deleted" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "There was a problem deleting the participant" });
            }
        }
        [HttpDelete]
        public ActionResult DeleteTeam(string teamId = null)
        {
            try
            {
                tournamentEditService.DeleteItem(teamId);
                return Json(new { success = true, message = "Team Deleted" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "There was a problem deleting the team" });
            }
        }


    }
}