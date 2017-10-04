using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graph;
using Newtonsoft.Json;
using MicrosoftGraph.Model;

namespace MicrosoftGraph.Services
{
    /// <summary>
    /// Group service 
    /// </summary>
    [Serializable]
    public class GroupService : IGroupService
    {
        private readonly IHttpService _httpService;
        private readonly ILoggingService _loggingService;
        private const string GroupEndpoint = "https://graph.microsoft.com/v1.0/groups/";

        /// <summary>
        /// Group service constructor
        /// </summary>
        /// <param name="httpService">HTTP service </param>
        /// <param name="loggingService">Logging service</param>
        public GroupService(IHttpService httpService, ILoggingService loggingService)
        {
            _httpService = httpService;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Get group information by email
        /// </summary>
        /// <param name="email">Group email</param>
        /// <param name="accessToken">Microsoft Graph access token</param>
        /// <returns>Group ID</returns>
        public async Task<string> GetGroupId(string email, string accessToken)
        {
            try
            {
                var endpoint = $"{GroupEndpoint}?$fileter=mail eq '{email}'";
                var httpResponseMessage = await _httpService.AuthenticatedGet(endpoint, accessToken);
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                var groupResponse = JsonConvert.DeserializeObject<GroupResponse>(content);
                var groupId = groupResponse.Value[0].Id;
                return groupId;
            }
            catch (Exception e)
            {
                _loggingService.Error(e, "Error in GroupService.GetGroupID");
                throw;
            }
        }

        /// <summary>
        /// Get group members by group id
        /// </summary>
        /// <param name="groupId">ID of Microsoft Graph Group</param>
        /// <param name="accessToken">Microsoft Graph Access Token</param>
        /// <returns>List of <see cref="User"/></returns>
        public async Task<List<User>> GetMembers(string groupId, string accessToken)
        {
            try
            {
                var endpoint = $"{GroupEndpoint}{groupId}/members";
                var httpResponseMessage = await _httpService.AuthenticatedGet(endpoint, accessToken);
                var members = JsonConvert.DeserializeObject<GroupMemberResponse>(await httpResponseMessage.Content.ReadAsStringAsync());
                return members.Value;
            }
            catch (Exception e)
            {
                _loggingService.Error(e, "Error in GroupService.GetMembers");
                throw;
            }
        }
    }
}