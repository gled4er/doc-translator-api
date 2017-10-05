using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graph;
using Newtonsoft.Json;
using MicrosoftGraph.Model;

namespace MicrosoftGraph.Services
{
    /// <summary>
    /// People Service
    /// </summary>
    [Serializable]
    public class PeopleService : IPeopleService
    {
        public readonly IHttpService HttpService;
        public readonly ILoggingService LoggingService;
        private const string PeopleEndpoint = "https://graph.microsoft.com/v1.0/users?$select=DisplayName,Mail&$Filter=";

        public PeopleService(IHttpService httpService, ILoggingService loggingService)
        {
            HttpService = httpService;
            LoggingService = loggingService;
        }

        /// <summary>
        /// Provide emails and additional information for users by their name
        /// </summary>
        /// <param name="users">List of users</param>
        /// <param name="accessToken">Microsoft Graph Access Token</param>
        /// <returns></returns>
        public async Task<List<Model.Person>> GetPeolpe(List<User> users, string accessToken)
        {
            if (users == null || users.Count == 0 || string.IsNullOrEmpty(accessToken))
            {
                return new List<Model.Person>();
            }
            try
            {
                var queryBuilder = new StringBuilder();
                queryBuilder.Append(PeopleEndpoint);
                for (var i = 0; i < users.Count; i++)
                {
                    if (i > 0 && i < users.Count - 1)
                    {
                        queryBuilder.Append(" or ");
                    }

                    if (!string.IsNullOrEmpty(users[i].GivenName) && !string.IsNullOrEmpty(users[i].Surname))
                    {
                        queryBuilder.Append($"(Givenname eq '{users[i].GivenName}' and Surname eq '{users[i].Surname}')");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(users[i].GivenName) && string.IsNullOrEmpty(users[i].Surname))
                        {
                            queryBuilder.Append($"Givenname eq '{users[i].GivenName}'");
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(users[i].GivenName) && !string.IsNullOrEmpty(users[i].Surname))
                            {
                                queryBuilder.Append($"Surname eq '{users[i].Surname}'");
                            }
                        }
                    }
                }
                var endpoint = queryBuilder.ToString();
                var httpResponseMessage  =  await HttpService.AuthenticatedGet(endpoint, accessToken);
                var userSearchResponse = JsonConvert.DeserializeObject<PersonSearchResponse>(await httpResponseMessage.Content.ReadAsStringAsync());
                return userSearchResponse != null ? userSearchResponse.Value : new List<Model.Person>();
            }
            catch (Exception exception)
            {
                LoggingService.Error(exception, "Error in PeopleService.GetPeople");
                throw;
            }
        }
    }
}