using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Security.Accounts;
using Sitecore.Security.Authentication;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using TourneyHub.Feature.Registration.Fields;
using TourneyHub.Feature.Registration.Models;
using TourneyHub.Feature.Tournament.Fields;
using TourneyHub.Feature.Tournament.Models;
using TourneyHub.Feature.Tournament.Services;

namespace TourneyHub.Feature.Tournament.Services
{
    public class TournamentService
    {
        private readonly Database _masterDb;
        public static string TournamentTypeIndividual;
        public static string TournamentTypeTeam;

        public TournamentService()
        {
            _masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
            TournamentTypeIndividual = "Individual";
            TournamentTypeTeam = "Team";
        }

        public UserViewModel GetUserData()
        {
            Item userItem = GetCurrentUserItem();

            if (userItem == null)
            {
                return new UserViewModel();
            }

            return new UserViewModel
            {
                Id = userItem.ID.ToString(),
                Name = userItem.Fields[UserFields.Fields.NameFieldId]?.Value ?? string.Empty,
                Surname = userItem.Fields[UserFields.Fields.SurnameFieldId]?.Value ?? string.Empty,
                Email = userItem.Fields[UserFields.Fields.EmailFieldId]?.Value ?? string.Empty,
                Username = userItem.Fields[UserFields.Fields.UsernameFieldId]?.Value ?? string.Empty
            };
        }



        public void CreateTournament(TournamentFormData tournamentFormData)
        {
            try
            {
                Item parentTournamentItem = CreateTournamentItem(tournamentFormData);

                if (parentTournamentItem != null)
                {
                    CreateTournamentParticipants(tournamentFormData, parentTournamentItem);
                    CreateTournamentMatches(tournamentFormData, parentTournamentItem, parentTournamentItem);
                    CreateTournamentCalendar(parentTournamentItem);
                }
            }
            catch (Exception ex)
            {
                // Properly handle or log the exception.
            }
        }

        private void CreateTournamentCalendar(Item parentTournamentItem)
        {
            using (new SecurityDisabler())
            {
                TemplateItem template = _masterDb.GetTemplate(TournamentFields.Templates.Calendar.ID);
                Item calendar = parentTournamentItem.Add("Calendar", template);
                using (new EditContext(calendar))
                {
                    calendar.Editing.BeginEdit();
                    calendar.Fields[TournamentFields.Templates.Calendar.Fields.TournamentFieldId].Value = parentTournamentItem.ID.ToString();
                    calendar.Editing.EndEdit();
                }
            }
        }

        public TournamentCalendar GetTournamentCalendar(Item calendarItem)
        {
            TournamentCalendar tournamentCalendar = new TournamentCalendar();

            List<TournamentCalendarEntry> tournamentCalendarEntries = new List<TournamentCalendarEntry>();

            foreach (Item calendarEntry in calendarItem.Children)
            {
                DateField dateField = calendarEntry.Fields[TournamentFields.Templates.CalendarEntry.Fields.MatchDateFieldId];
                Item firstParticipantItem = _masterDb.GetItem(calendarEntry.Fields[TournamentFields.Templates.CalendarEntry.Fields.FirstParticipantFieldId].Value);
                Item secondParticipantItem = _masterDb.GetItem(calendarEntry.Fields[TournamentFields.Templates.CalendarEntry.Fields.SecondParticipantFieldId].Value);
                TournamentParticipant firstParticipant = GetParticipant(firstParticipantItem);
                TournamentParticipant secondParticipant = GetParticipant(secondParticipantItem);
                TournamentCalendarEntry tournamentCalendarEntry = new TournamentCalendarEntry
                {
                    EntryId = calendarEntry.ID.ToString(),
                    MatchId = calendarEntry.Fields[TournamentFields.Templates.CalendarEntry.Fields.TournamentMatchFieldId].Value,
                    MatchDate = dateField.DateTime,
                    MatchLocation = calendarEntry.Fields[TournamentFields.Templates.CalendarEntry.Fields.MatchLocationFieldId].Value,
                    MatchReferee = calendarEntry.Fields[TournamentFields.Templates.CalendarEntry.Fields.MatchRefereeFieldId].Value,
                    FirstParticipantId = firstParticipant.Name,
                    SecondParticipantId = secondParticipant.Name,
                };

                tournamentCalendarEntries.Add(tournamentCalendarEntry);
            }
            tournamentCalendar.CalendarEntries = tournamentCalendarEntries;

            return tournamentCalendar;
        }

        public List<TournamentModel> GetTournaments()
        {
            Item tournamentFolderItem = _masterDb.GetItem(TournamentFields.TournamentParentPageId);

            List<TournamentModel> tournamentsList = tournamentFolderItem.Children
                .Where(tournament => tournament.TemplateID == TournamentFields.MainTemplateID)
                .Select(tournament => new TournamentModel
                {
                    Id = tournament.ID.ToString(),
                    TournamentType = tournament.Fields[TournamentFields.Templates.TournamentInfo.Fields.TournamentTypeFieldId].Value,
                    TournamentFormat = tournament.Fields[TournamentFields.Templates.TournamentInfo.Fields.TournamentFormatFieldId].Value,
                    SportName = tournament.Fields[TournamentFields.Templates.TournamentInfo.Fields.SportNameFieldId].Value,
                    TournamentName = tournament.Fields[TournamentFields.Templates.TournamentInfo.Fields.TournamentNameFieldId].Value,
                    LinkToSelf = Sitecore.Links.LinkManager.GetItemUrl(tournament),
                    CreatedByUser = tournament.Fields[TournamentFields.Templates.TournamentInfo.Fields.CreatedByUserFieldId].Value,
                    LinkToParticipants = GetLinkToParticipants(tournament),
                    LinkToTournamentMatches = GetLinkToTournamentMatches(tournament),
                    DateCreated = DateUtil.IsoDateToDateTime(tournament.Fields["__Created"].Value) // Set DateCreated
                })
                .ToList();

            return tournamentsList;
        }

