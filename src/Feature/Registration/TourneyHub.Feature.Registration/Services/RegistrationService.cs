using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Security.Accounts;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using TourneyHub.Feature.Registration.Fields;
using TourneyHub.Feature.Registration.Models;

namespace TourneyHub.Feature.Registration.Services
{
    public class RegistrationService
    {
        private const string Domain = @"extranet";
        private const string UsersFolderPath = "/sitecore/content/Users";
        public RegistrationResult RegisterUser(UserViewModel userViewModel)
        {
            string userName = string.Format(@"{0}\{1}", Domain, userViewModel.Username);

            try
            {
                // Check if the user already exists
                if (UserExists(userName))
                {
                    return RegistrationResult.UserExists;
                }

                // TODO: Implement password encryption

                Role role = Role.FromName(@"extranet\TourneyHubUser");

                // Create a new user
                Membership.CreateUser(userName, userViewModel.Password, userViewModel.Email);
                Sitecore.Security.Accounts.User user = Sitecore.Security.Accounts.User.FromName(userName, true);

                // Assign the user to the specified role
                user.Roles.Add(role);

                // Set user profile information
                Sitecore.Security.UserProfile userProfile = user.Profile;
                userProfile.FullName = string.Format("{0} {1}", userViewModel.Name, userViewModel.Surname);
                userProfile.Email = userViewModel.Email;
                userProfile.Comment = "TourneyHub User";
                userProfile.Save();

                // Create a user item under the Users folder
                using (new SecurityDisabler())
                {

                    Item usersFolder = Sitecore.Context.Database.GetItem(UserFields.UsersFolderId);
                    string userItemName = userViewModel.Username;
                    TemplateItem userTemplate = Sitecore.Context.Database.GetTemplate(UserFields.Id);
                    Item newUserItem = usersFolder.Add(userItemName, userTemplate);

                    using (new EditContext(newUserItem))
                    {
                        if (usersFolder != null)
                        {
                            newUserItem.Editing.BeginEdit();
                            newUserItem[UserFields.Fields.UsernameFieldId] = userViewModel.Username;
                            newUserItem[UserFields.Fields.NameFieldId] = userViewModel.Name;
                            newUserItem[UserFields.Fields.SurnameFieldId] = userViewModel.Surname;
                            newUserItem[UserFields.Fields.EmailFieldId] = userViewModel.Email;
                            newUserItem.Editing.EndEdit();

                        }
                    }
                }
                return RegistrationResult.Success;
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine(ex.Message);
                Sitecore.Diagnostics.Log.Error($"Error during user registration: {ex.Message}", this);
                return RegistrationResult.Error;
            }
        }

        private bool UserExists(string userName)
        {
            // Check if a user with the same username already exists
            return Sitecore.Security.Accounts.User.Exists(userName);
        }
    }

    public enum RegistrationResult
    {
        Success,
        UserExists,
        Error
    }

}