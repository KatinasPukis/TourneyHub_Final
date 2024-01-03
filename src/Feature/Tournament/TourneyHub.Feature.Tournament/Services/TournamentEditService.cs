using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Resources.Media;
using Sitecore.Security.Accounts;
using Sitecore.Security.Authentication;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Security;
using TourneyHub.Feature.Registration.Fields;
using TourneyHub.Feature.Registration.Models;
using TourneyHub.Feature.Tournament.Fields;
using TourneyHub.Feature.Tournament.Models;

namespace TourneyHub.Feature.Tournament.Services
{
    public class TournamentEditService
    {
        private readonly Database _masterDb;
        public TournamentEditService()
        {
            _masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
        }

        public void EditTournamentParticipantItem(TournamentParticipant participantData)
        {
            if (participantData == null)
            {
                return;
            }

            HttpPostedFileBase imageFile = participantData.EditImage;

            Item imageItem = CreateImageItem(imageFile);

            Item participantItem = _masterDb.GetItem(participantData.Id);

            if (participantItem == null)
            {
                return;
            }

            using (new SecurityDisabler())
            {
                using (new EditContext(participantItem))
                {
                    participantItem.Editing.BeginEdit();
                    participantItem.Fields[TournamentFields.Templates.Participant.Fields.NameFieldId].Value = participantData.Name;
                    participantItem.Fields[TournamentFields.Templates.Participant.Fields.SurnameFieldId].Value = participantData.Surname;
                    participantItem.Fields[TournamentFields.Templates.Participant.Fields.InformationFieldId].Value = participantData.Info;
                    participantItem.Fields[TournamentFields.Templates.Participant.Fields.AgeFieldId].Value = participantData.Age.ToString();

                    ImageField imageField = (ImageField)participantItem.Fields[TournamentFields.Templates.Participant.Fields.ImageFieldId];
                    if (imageField != null && imageItem != null)
                    {
                        imageField.MediaID = imageItem.ID;
                    }

                    participantItem.Editing.EndEdit();
                }
            }
        }