        public void CreateTemporaryTournament(TournamentFormData tournamentFormData, string uniqueIdentifier)
        {
            try
            {
                Item parentTournamentItem = CreateTempTournamentItem(tournamentFormData, uniqueIdentifier);

                if (parentTournamentItem != null)
                {
                    CreateTournamentParticipants(tournamentFormData, parentTournamentItem);
                    CreateTournamentMatches(tournamentFormData, parentTournamentItem, parentTournamentItem);
                    CreateTournamentCalendar(parentTournamentItem);
                }
            }
            catch (Exception ex)
            {
                // Properly handle or log the exception.
            }
        }

        private Item CreateTempTournamentItem(TournamentFormData tournamentFormData, string uniqueIdentifier)
        {
            Item tournamentItem;


            using (new SecurityDisabler())
            {
                Item parentItem = _masterDb.GetItem(TournamentFields.TempTournamentParentPageId);
                TemplateItem template = _masterDb.GetTemplate(TournamentFields.MainTemplateID);

                string currentUserName = uniqueIdentifier;

                Item tournament = CreateUniqueItem(parentItem, currentUserName, template);

                using (new EditContext(tournament))
                {
                    tournament.Editing.BeginEdit();

                    Field tournamentNameField = tournament.Fields[TournamentFields.Templates.TournamentInfo.Fields.TournamentNameFieldId];
                    Field sportNameField = tournament.Fields[TournamentFields.Templates.TournamentInfo.Fields.SportNameFieldId];
                    Field tournamentFormatField = tournament.Fields[TournamentFields.Templates.TournamentInfo.Fields.TournamentFormatFieldId];
                    Field tournamentTypeField = tournament.Fields[TournamentFields.Templates.TournamentInfo.Fields.TournamentTypeFieldId];
                    Field CreatedByuserField = tournament.Fields[TournamentFields.Templates.TournamentInfo.Fields.CreatedByUserFieldId];

                    if (tournamentNameField != null)
                    {
                        tournamentNameField.Value = tournamentFormData.TournamentName;
                    }

                    if (sportNameField != null)
                    {
                        sportNameField.Value = tournamentFormData.SportName;
                    }

                    if (tournamentFormatField != null)
                    {
                        tournamentFormatField.Value = SetTournamentFormat(tournamentFormData);
                    }
                    if (tournamentTypeField != null)
                    {
                        tournamentTypeField.Value = SetTournamentType(tournamentFormData);
                    }
                    if (CreatedByuserField != null)
                    {
                        CreatedByuserField.Value = uniqueIdentifier;
                    }

                    tournamentItem = tournament;

                    tournament.Editing.EndEdit();
                }
            }

            return tournamentItem;
        }

