using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace TestApp.Authentication
{
    internal static class AuthenticationHelper
    {
        /// <summary>
        /// Get Token for User.
        /// </summary>
        /// <returns>Token for user.</returns>
        public static async Task<string> GetTokenForUser(string tenantName, string clientId)
        {
            var authString = string.Format("https://login.microsoftonline.com/common/{0}", tenantName);
            var resourceUrl = "https://graph.microsoft.com";
            var redirectUri = new Uri("http://localhost:8080");
            var authenticationContext = new AuthenticationContext(authString, false);
            var userAuthResult = await authenticationContext.AcquireTokenAsync(resourceUrl,
                clientId, redirectUri, new PlatformParameters(PromptBehavior.RefreshSession));
            var tokenForUser = userAuthResult.AccessToken;
            return tokenForUser;
        }

    }
}