        //Helper
        private Item CreateImageItem(HttpPostedFileBase imageFile)
        {
            try
            {
                if (imageFile != null)
                {
                    string destinationPath = "/sitecore/media library/Images";

                    Item existingFolder = _masterDb.GetItem(destinationPath);
                    string sanitizedFileName = Path.GetFileNameWithoutExtension(imageFile.FileName).Replace(".", "");
                    if (existingFolder != null)
                    {

                        Item existingMediaItem = existingFolder.Axes.GetDescendants()
                            .FirstOrDefault(item => item.Name.Equals(sanitizedFileName, StringComparison.OrdinalIgnoreCase));

                        if (existingMediaItem != null)
                        {
                            return existingMediaItem;
                        }
                        string uniqueName = sanitizedFileName;
                        destinationPath = destinationPath + "/" + uniqueName;
                    }

                    MediaCreatorOptions options = new MediaCreatorOptions
                    {
                        Database = _masterDb,
                        Destination = destinationPath,
                        Versioned = false,
                    };



                    MediaItem mediaItem = MediaManager.Creator.CreateFromStream(imageFile.InputStream, sanitizedFileName, options);

                    if (mediaItem != null)
                    {
                        return mediaItem;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public void EditTournament(TournamentModel tournament)
        {
            if (tournament == null)
            {
                return;
            }

            Item tournamentItem = _masterDb.GetItem(tournament.Id);

            if (tournamentItem != null)
            {
                using (new SecurityDisabler())
                {
                    using (new EditContext(tournamentItem))
                    {
                        tournamentItem.Editing.BeginEdit();
                        tournamentItem.Fields[TournamentFields.Templates.TournamentInfo.Fields.SportNameFieldId].Value = tournament.SportName;

                        SetDateTimeFieldValue(tournamentItem, TournamentFields.Templates.TournamentInfo.Fields.TournamentStartDateFieldId, tournament.StartDate);
                        SetDateTimeFieldValue(tournamentItem, TournamentFields.Templates.TournamentInfo.Fields.TournamentEndDateFieldId, tournament.EndDate);

                        tournamentItem.Editing.EndEdit();
                    }
                }
            }

        }

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

        public void EditUser(UserViewModel userModel, bool updatePassword)
        {
            User currentUser = Sitecore.Context.User;

            if (currentUser.IsAuthenticated)
            {
                Item userItem = _masterDb.GetItem(userModel.Id);

                if (userItem != null)
                {
                    using (new SecurityDisabler())
                    {
                        using (new EditContext(userItem))
                        {
                            userItem.Editing.BeginEdit();
                            userItem[UserFields.Fields.NameFieldId] = userModel.Name;
                            userItem[UserFields.Fields.SurnameFieldId] = userModel.Surname;
                            userItem[UserFields.Fields.EmailFieldId] = userModel.Email;
                            userItem.Editing.EndEdit();

                            if (updatePassword)
                            {
                                string newPassword = userModel.Password;

                                MembershipUser membershipUser = Membership.GetUser(currentUser.Name);

                                if (membershipUser != null)
                                {
                                    string oldPassword = membershipUser.ResetPassword();
                                    membershipUser.ChangePassword(oldPassword, newPassword);
                                }
                            }
                        }
                    }
                }
            }
        }
        public void DeleteItem(string itemId)
        {
            try
            {
                ID itemID = new ID(itemId);
                Item itemToDelete = Sitecore.Context.Database.GetItem(itemID);

                if (itemToDelete != null)
                {
                    using (new SecurityDisabler())
                    {
                        RemoveLinksofTargetItem(itemToDelete);
                        itemToDelete.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error deleting item: " + ex.Message, this);
            }
        }

        public void RemoveLinksofTargetItem(Item targetItem)
        {
            var links = Globals.LinkDatabase.GetReferrers(targetItem);

            foreach (var link in links)
            {
                Item sourceItem = link.GetSourceItem();
                if (sourceItem != null && !ID.IsNullOrEmpty(link.SourceFieldID))
                    RemoveLink(sourceItem, link);
            }
        }
        private void RemoveLink(Item item, ItemLink itemLink)
        {
            CustomField field = FieldTypeManager.GetField(item.Fields[itemLink.SourceFieldID]);
            if (field != null)
            {
                using (new SecurityDisabler())
                {
                    item.Editing.BeginEdit();
                    field.RemoveLink(itemLink);
                    item.Editing.EndEdit();
                }
            }
        }
        public void EditTournamentTeamItem(TournamentTeam teamData)
        {
            if (teamData == null)
            {
                return;
            }

            HttpPostedFileBase imageFile = teamData.TeamImage;

            Item imageItem = CreateImageItem(imageFile);

            Item teamItem = _masterDb.GetItem(teamData.Id);

            if (teamItem == null)
            {
                return;
            }

            using (new SecurityDisabler())
            {
                using (new EditContext(teamItem))
                {
                    teamItem.Editing.BeginEdit();
                    teamItem.Fields[TournamentFields.Templates.TournamentTeam.Fields.TeamNameFieldId].Value = teamData.TeamName;
                    teamItem.Fields[TournamentFields.Templates.TournamentTeam.Fields.TeamDescriptionFieldId].Value = teamData.TeamDescription;
                    ImageField imageField = (ImageField)teamItem.Fields[TournamentFields.Templates.TournamentTeam.Fields.TeamLogoFieldId];
                    if (imageField != null && imageItem != null)
                    {
                        imageField.MediaID = imageItem.ID;
                    }

                    teamItem.Editing.EndEdit();
                }
            }
        }

        public void DeleteMatchResults(DeleteResultModel deleteResultModel)
        {
            Item matchItem = _masterDb.GetItem(deleteResultModel.MatchId);
            using (new SecurityDisabler())
            {
                using (new EditContext(matchItem))
                {
                    matchItem.Editing.BeginEdit();
                    matchItem?.DeleteChildren();
                    matchItem[TournamentFields.Templates.TournamentMatch.Fields.WinnerFieldId] = string.Empty;
                    matchItem.Editing.EndEdit();
                }
            }
           
            Item stageItem = matchItem.Parent;
            if (matchItem != null)
            {
                bool shouldSkip = false;
                foreach (Item stage in stageItem.Parent.Children)
                {
                    if (stage.ID == stageItem.ID)
                    {
                        shouldSkip = true;
                        continue;
                    }

                    if (shouldSkip)
                    {

                        foreach (Item match in stage.Children)
                        {
                            if (match.Fields[TournamentFields.Templates.TournamentMatch.Fields.FirstParticipantFieldId].Value == deleteResultModel.FirstParticipantId
                                || match.Fields[TournamentFields.Templates.TournamentMatch.Fields.SecondParticipantFieldId].Value == deleteResultModel.FirstParticipantId
                                || match.Fields[TournamentFields.Templates.TournamentMatch.Fields.FirstParticipantFieldId].Value == deleteResultModel.SecondParticipantId
                                || match.Fields[TournamentFields.Templates.TournamentMatch.Fields.SecondParticipantFieldId].Value == deleteResultModel.SecondParticipantId)
                            {
                                using (new SecurityDisabler())
                                {
                                    using (new EditContext(match))
                                    {
                                        match.Editing.BeginEdit();
                                        if (match.Fields[TournamentFields.Templates.TournamentMatch.Fields.FirstParticipantFieldId].Value == deleteResultModel.FirstParticipantId)
                                        {
                                            match[TournamentFields.Templates.TournamentMatch.Fields.FirstParticipantFieldId] = string.Empty; // Remove or set to empty
                                        }
                                        if (match.Fields[TournamentFields.Templates.TournamentMatch.Fields.SecondParticipantFieldId].Value == deleteResultModel.FirstParticipantId)
                                        {
                                            match[TournamentFields.Templates.TournamentMatch.Fields.SecondParticipantFieldId] = string.Empty; // Remove or set to empty
                                        }
                                        if (match.Fields[TournamentFields.Templates.TournamentMatch.Fields.FirstParticipantFieldId].Value == deleteResultModel.SecondParticipantId)
                                        {
                                            match[TournamentFields.Templates.TournamentMatch.Fields.FirstParticipantFieldId] = string.Empty; // Remove or set to empty
                                        }
                                        if (match.Fields[TournamentFields.Templates.TournamentMatch.Fields.SecondParticipantFieldId].Value == deleteResultModel.SecondParticipantId)
                                        {
                                            match[TournamentFields.Templates.TournamentMatch.Fields.SecondParticipantFieldId] = string.Empty; // Remove or set to empty
                                        }

                                        match[TournamentFields.Templates.TournamentMatch.Fields.WinnerFieldId] = string.Empty;
                                        match?.DeleteChildren();
                                        match.Editing.EndEdit();
                                        foreach (Item score in match.Children)
                                        {
                                            if (score.Fields[TournamentFields.Templates.Score.Fields.ParticipantFieldId].Value == deleteResultModel.FirstParticipantId
                                                || score.Fields[TournamentFields.Templates.Score.Fields.ParticipantFieldId].Value == deleteResultModel.SecondParticipantId)
                                            {
                                                score.Delete();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        continue;
                    }

                }
            }
        }
        public void AddNewParticipantToTournament(string tournamentId)
        {
            if (string.IsNullOrEmpty(tournamentId))
            {
                return;
            }

            Item tournamentItem = _masterDb.GetItem(tournamentId);

            if (tournamentItem == null)
            {
                return;
            }

            Item participantsFolderItem = tournamentItem.Children
                .FirstOrDefault(child => child.TemplateID == TournamentFields.Templates.Participants.ID);

            TemplateItem participantTemplate = _masterDb.GetTemplate(TournamentFields.Templates.Participant.ID);

            string participantsItemName = GetUniqueItemName("Participant", participantsFolderItem);

            Item participantItem = participantsFolderItem.Add(participantsItemName, participantTemplate);

            using (new SecurityDisabler())
            {
                using (new EditContext(participantItem))
                {
                    participantItem.Editing.BeginEdit();

                    participantItem.Fields[TournamentFields.Templates.Participant.Fields.NameFieldId].Value = participantsItemName;

                    participantItem.Editing.EndEdit();
                }
            }
        }
        public void AddNewParticipantToTeam(string teamItemId)
        {
            if (string.IsNullOrEmpty(teamItemId))
            {
                return;
            }

            Item teamItem = _masterDb.GetItem(teamItemId);

            if (teamItem == null)
            {
                return;
            }


            TemplateItem participantTemplate = _masterDb.GetTemplate(TournamentFields.Templates.Participant.ID);

            string participantsItemName = GetUniqueItemName("Participant", teamItem);

            Item participantItem = teamItem.Add(participantsItemName, participantTemplate);

            using (new SecurityDisabler())
            {
                using (new EditContext(participantItem))
                {
                    participantItem.Editing.BeginEdit();

                    participantItem.Fields[TournamentFields.Templates.Participant.Fields.NameFieldId].Value = participantsItemName;

                    participantItem.Editing.EndEdit();
                }
            }
        }
        public void AddNewTeamtToTournament(string tournamentId)
        {
            if (string.IsNullOrEmpty(tournamentId))
            {
                return;
            }

            Item tournamentItem = _masterDb.GetItem(tournamentId);

            if (tournamentItem == null)
            {
                return;
            }

            Item participantsFolderItem = tournamentItem.Children
                .FirstOrDefault(child => child.TemplateID == TournamentFields.Templates.Participants.ID);

            TemplateItem teamItemTemplate = _masterDb.GetTemplate(TournamentFields.Templates.TournamentTeam.ID);

            string teamItemName = GetUniqueItemName("Team", participantsFolderItem);

            Item teamItem = participantsFolderItem.Add(teamItemName, teamItemTemplate);

            using (new SecurityDisabler())
            {
                using (new EditContext(teamItem))
                {
                    teamItem.Editing.BeginEdit();

                    teamItem.Fields[TournamentFields.Templates.TournamentTeam.Fields.TeamNameFieldId].Value = teamItemName;

                    teamItem.Editing.EndEdit();
                }
            }
        }
        public string GetUniqueItemName(string baseName, Item parentItem)
        {
            List<string> existingNames = parentItem.Children.Select(child => child.Name).ToList();

            int counter = 0;

            string uniqueName = baseName;

            while (existingNames.Contains(uniqueName))
            {
                counter++;
                uniqueName = $"{baseName}{counter}";
            }

            return uniqueName;
        }
    }


}