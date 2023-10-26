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
using TourneyHub.Feature.Tournament.Fields;
using TourneyHub.Feature.Tournament.Models;

namespace TourneyHub.Feature.Tournament.Services
{
    public class TournamentService
    {
        private readonly Database _masterDb;
        public static string TounamentTypeIndividual;
        public static string TounamentTypeTeam;

        //TODO: FIX number of participants and teams in FE
        public TournamentService()
        {
            // Initialize the database in the constructor using a constant or configuration.
            _masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
            TounamentTypeIndividual = "Individual";
            TounamentTypeTeam = "Team";
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
                }
            }
            catch (Exception ex)
            {
                // Properly handle or log the exception.
            }
        }
        public List<TournamentModel> GetTournaments()
        {
            Item tournamentFolderItem = _masterDb.GetItem(TournamentFields.TournamentParentPageId);

            List<TournamentModel> tournamentsList = tournamentFolderItem.Children
                .Where(tournament => tournament.TemplateID == TournamentFields.MainTemplateID)
                .Select(tournament => new TournamentModel
                {
                    TournamentType = tournament.Fields[TournamentFields.Templates.TournamentInfo.Fields.TournamentTypeFieldId].Value,
                    TournamentFormat = tournament.Fields[TournamentFields.Templates.TournamentInfo.Fields.TournamentFormatFieldId].Value,
                    SportName = tournament.Fields[TournamentFields.Templates.TournamentInfo.Fields.SportNameFieldId].Value,
                    TournamentName = tournament.Fields[TournamentFields.Templates.TournamentInfo.Fields.TournamentNameFieldId].Value,
                    LinkToSelf = Sitecore.Links.LinkManager.GetItemUrl(tournament),
                    CreatedByUser = tournament.Fields[TournamentFields.Templates.TournamentInfo.Fields.CreatedByUserFieldId].Value
                })
                .ToList();

            return tournamentsList;
        }


        private void CreateTournamentMatches(TournamentFormData tournamentFormData, Item parentTournamentItem, Item tournament)
        {
            using (new SecurityDisabler())
            {
                if (parentTournamentItem == null)
                {
                    return;
                }

                TemplateItem tournamentMatchTemplate = _masterDb.GetTemplate(TournamentFields.Templates.TournamentMatches.ID);
                string tournamentMatchesName = TournamentFields.TournamentItemNames.TournamentMatch;
                Item tournamentMatchesItem = parentTournamentItem.Add(tournamentMatchesName, tournamentMatchTemplate);

                using (new EditContext(tournamentMatchesItem))
                {
                    tournamentMatchesItem.Editing.BeginEdit();

                    int numberOfMatches = 0;
                    TournamentParticipants tournamentParticipants = GetTournamentParticipants(tournament);

                    if (tournamentFormData.TournamentType == TounamentTypeIndividual)
                    {
                        numberOfMatches = tournamentFormData.NumberOfParticipants / 2;
                        if (tournamentParticipants != null)
                        {
                            List<TournamentParticipant> participants = tournamentParticipants.Participants;
                            if (participants.Count < 2 * numberOfMatches)
                            {
                                return;
                            }

                            for (int matchNumber = 1; matchNumber <= numberOfMatches; matchNumber++)
                            {
                                string matchName = $"{tournamentMatchesName} {matchNumber}";
                                TemplateItem matchTemplate = _masterDb.GetTemplate(TournamentFields.Templates.TournamentMatch.ID);
                                Item matchItem = tournamentMatchesItem.Add(matchName, matchTemplate);

                                using (new EditContext(matchItem))
                                {
                                    matchItem.Editing.BeginEdit();
                                    matchItem.Fields[TournamentFields.Templates.TournamentMatch.Fields.FirstParticipantFieldId].Value = participants[2 * matchNumber - 2].Id;
                                    matchItem.Fields[TournamentFields.Templates.TournamentMatch.Fields.SecondParticipantFieldId].Value = participants[2 * matchNumber - 1].Id;
                                    matchItem.Editing.EndEdit();
                                }
                            }
                        }
                    }
                    else if (tournamentFormData.TournamentType == TounamentTypeTeam)
                    {
                        numberOfMatches = tournamentFormData.NumberOfTeams / 2;
                        if (tournamentParticipants != null)
                        {
                            List<TournamentTeam> teams = tournamentParticipants.Teams;
                            if (teams.Count < 2 * numberOfMatches)
                            {
                                // Handle the case where there are not enough teams for the matches.
                                // You can throw an exception, log an error, or handle it according to your requirements.
                                return;
                            }

                            for (int matchNumber = 1; matchNumber <= numberOfMatches; matchNumber++)
                            {
                                string matchName = $"{tournamentMatchesName} {matchNumber}";
                                TemplateItem matchTemplate = _masterDb.GetTemplate(TournamentFields.Templates.TournamentMatch.ID);
                                Item matchItem = tournamentMatchesItem.Add(matchName, matchTemplate);

                                using (new EditContext(matchItem))
                                {
                                    matchItem.Editing.BeginEdit();
                                    matchItem.Fields[TournamentFields.Templates.TournamentMatch.Fields.FirstParticipantFieldId].Value = teams[2 * matchNumber - 2].Id;
                                    matchItem.Fields[TournamentFields.Templates.TournamentMatch.Fields.SecondParticipantFieldId].Value = teams[2 * matchNumber - 1].Id;
                                    matchItem.Editing.EndEdit();
                                }
                            }
                        }
                    }

                    tournamentMatchesItem.Editing.EndEdit();
                }
            }
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
                Participants = participants,
                Teams = tournamentTeams
            };
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

                        if (tournamentFormData.TournamentType == TounamentTypeIndividual)
                        {
                            CreateTournamentParticipant(participantsItem, tournamentFormData.NumberOfParticipants);
                        }
                        else if (tournamentFormData.TournamentType == TounamentTypeTeam)
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
                        // Use LINQ to simplify the search for the user item
                        Item currentUserItem = userFolder.Children
                            .FirstOrDefault(userItem => username == userItem.Fields[UserFields.Fields.UsernameFieldId].Value);

                        return currentUserItem; // Return the found item if it exists
                    }
                }
            }

            return null; // Return null if no user item is found
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

    }
}
