using Sitecore.Security.Authentication;
using Sitecore.Security.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TourneyHub.Feature.Login.Models;

namespace TourneyHub.Feature.Login.Services
{
    using Sitecore;
    using Sitecore.Data.Items;
    using Sitecore.Security.Accounts;
    using Sitecore.Security.Authentication;
    using TourneyHub.Feature.Registration.Fields;

    public class LoginService
    {
        public bool LogInUser(string username, string password)
        {
            // Check if the username and password are not empty (add more validation as needed)
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false; // Invalid input
            }

            string accountName = string.Empty;
            Domain domain = Sitecore.Context.Domain;

            try
            {
                if (domain != null)
                {
                    accountName = domain.GetFullName(username);
                }

                // Attempt to log in the user
                if (AuthenticationManager.Login(accountName, password))
                {
                    return true; // Login successful
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging and error handling
                // You may also consider rethrowing the exception for higher-level error handling
            }

            return false; // Login failed
        }
        public Item GetCurrentUserItem()
        {
            if (Sitecore.Context.User.IsAuthenticated)
            {
                User user = AuthenticationManager.GetActiveUser();

                if (user != null)
                {
                    Item userFolder = Sitecore.Context.Database.GetItem(UserFields.UsersFolderId);

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
    }

}