        private void CreateTournamentMatches(TournamentFormData tournamentFormData, Item parentTournamentItem, Item tournament)
        {
            using (new SecurityDisabler())
            {
                if (parentTournamentItem == null)
                {
                    return;
                }

                var tournamentMatchTemplate = _masterDb.GetTemplate(TournamentFields.Templates.TournamentMatches.ID);
                var tournamentMatchesName = TournamentFields.TournamentItemNames.TournamentMatch;

                var tournamentMatchesItem = parentTournamentItem.Add(tournamentMatchesName, tournamentMatchTemplate);

                var stageTemplateItem = _masterDb.GetTemplate(TournamentFields.Templates.Stage.ID);
                int numParticipants = tournamentFormData.TournamentType == TournamentTypeIndividual ? tournamentFormData.NumberOfParticipants : tournamentFormData.NumberOfTeams;
                int numStages = 0;

                while (numParticipants > 1)
                {
                    numParticipants /= 2;
                    numStages++;

                    var stageItem = tournamentMatchesItem.Add("Stage " + numStages, stageTemplateItem);

                    if (numStages == 1)
                    {
                        TournamentParticipants tournamentParticipants = GetTournamentParticipants(tournament);

                        if (tournamentParticipants != null)
                        {
                            int numberOfMatches = numParticipants;
                            var participants = tournamentParticipants.Participants;
                            var teams = tournamentParticipants.Teams;

                            if (tournamentFormData.TournamentType == TournamentTypeIndividual && participants.Count < 2 * numberOfMatches)
                            {
                                return;
                            }
                            else if (tournamentFormData.TournamentType == TournamentTypeTeam && teams.Count < 2 * numberOfMatches)
                            {

                                return;
                            }

                            for (int matchNumber = 1; matchNumber <= numberOfMatches; matchNumber++)
                            {
                                string matchName = $"{tournamentMatchesName} {matchNumber}";
                                TemplateItem matchTemplate = _masterDb.GetTemplate(TournamentFields.Templates.TournamentMatch.ID);
                                Item matchItem = stageItem.Add(matchName, matchTemplate);

                                using (new EditContext(matchItem))
                                {
                                    matchItem.Editing.BeginEdit();
                                    if (tournamentFormData.TournamentType == TournamentTypeIndividual)
                                    {
                                        matchItem.Fields[TournamentFields.Templates.TournamentMatch.Fields.FirstParticipantFieldId].Value = participants[2 * matchNumber - 2].Id;
                                        matchItem.Fields[TournamentFields.Templates.TournamentMatch.Fields.SecondParticipantFieldId].Value = participants[2 * matchNumber - 1].Id;
                                    }
                                    else if (tournamentFormData.TournamentType == TournamentTypeTeam)
                                    {
                                        matchItem.Fields[TournamentFields.Templates.TournamentMatch.Fields.FirstParticipantFieldId].Value = teams[2 * matchNumber - 2].Id;
                                        matchItem.Fields[TournamentFields.Templates.TournamentMatch.Fields.SecondParticipantFieldId].Value = teams[2 * matchNumber - 1].Id;
                                    }
                                    matchItem.Editing.EndEdit();
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int matchNumber = 1; matchNumber <= numParticipants; matchNumber++)
                        {
                            string matchName = $"{tournamentMatchesName} {matchNumber}";
                            TemplateItem matchTemplate = _masterDb.GetTemplate(TournamentFields.Templates.TournamentMatch.ID);
                            stageItem.Add(matchName, matchTemplate);
                        }
                    }

                    stageItem.Editing.EndEdit();
                }

                tournamentMatchesItem.Editing.EndEdit();
            }
        }

        public List<ParticipantScore> GetScoresForMatch(string matchId)
        {
            Item matchItem = _masterDb.GetItem(matchId);

            if (matchItem == null)
            {
                return null;
            }

            List<ParticipantScore> participantScores = new List<ParticipantScore>();

            foreach (Item scoreItem in matchItem.Children)
            {
                string participantId = scoreItem.Fields[TournamentFields.Templates.Score.Fields.ParticipantFieldId].Value;

                int score = Int32.Parse(scoreItem.Fields[TournamentFields.Templates.Score.Fields.ScoreFieldId].Value);

                ParticipantScore participantScore = participantScores.FirstOrDefault(ps => ps.ParticipantId == participantId);

                if (participantScore == null)
                {
                    participantScore = new ParticipantScore
                    {
                        ParticipantId = participantId,
                        Scores = new List<int>()
                    };

                    participantScores.Add(participantScore);
                }

                participantScore.Scores.Add(score);
            }

            return participantScores;
        }

        private TournamentParticipants GetTournamentParticipants(Item tournament)
        {
            if (tournament == null)
            {
                return new TournamentParticipants();
            }

            Item participantsItem = tournament.Children.FirstOrDefault(item =>
                item.TemplateID == TournamentFields.Templates.Participants.ID);

            if (participantsItem == null)
            {
                return new TournamentParticipants();
            }

            List<TournamentParticipant> participants = new List<TournamentParticipant>();

            List<TournamentTeam> tournamentTeams = new List<TournamentTeam>();

            foreach (Item item in participantsItem.Children)
            {
                if (item.TemplateID == TournamentFields.Templates.Participant.ID)
                {
                    participants.Add(new TournamentParticipant { Id = item.ID.ToString() });
                }
                else if (item.TemplateID == TournamentFields.Templates.TournamentTeam.ID)
                {
                    tournamentTeams.Add(new TournamentTeam { Id = item.ID.ToString() });
                }
            }

            return new TournamentParticipants
            {
                TournamentId = tournament.ID.ToString(),
                Participants = participants,
                Teams = tournamentTeams
            };
        }

        public void AddNewSchedule(string matchId, DateTime matchDate, string matchLocation, string matchReferee)
        {

            Item tournamentMatchItem = _masterDb.GetItem(matchId);
            Item tournamentItem = GetTournamentItem(tournamentMatchItem);

            Item calendarItem = tournamentItem.Children
            .FirstOrDefault(item => item.TemplateID == TournamentFields.Templates.Calendar.ID);

            string firstParticipantId = tournamentMatchItem.Fields[TournamentFields.Templates.TournamentMatch.Fields.FirstParticipantFieldId].Value;

            string secondParticipantId = tournamentMatchItem.Fields[TournamentFields.Templates.TournamentMatch.Fields.SecondParticipantFieldId].Value;

            TemplateItem calendarEntryTemplate = _masterDb.GetTemplate(TournamentFields.Templates.CalendarEntry.ID);

            string calendarEntryName = GetUniqueItemName("Calendar Entry", calendarItem);

            Item calendarEntryItem = calendarItem.Add(calendarEntryName, calendarEntryTemplate);

            using (new SecurityDisabler())
            {
                using (new EditContext(calendarEntryItem))
                {
                    calendarEntryItem.Editing.BeginEdit();
                    calendarEntryItem.Fields[TournamentFields.Templates.CalendarEntry.Fields.TournamentMatchFieldId].Value = matchId;
                    SetDateTimeFieldValue(calendarEntryItem, TournamentFields.Templates.CalendarEntry.Fields.MatchDateFieldId, matchDate);
                    calendarEntryItem.Fields[TournamentFields.Templates.CalendarEntry.Fields.MatchLocationFieldId].Value = matchLocation;
                    calendarEntryItem.Fields[TournamentFields.Templates.CalendarEntry.Fields.MatchRefereeFieldId].Value = matchReferee;
                    calendarEntryItem.Fields[TournamentFields.Templates.CalendarEntry.Fields.FirstParticipantFieldId].Value = firstParticipantId;
                    calendarEntryItem.Fields[TournamentFields.Templates.CalendarEntry.Fields.SecondParticipantFieldId].Value = secondParticipantId;
                    calendarEntryItem.Editing.EndEdit();

                }
            }


        }
        //temp
        private void SetDateTimeFieldValue(Item item, ID fieldId, DateTime? dateTimeValue)
        {
            if (item == null || dateTimeValue == null)
            {
                return;
            }

            DateField dateTimeField = item.Fields[fieldId];

            if (dateTimeField != null)
            {
                dateTimeField.Value = DateUtil.ToIsoDate((DateTime)dateTimeValue);
            }
        }
        //temp
        public string GetUniqueItemName(string baseName, Item parentItem)
        {
            var existingNames = parentItem.Children.Select(child => child.Name).ToList();

            int counter = 0;

            string uniqueName = baseName;

            while (existingNames.Contains(uniqueName))
            {
                counter++;
                uniqueName = $"{baseName}{counter}";
            }

            return uniqueName;
        }

        public void CreateNewMatch(MatchData matchResultModel)
        {
            Item matchItem = _masterDb.GetItem(matchResultModel.MatchId);
            CreateScoreItems(matchResultModel, matchItem);
            SetAccesibility(matchItem);
            SendUserToNewMatch(matchResultModel, matchItem);
        }

        private void SetAccesibility(Item matchItem)
        {
            Item stageItem = matchItem.Parent;

            Item matchesItem = stageItem.Parent;

            bool isAccessible = false;

            foreach (Item stage in matchesItem.Children)
            {
                using (new SecurityDisabler())
                {
                    using (new EditContext(stage))
                    {
                        stage.Fields[TournamentFields.Templates.Stage.Fields.IsAccesibleFieldId].Value = "0";

                    }
                }
            }

            foreach (Item stage in matchesItem.Children)
            {
                if (stageItem.ID == stage.ID)
                {
                    isAccessible = false;
                    continue;
                }
                foreach (Item match in stage.Children)
                {

                    if (string.IsNullOrEmpty(match.Fields[TournamentFields.Templates.TournamentMatch.Fields.FirstParticipantFieldId].Value) ||
                string.IsNullOrEmpty(match.Fields[TournamentFields.Templates.TournamentMatch.Fields.SecondParticipantFieldId].Value))
                    {
                        isAccessible = true;
                        break;
                    }

                }
                if (isAccessible)
                {
                    using (new SecurityDisabler())
                    {
                        using (new EditContext(stage))
                        {
                            stage.Fields[TournamentFields.Templates.Stage.Fields.IsAccesibleFieldId].Value = "1";
                            break;

                        }
                    }


                }
            }
        }

        private void SendUserToNewMatch(MatchData matchResultModel, Item matchItem)
        {
            Item parentStageItem = matchItem.Parent;
            Item tournamentMatches = parentStageItem.Parent;
            foreach (Item stage in tournamentMatches.Children)
            {
                if (stage.ID == parentStageItem.ID)
                {
                    continue;
                }
                if (stage.Fields[TournamentFields.Templates.Stage.Fields.IsAccesibleFieldId].Value == "1")
                {
                    foreach (Item match in stage.Children)
                    {
                        if (string.IsNullOrEmpty(match.Fields[TournamentFields.Templates.TournamentMatch.Fields.FirstParticipantFieldId].Value) ||
               string.IsNullOrEmpty(match.Fields[TournamentFields.Templates.TournamentMatch.Fields.SecondParticipantFieldId].Value))
                        {
                            if (match.Fields[TournamentFields.Templates.TournamentMatch.Fields.FirstParticipantFieldId].Value != matchResultModel.WinnerId &&
                                match.Fields[TournamentFields.Templates.TournamentMatch.Fields.SecondParticipantFieldId].Value != matchResultModel.WinnerId)
                            {
                                if (CheckIfCanBeAddedToMatch(stage, matchResultModel.WinnerId))
                                {
                                    using (new SecurityDisabler())
                                    {
                                        using (new EditContext(match))
                                        {
                                            match.Editing.BeginEdit();
                                            if (string.IsNullOrEmpty(match.Fields[TournamentFields.Templates.TournamentMatch.Fields.FirstParticipantFieldId].Value))
                                            {
                                                match.Fields[TournamentFields.Templates.TournamentMatch.Fields.FirstParticipantFieldId].Value = matchResultModel.WinnerId;
                                                break;
                                            }
                                            if (string.IsNullOrEmpty(match.Fields[TournamentFields.Templates.TournamentMatch.Fields.SecondParticipantFieldId].Value))
                                            {
                                                match.Fields[TournamentFields.Templates.TournamentMatch.Fields.SecondParticipantFieldId].Value = matchResultModel.WinnerId;
                                                break;
                                            }
                                            match.Editing.EndEdit();


                                        }
                                    }
                                }

                            }



                        }
                    }
                }

            }

        }
        private bool CheckIfCanBeAddedToMatch(Item stage, string winnerId)
        {
            bool IsAllowed = true;
            foreach (Item match in stage.Children)
            {
                if (match.Fields[TournamentFields.Templates.TournamentMatch.Fields.FirstParticipantFieldId].Value == winnerId)
                {
                    IsAllowed = false;
                }
                if (match.Fields[TournamentFields.Templates.TournamentMatch.Fields.SecondParticipantFieldId].Value == winnerId)
                {
                    IsAllowed = false;
                }

            }
            return IsAllowed;

        }

        private string CreateScoreItems(MatchData matchResultModel, Item matchItem)
        {
            matchItem.Editing.BeginEdit();
            matchItem.Fields[TournamentFields.Templates.TournamentMatch.Fields.WinnerFieldId].Value = matchResultModel.WinnerId;

            using (new SecurityDisabler())
            {
                using (new EditContext(matchItem))
                {
                    TemplateItem template = _masterDb.GetTemplate(TournamentFields.Templates.Score.ID);
                    int temp = 0;

                    try
                    {
                        foreach (MatchScores item in matchResultModel.Scores)
                        {
                            foreach (var scores in item.Scores)
                            {
                                temp++;
                                Item participantItem = _masterDb.GetItem(item.ParticipantId);
                                string participantName = GetParticipant(participantItem).Name;
                                string matchName = "Score " + participantName + " " + temp;

                                // Check if the item already exists
                                Item existingScoreItem = matchItem.Children[matchName];

                                if (existingScoreItem != null)
                                {
                                    // Item already exists, update its fields
                                    existingScoreItem.Editing.BeginEdit();
                                    existingScoreItem.Fields[TournamentFields.Templates.Score.Fields.ParticipantFieldId].Value = participantItem.ID.ToString();
                                    existingScoreItem.Fields[TournamentFields.Templates.Score.Fields.ScoreFieldId].Value = scores.ToString();
                                    existingScoreItem.Editing.EndEdit();
                                }
                                else
                                {
                                    // Item does not exist, create a new one
                                    Item scoreItem = matchItem.Add(matchName, template);
                                    scoreItem.Editing.BeginEdit();
                                    scoreItem.Fields[TournamentFields.Templates.Score.Fields.ParticipantFieldId].Value = participantItem.ID.ToString();
                                    scoreItem.Fields[TournamentFields.Templates.Score.Fields.ScoreFieldId].Value = scores.ToString();
                                    scoreItem.Editing.EndEdit();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            matchItem.Editing.EndEdit();
            return null;
        }

        private void CreateTournamentParticipants(TournamentFormData tournamentFormData, Item parentTournamentItem)
        {
            using (new SecurityDisabler())
            {
                if (parentTournamentItem != null)
                {
                    TemplateItem template = _masterDb.GetTemplate(TournamentFields.Templates.Participants.ID);

                    string participantsItemName = TournamentFields.TournamentItemNames.Participants;

                    Item participantsItem = parentTournamentItem.Add(participantsItemName, template);

                    using (new EditContext(participantsItem))
                    {
                        participantsItem.Editing.BeginEdit();

                        if (tournamentFormData.TournamentType == TournamentTypeIndividual)
                        {
                            CreateTournamentParticipant(participantsItem, tournamentFormData.NumberOfParticipants);
                        }
                        else if (tournamentFormData.TournamentType == TournamentTypeTeam)
                        {
                            CreateTournamentTeams(participantsItem, tournamentFormData);
                        }

                        participantsItem.Editing.EndEdit();
                    }
                }
            }
        }

        private void CreateTournamentTeams(Item participantsItem, TournamentFormData tournamentFormData)
        {
            TemplateItem teamTemplate = _masterDb.GetTemplate(TournamentFields.Templates.TournamentTeam.ID);

            using (new SecurityDisabler())
            {
                participantsItem.Editing.BeginEdit();

                for (int teamIndex = 0; teamIndex < tournamentFormData.NumberOfTeams; teamIndex++)
                {
                    string teamName = $"Team {teamIndex}";
                    Item teamItem = CreateUniqueItem(participantsItem, teamName, teamTemplate);

                    teamItem.Editing.BeginEdit();
                    teamItem.Fields[TournamentFields.Templates.TournamentTeam.Fields.TeamNameFieldId].Value = teamName;
                    teamItem.Editing.EndEdit();

                    CreateTournamentParticipant(teamItem, tournamentFormData.NumberOfMembersPerTeam);
                }

                participantsItem.Editing.EndEdit();
            }
        }

        private void CreateTournamentParticipant(Item participantsItem, int numberOfParticipants)
        {
            TemplateItem participantTemplate = _masterDb.GetTemplate(TournamentFields.Templates.Participant.ID);

            using (new SecurityDisabler())
            {
                participantsItem.Editing.BeginEdit();

                for (int i = 0; i < numberOfParticipants; i++)
                {
                    string participantName = $"Participant {i}";
                    Item participantItem = CreateUniqueItem(participantsItem, participantName, participantTemplate);

                    participantItem.Editing.BeginEdit();

                    // Set participant information fields as needed.
                    participantItem.Fields[TournamentFields.Templates.Participant.Fields.NameFieldId].Value = participantName;

                    participantItem.Editing.EndEdit();
                }

                participantsItem.Editing.EndEdit();
            }
        }

        private Item CreateUniqueItem(Item parentItem, string itemName, TemplateItem template)
        {
            // Ensure item name is unique.
            string uniqueItemName = itemName;
            int count = 1;
            while (parentItem.Children[uniqueItemName] != null)
            {
                uniqueItemName = $"{itemName} ({count++})";
            }

            return parentItem.Add(uniqueItemName, template);
        }

        private Item CreateTournamentItem(TournamentFormData tournamentFormData)
        {
            Item tournamentItem;


            using (new SecurityDisabler())
            {
                Item parentItem = _masterDb.GetItem(TournamentFields.TournamentParentPageId);
                TemplateItem template = _masterDb.GetTemplate(TournamentFields.MainTemplateID);

                string currentUserName = tournamentFormData.TournamentName;

                Item tournament = CreateUniqueItem(parentItem, currentUserName, template);

                using (new EditContext(tournament))
                {
                    tournament.Editing.BeginEdit();

                    Field tournamentNameField = tournament.Fields[TournamentFields.Templates.TournamentInfo.Fields.TournamentNameFieldId];
                    Field sportNameField = tournament.Fields[TournamentFields.Templates.TournamentInfo.Fields.SportNameFieldId];
                    Field tournamentFormatField = tournament.Fields[TournamentFields.Templates.TournamentInfo.Fields.TournamentFormatFieldId];
                    Field tournamentTypeField = tournament.Fields[TournamentFields.Templates.TournamentInfo.Fields.TournamentTypeFieldId];
                    Field CreatedByuserField = tournament.Fields[TournamentFields.Templates.TournamentInfo.Fields.CreatedByUserFieldId];

                    if (tournamentNameField != null)
                    {
                        tournamentNameField.Value = tournamentFormData.TournamentName;
                    }

                    if (sportNameField != null)
                    {
                        sportNameField.Value = tournamentFormData.SportName;
                    }

                    if (tournamentFormatField != null)
                    {
                        tournamentFormatField.Value = SetTournamentFormat(tournamentFormData);
                    }
                    if (tournamentTypeField != null)
                    {
                        tournamentTypeField.Value = SetTournamentType(tournamentFormData);
                    }
                    if (CreatedByuserField != null)
                    {
                        CreatedByuserField.Value = GetCurrentUserItem().ID.ToString();
                    }

                    tournamentItem = tournament;

                    tournament.Editing.EndEdit();
                }
            }

            return tournamentItem;
        }

        public Item GetCurrentUserItem()
        {
            if (Sitecore.Context.User.IsAuthenticated)
            {
                User user = AuthenticationManager.GetActiveUser();

                if (user != null)
                {
                    Item userFolder = _masterDb.GetItem(UserFields.UsersFolderId);

                    string username = user.Name;

                    int index = username.IndexOf("\\");

                    if (index != -1)
                    {
                        username = username.Substring(index + 1);
                    }

                    if (userFolder != null)
                    {
                        Item currentUserItem = userFolder.Children
                            .FirstOrDefault(userItem => username == userItem.Fields[UserFields.Fields.UsernameFieldId].Value);

                        return currentUserItem;
                    }
                }
            }

            return null;
        }


        private string SetTournamentType(TournamentFormData tournamentFormData)
        {
            List<TournamentType> tournamentTypes = GetTournamentTypes();

            string tournamentTypeId = string.Empty;

            if (tournamentTypes != null)
            {
                foreach (TournamentType tournamentType in tournamentTypes)
                {
                    if (tournamentFormData.TournamentType == tournamentType.Type)
                    {
                        tournamentTypeId = tournamentType.Id;
                    }
                }
            }
            return tournamentTypeId;

        }

        private List<TournamentType> GetTournamentTypes()
        {
            List<TournamentType> tournamentTypes = new List<TournamentType>();

            Item tournamentTypeFolder = _masterDb.GetItem(TournamentFields.TournamentData.TournamentDataFolder.TournamentTypeFolderId);

            foreach (Item item in tournamentTypeFolder.Children)
            {
                if (item != null)
                {
                    TournamentType tournamentType = new TournamentType
                    {
                        Id = item.ID.ToString(),
                        Type = item.Fields[TournamentFields.TournamentData.TournamentType.TournamentTypeFieldID].Value
                    };
                    tournamentTypes.Add(tournamentType);
                }


            }
            return tournamentTypes;
        }

        private string SetTournamentFormat(TournamentFormData tournamentFormData)
        {
            List<TournamentFormat> tournamentFormats = GetTournamentFormats();

            string tournamentFormatId = string.Empty;

            if (tournamentFormats != null)
            {
                foreach (TournamentFormat tournamentFormat in tournamentFormats)
                {
                    if (tournamentFormData.TournamentFormat == tournamentFormat.Format)
                    {
                        tournamentFormatId = tournamentFormat.Id;
                    }
                }
            }
            return tournamentFormatId;

        }

        private List<TournamentFormat> GetTournamentFormats()
        {
            List<TournamentFormat> tournamentFormats = new List<TournamentFormat>();

            Item tournamentFormatFolder = _masterDb.GetItem(TournamentFields.TournamentData.TournamentDataFolder.TournamentFormatFolderId);

            foreach (Item item in tournamentFormatFolder.Children)
            {
                if (item != null)
                {
                    TournamentFormat tournamentFormat = new TournamentFormat
                    {
                        Id = item.ID.ToString(),
                        Format = item.Fields[TournamentFields.TournamentData.TournamentFormat.TournamentFormatFieldID].Value
                    };
                    tournamentFormats.Add(tournamentFormat);
                }


            }
            return tournamentFormats;
        }
        public TournamentModel GetTournament(Item tournamentItem)
        {
            if (tournamentItem == null || tournamentItem.TemplateID != TournamentFields.MainTemplateID)
            {
                return null;
            }

            string linkToParticipants = GetLinkToParticipants(tournamentItem);
            string linkToTournamentMatches = GetLinkToTournamentMatches(tournamentItem);

            return new TournamentModel
            {
                Id = tournamentItem.ID.ToString(),
                TournamentType = GetTournamentDroplinkValue(tournamentItem.Fields[TournamentFields.Templates.TournamentInfo.Fields.TournamentTypeFieldId].Value),
                TournamentFormat = GetTournamentDroplinkValue(tournamentItem.Fields[TournamentFields.Templates.TournamentInfo.Fields.TournamentFormatFieldId].Value),
                SportName = tournamentItem.Fields[TournamentFields.Templates.TournamentInfo.Fields.SportNameFieldId].Value,
                TournamentName = tournamentItem.Fields[TournamentFields.Templates.TournamentInfo.Fields.TournamentNameFieldId].Value,
                LinkToSelf = Sitecore.Links.LinkManager.GetItemUrl(tournamentItem),
                CreatedByUser = tournamentItem.Fields[TournamentFields.Templates.TournamentInfo.Fields.CreatedByUserFieldId].Value,
                LinkToParticipants = linkToParticipants,
                LinkToTournamentMatches = linkToTournamentMatches,
                DateCreated = DateUtil.IsoDateToDateTime(tournamentItem.Fields["__Created"].Value),
                StartDate = GetDateFieldValue(tournamentItem, TournamentFields.Templates.TournamentInfo.Fields.TournamentStartDateFieldId),
                EndDate = GetDateFieldValue(tournamentItem, TournamentFields.Templates.TournamentInfo.Fields.TournamentEndDateFieldId)
            };
        }
        private DateTime GetDateFieldValue(Item item, ID fieldId)
        {
            if (item == null)
            {
                return DateTime.MinValue;
            }

            DateField dateField = item.Fields[fieldId];

            if (dateField != null && !string.IsNullOrEmpty(dateField.Value))
            {
                DateTime? parsedDateTime = DateUtil.IsoDateToDateTime(dateField.Value);

                if (parsedDateTime != null && parsedDateTime != DateTime.MinValue)
                {
                    return (DateTime)parsedDateTime;
                }
            }

            return DateTime.MinValue;
        }



        private string GetLinkToTournamentMatches(Item tournamentItem)
        {
            Item tournamentMatchesItem = tournamentItem.Children.FirstOrDefault(item =>
                            item.TemplateID.Equals(TournamentFields.Templates.TournamentMatches.ID));

            string linkToTournamentMatches = tournamentMatchesItem != null
                ? Sitecore.Links.LinkManager.GetItemUrl(tournamentMatchesItem)
                : string.Empty;

            return linkToTournamentMatches;
        }

        private string GetLinkToParticipants(Item tournamentItem)
        {

            Item participantsItem = tournamentItem.Children.FirstOrDefault(item =>
                item.TemplateID.Equals(TournamentFields.Templates.Participants.ID));

            string linkToParticipants = participantsItem != null
                ? Sitecore.Links.LinkManager.GetItemUrl(participantsItem)
                : string.Empty;

            return linkToParticipants;
        }

        private string GetTournamentDroplinkValue(string fieldValue)
        {
            string droplinkValue = string.Empty;

            if (!string.IsNullOrEmpty(fieldValue))
            {
                Item droplinkItem = _masterDb.GetItem(fieldValue);

                if (droplinkItem.TemplateID == TournamentFields.TournamentData.TournamentType.ID)
                {
                    droplinkValue = droplinkItem.Fields[TournamentFields.TournamentData.TournamentType.TournamentTypeFieldID].Value;
                }
                if (droplinkItem.TemplateID == TournamentFields.TournamentData.TournamentFormat.ID)
                {
                    droplinkValue = droplinkItem.Fields[TournamentFields.TournamentData.TournamentFormat.TournamentFormatFieldID].Value;
                }
            }

            return droplinkValue;
        }

        //Get participants
        public TournamentParticipant GetParticipant(Item participantItem)
        {
            if (participantItem == null)
            {
                return null;
            }

            var fields = participantItem.Fields;
            string imageURL = GetImageURLFromItem(participantItem, TournamentFields.Templates.Participant.Fields.ImageFieldId);
            return new TournamentParticipant
            {
                Id = participantItem.ID.ToString(),
                Name = fields[TournamentFields.Templates.Participant.Fields.NameFieldId]?.Value,
                Surname = fields[TournamentFields.Templates.Participant.Fields.SurnameFieldId]?.Value,
                Info = fields[TournamentFields.Templates.Participant.Fields.InformationFieldId]?.Value,
                Age = Int32.TryParse(fields[TournamentFields.Templates.Participant.Fields.AgeFieldId]?.Value, out var age) ? age : 0,
                Image = imageURL
            };
        }
        //Helper
        public string GetImageURLFromItem(Item item, ID imageFieldId)
        {
            if (item == null)
            {
                return null;
            }

            ImageField imageField = (ImageField)item.Fields[imageFieldId];

            if (imageField?.MediaItem != null)
            {
                return Sitecore.Resources.Media.MediaManager.GetMediaUrl(imageField.MediaItem);
            }

            return null;
        }
        public TournamentParticipants GetParticipants(Item participantsItem)
        {
            TournamentParticipants tournamentParticipants = new TournamentParticipants();

            tournamentParticipants.TournamentId = participantsItem.ParentID.ToString();

            List<TournamentParticipant> participants = new List<TournamentParticipant>();

            List<TournamentTeam> teams = new List<TournamentTeam>();


            foreach (Item item in participantsItem.Children)
            {
                if (item.TemplateID == TournamentFields.Templates.Participant.ID)
                {
                    string imageURL = GetImageURLFromItem(item, TournamentFields.Templates.Participant.Fields.ImageFieldId);

                    TournamentParticipant tournamentParticipant = new TournamentParticipant
                    {
                        Id = item.ID.ToString(),
                        Name = item.Fields[TournamentFields.Templates.Participant.Fields.NameFieldId]?.Value,
                        Surname = item.Fields[TournamentFields.Templates.Participant.Fields.SurnameFieldId]?.Value,
                        Info = item.Fields[TournamentFields.Templates.Participant.Fields.InformationFieldId]?.Value,
                        Age = Int32.TryParse(item.Fields[TournamentFields.Templates.Participant.Fields.AgeFieldId]?.Value, out var age) ? age : 0,
                        Image = imageURL,
                        LinkToSelf = GetLinkToParticipant(item)
                    };
                    participants.Add(tournamentParticipant);
                }
                else
                {
                    TournamentTeam tournamentTeam = new TournamentTeam
                    {
                        Id = item.ID.ToString(),
                        TeamDescription = item.Fields[TournamentFields.Templates.TournamentTeam.Fields.TeamDescriptionFieldId]?.Value,
                        TeamName = item.Fields[TournamentFields.Templates.TournamentTeam.Fields.TeamNameFieldId]?.Value,
                        LogoUrl = GetImageURLFromItem(item, TournamentFields.Templates.TournamentTeam.Fields.TeamLogoFieldId),
                        TeamMembers = GetTeamMembers(item),
                        LinkToSelf = GetLinkToTeam(item),
                    };
                    teams.Add(tournamentTeam);
                }
            }

            if (participants != null)
            {
                tournamentParticipants.Participants = participants;
            }
            if (teams != null)
            {
                tournamentParticipants.Teams = teams;
            }

            return tournamentParticipants;
        }
        private List<TournamentParticipant> GetTeamMembers(Item teamItem)
        {
            List<TournamentParticipant> members = new List<TournamentParticipant>();

            foreach (Item participant in teamItem.Children)
            {

                TournamentParticipant tournamentParticipant = new TournamentParticipant
                {
                    Id = participant.ID.ToString(),
                    Name = participant.Fields[TournamentFields.Templates.Participant.Fields.NameFieldId]?.Value,
                    Surname = participant.Fields[TournamentFields.Templates.Participant.Fields.SurnameFieldId]?.Value,
                    Info = participant.Fields[TournamentFields.Templates.Participant.Fields.InformationFieldId]?.Value,
                    Age = Int32.TryParse(participant.Fields[TournamentFields.Templates.Participant.Fields.AgeFieldId]?.Value, out var age) ? age : 0,
                    Image = GetImageURLFromItem(participant, TournamentFields.Templates.Participant.Fields.ImageFieldId),
                    LinkToSelf = GetLinkToParticipant(participant)

                };
                members.Add(tournamentParticipant);
            }
            return members;
        }



        private string GetLinkToParticipant(Item participantItem)
        {
            string linkToParticipants = string.Empty;

            if (participantItem != null && participantItem.TemplateID == TournamentFields.Templates.Participant.ID)
            {
                linkToParticipants = Sitecore.Links.LinkManager.GetItemUrl(participantItem);
            }

            return linkToParticipants;
        }
        private string GetLinkToTeam(Item teamItem)
        {
            string linkToParticipants = string.Empty;

            if (teamItem != null && teamItem.TemplateID == TournamentFields.Templates.TournamentTeam.ID)
            {
                linkToParticipants = Sitecore.Links.LinkManager.GetItemUrl(teamItem);
            }

            return linkToParticipants;
        }
        public Item GetTournamentItem(Item contextItem)
        {
            if (contextItem == null)
            {
                return null;
            }

            Item currentParent = contextItem.Parent;

            while (currentParent != null && currentParent.TemplateID.ToString() != TournamentFields.MainTemplateID.ToString())
            {
                currentParent = currentParent.Parent;
            }

            return currentParent;
        }
        public TournamentTeam GetTournamentTeam(Item teamItem)
        {
            return new TournamentTeam
            {
                Id = teamItem.ID.ToString(),
                TeamName = teamItem.Fields[TournamentFields.Templates.TournamentTeam.Fields.TeamNameFieldId].Value,
                TeamDescription = teamItem.Fields[TournamentFields.Templates.TournamentTeam.Fields.TeamDescriptionFieldId].Value,
                LogoUrl = GetImageURLFromItem(teamItem, TournamentFields.Templates.TournamentTeam.Fields.TeamLogoFieldId),
                TeamMembers = GetTeamMembers(teamItem),
                LinkToSelf = GetLinkToTeam(teamItem)
            };
        }
        public TournamentMatches GetTournamentMatches(Item tournamentMatchesItem)
        {
            TournamentMatches tournamentMatches = new TournamentMatches();



            List<TournamentStage> tournamentStages = new List<TournamentStage>();



            foreach (Item stageItem in tournamentMatchesItem.Children)
            {
                List<TournamentMatch> tournamentMatchesList = new List<TournamentMatch>();
                TournamentStage tournamentStage = new TournamentStage();

                foreach (Item matchItem in stageItem.Children)
                {


                    if (matchItem.TemplateID == TournamentFields.Templates.TournamentMatch.ID)
                    {
                        DateTime DateOfMatch;

                        if (DateTime.TryParse(matchItem.Fields[TournamentFields.Templates.TournamentMatch.Fields.DateOfMatchFieldId]?.Value, out DateOfMatch))
                        {
                        }
                        else
                        {
                            DateOfMatch = DateTime.MinValue;
                        }


                        tournamentMatches.IsIndividual = GetTournamentType(tournamentMatchesItem);
                        tournamentMatches.Id = tournamentMatches.Id;


                        TournamentMatch tournamentMatch = new TournamentMatch
                        {
                            Id = matchItem.ID.ToString(),
                            MatchName = matchItem.Name,
                            FirstParticipant = GetMatchParticipant(matchItem.Fields[TournamentFields.Templates.TournamentMatch.Fields.FirstParticipantFieldId].Value),
                            SecondParticipant = GetMatchParticipant(matchItem.Fields[TournamentFields.Templates.TournamentMatch.Fields.SecondParticipantFieldId].Value),
                            FirstTeam = GetMatchTeam(matchItem.Fields[TournamentFields.Templates.TournamentMatch.Fields.FirstParticipantFieldId].Value),
                            SecondTeam = GetMatchTeam(matchItem.Fields[TournamentFields.Templates.TournamentMatch.Fields.SecondParticipantFieldId].Value),
                            DateOfMatch = DateOfMatch,
                            Score = Int32.TryParse(matchItem.Fields[TournamentFields.Templates.TournamentMatch.Fields.ScoreFieldId].Value, out var age) ? age : 0,
                            Winner = matchItem.Fields[TournamentFields.Templates.TournamentMatch.Fields.WinnerFieldId].Value
                        };
                        tournamentMatchesList.Add(tournamentMatch);
                    }
                    else
                    {
                        continue;
                    }


                }
                tournamentStage.TournamentMatches = tournamentMatchesList;
                tournamentStage.StageName = stageItem.Fields[TournamentFields.Templates.Stage.Fields.StageNameFieldId].Value;
                tournamentStages.Add(tournamentStage);
            }
            tournamentMatches.TournamentStages = tournamentStages;

            return tournamentMatches;
        }

        private bool GetTournamentType(Item tournamentMatchesItem)
        {
            Item tournament = _masterDb.GetItem(tournamentMatchesItem.ParentID);
            return GetTournamentDroplinkValue(tournament.Fields[TournamentFields.Templates.TournamentInfo.Fields.TournamentTypeFieldId].Value) == "Individual";
        }


        private TournamentParticipant GetMatchParticipant(string value)
        {
            Item participant = _masterDb.GetItem(value);

            if (participant == null)
            {
                return null;
            }

            return new TournamentParticipant
            {
                Id = participant.ID.ToString(),
                Name = participant.Fields[TournamentFields.Templates.Participant.Fields.NameFieldId]?.Value,
                Surname = participant.Fields[TournamentFields.Templates.Participant.Fields.SurnameFieldId]?.Value,
                Info = participant.Fields[TournamentFields.Templates.Participant.Fields.InformationFieldId]?.Value,
                Age = Int32.TryParse(participant.Fields[TournamentFields.Templates.Participant.Fields.AgeFieldId]?.Value, out var age) ? age : 0,
                Image = GetImageURLFromItem(participant, TournamentFields.Templates.Participant.Fields.ImageFieldId),
                LinkToSelf = GetLinkToParticipant(participant)
            };
        }
        private TournamentTeam GetMatchTeam(string value)
        {
            Item teamItem = _masterDb.GetItem(value);

            if (teamItem == null)
            {
                return null;
            }

            TournamentTeam tournamentTeam = GetTournamentTeam(teamItem);

            return tournamentTeam;
        }
        public TournamentMatch GetMatchData(string matchId)
        {
            Item matchItem = _masterDb.GetItem(matchId);

            if (matchItem == null)
            {
                return null;
            }

            return new TournamentMatch
            {
                Id = matchItem.ID.ToString(),
                MatchName = matchItem.Name,
                FirstParticipant = GetMatchParticipant(matchItem.Fields[TournamentFields.Templates.TournamentMatch.Fields.FirstParticipantFieldId].Value),
                SecondParticipant = GetMatchParticipant(matchItem.Fields[TournamentFields.Templates.TournamentMatch.Fields.SecondParticipantFieldId].Value),
                FirstTeam = GetMatchTeam(matchItem.Fields[TournamentFields.Templates.TournamentMatch.Fields.FirstParticipantFieldId].Value),
                SecondTeam = GetMatchTeam(matchItem.Fields[TournamentFields.Templates.TournamentMatch.Fields.SecondParticipantFieldId].Value)
            };
        }
    }
}
