using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
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
            // Initialize the database in the constructor using a constant or configuration.
            _masterDb = Sitecore.Configuration.Factory.GetDatabase("master");
        }

        public void EditTournamentParticipantItem(TournamentParticipant participantData)
        {
            if (participantData == null)
            {
                // Handle the case where participantData is null
                // You can log an error or return an error response
                return;
            }

            HttpPostedFileBase imageFile = participantData.EditImage;

            // Create or update the image item in Sitecore
            Item imageItem = CreateImageItem(imageFile);

            Item participantItem = _masterDb.GetItem(participantData.Id);

            if (participantItem == null)
            {
                // Handle the case where the participant item doesn't exist
                // You can log an error or return an error response
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

                    // Set the value of the Image Field with the image item ID
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
                    // Define the destination path
                    string destinationPath = "/sitecore/media library/Images";

                    // Check if the folder already exists at the destination
                    Item existingFolder = _masterDb.GetItem(destinationPath);
                    string sanitizedFileName = Path.GetFileNameWithoutExtension(imageFile.FileName).Replace(".", "");
                    if (existingFolder != null)
                    {

                        Item existingMediaItem = existingFolder.Axes.GetDescendants()
                            .FirstOrDefault(item => item.Name.Equals(sanitizedFileName, StringComparison.OrdinalIgnoreCase));

                        if (existingMediaItem != null)
                        {
                            // The media item with the same name already exists; return its URL
                            return existingMediaItem;
                        }

                        // Append a unique identifier to the folder name
                        string uniqueName = sanitizedFileName;
                        destinationPath = destinationPath + "/" + uniqueName;
                    }

                    // Create a media item in Sitecore for the uploaded image
                    MediaCreatorOptions options = new MediaCreatorOptions
                    {
                        Database = _masterDb, // Use the appropriate database
                        Destination = destinationPath, // Set the updated path
                        Versioned = false, // Set to true if you want versioned media items
                    };



                    MediaItem mediaItem = MediaManager.Creator.CreateFromStream(imageFile.InputStream, sanitizedFileName, options);

                    if (mediaItem != null)
                    {
                        return mediaItem;
                    }
                    else
                    {
                        // Handle the case where mediaItem is null
                        // Log an error or return an error response
                        return null; // You can choose how to handle this case
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log it, and return an error response
                Console.WriteLine(ex.Message);
            }

            return null; // Return null in case of any error or when imageFile is null
        }

        public void EditTournament(TournamentModel tournament)
        {
            if (tournament == null)
            {
                // Handle the case where the 'tournament' parameter is null.
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
            else
            {
                // Handle the case where 'tournamentItem' is null.
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
                // Set the DateTime value to the field
                dateTimeField.Value = DateUtil.ToIsoDate((DateTime)dateTimeValue);
            }
            else
            {
                // Handle the case where 'dateTimeField' is null
            }
        }



        public void EditUser(UserViewModel userModel)
        {
            // Get the current user in Sitecore
            User currentUser = Sitecore.Context.User;

            // Check if the user is authenticated
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



        public void DeleteTournament(string id)
        {
            try
            {
                Item itemToDelete = Sitecore.Context.Database.GetItem(new ID(id));

                if (itemToDelete != null)
                {
                    using (new SecurityDisabler())
                    {
                        itemToDelete.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error deleting tournament: " + ex.Message, this);
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
            Item stageItem = matchItem.Parent;
            if (matchItem != null)
            {
                bool shouldSkip = false;
                foreach (Item stage in stageItem.Parent.Children)
                {
                    if (stage.ID == stageItem.ID)
                    {
                        shouldSkip = true; // Set the flag to skip the next item
                        continue; // Continue to the next iteration
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
                        continue; // Skip the current item
                    }

                }
            }
        }
    }
}