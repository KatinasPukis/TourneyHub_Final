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
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            string accountName = string.Empty;
            Domain domain = Sitecore.Context.Domain;

            try
            {
                if (domain != null)
                {
                    accountName = domain.GetFullName(username);
                }
                if (AuthenticationManager.Login(accountName, password))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {

            }

            return false;
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
                        Item currentUserItem = userFolder.Children
                            .FirstOrDefault(userItem => username == userItem.Fields[UserFields.Fields.UsernameFieldId].Value);

                        return currentUserItem;
                    }
                }
            }

            return null;
        }
    }

}