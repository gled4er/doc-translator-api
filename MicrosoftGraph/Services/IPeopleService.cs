using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graph;
using MicrosoftGraph.Model;

namespace MicrosoftGraph.Services
{
    /// <summary>
    /// Interface for People Service
    /// </summary>
    public interface IPeopleService
    {
        /// <summary>
        /// Provide emails and additional information for users by their name
        /// </summary>
        /// <param name="users">List of users</param>
        /// <param name="accessToken">Microsoft Graph Access Token</param>
        /// <returns></returns>
        Task<List<MicrosoftGraph.Model.Person>> GetPeolpe(List<User> users, string accessToken);
    }
}
