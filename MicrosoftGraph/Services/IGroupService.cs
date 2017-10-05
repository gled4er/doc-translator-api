using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace MicrosoftGraph.Services
{
    /// <summary>
    /// Group Service
    /// </summary>
    public interface IGroupService
    {
        /// <summary>
        /// Get group information by email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="accessToken">Microsoft Graph access token</param>
        /// <returns>Group Id</returns>
        Task<string> GetGroupId(string email, string accessToken);

        /// <summary>
        /// Get group members by group id
        /// </summary>
        /// <param name="groupId">Microsoft Graph group id</param>
        /// <param name="accessToken">Microsoft Graph access token</param>
        /// <returns></returns>
        Task<List<User>> GetMembers(string groupId, string accessToken);
    }
}
