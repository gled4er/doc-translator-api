using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicrosoftGraph.Services
{
    /// <summary>
    /// Email Service 
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Get emails from raw user input
        /// </summary>
        /// <param name="emailInput">Email user input</param>
        /// <param name="accessToken">Microsoft Graph Access Token</param>
        /// <returns></returns>
        Task<List<string>> GetEmails(string emailInput, string accessToken);
    }
}
