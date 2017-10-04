using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User = Microsoft.Graph.User;

namespace MicrosoftGraph.Services
{
    /// <summary>
    /// Email Service 
    /// </summary>
    [Serializable]
    public class EmailService : IEmailService
    {
        private readonly ILoggingService _loggingService;
        private readonly IGroupService _groupService;
        private readonly IPeopleService _peopleService;

        /// <summary>
        /// Email Service constructor
        /// </summary>
        /// <param name="groupService">Instance of <see cref="GroupService"/></param>
        /// <param name="peopleService">Instance of <see cref="PeopleService"/></param>
        /// <param name="loggingService">Instance of <see cref="LoggingService"/></param>
        public EmailService(IGroupService groupService, IPeopleService peopleService, ILoggingService loggingService)
        {
            _groupService = groupService;
            _peopleService = peopleService;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Get emails from raw user input
        /// </summary>
        /// <param name="emailInput">Email user input</param>
        /// <param name="accessToken">Microsoft Graph access token</param>
        /// <returns></returns>
        public async Task<List<string>> GetEmails(string emailInput, string accessToken)
        {
            try
            {
                var emailList = new List<string>();
                //This is because in Skype for business, " "(space) is automatically converted to "&#160;", which is blocking to get emails
                var emailInputImproved = emailInput.Replace("&#160;", "").Replace("&#160:^", "");
                //This is removing hyper-link which Skype for business automatically adds
                var emails = System.Text.RegularExpressions.Regex.Replace(emailInputImproved, "\\(.+?\\)", "");
                var emailArray = emails.Split(',');
                foreach (var email in emailArray)
                {
                    // We have name 
                    if (!email.Contains("@"))
                    {
                        var trimmed = email.Trim();
                        var valueArray = trimmed.Split(new[] {" "}, StringSplitOptions.None);
                        var user = new User();
                        // TBD : We assume that first the given name is provided and then the surname with ' ' in between
                        if (valueArray.Length > 1)
                        {
                            user.GivenName = valueArray[0];
                            user.Surname = valueArray[1];
                        }
                        // TBD : Refactoring needed. For now we assume if there is one name it is the surname due to Japanese 
                        else
                        {
                            // TBD : Abstract that via configuration or branch logic in order to keep it generic
                            user.Surname = valueArray[0].Replace("さん", string.Empty).Replace("ー", string.Empty).Replace("-", string.Empty)
                                .Replace(" ", string.Empty);
                        }
                        var persons = await _peopleService.GetPeolpe(new List<User> {user}, accessToken);
                        if (persons == null || persons.Count <= 0) { continue; }
                        var emailAddress = persons.FirstOrDefault()?.Mail;
                        emailList.Add(emailAddress);
                    }
                    else
                    {
                        var improvedEmail = email.Replace(" ", "").Replace("　", "");
                        if (improvedEmail.ToLower().Trim().StartsWith("dl-"))
                        {
                            var groupId =
                                await _groupService.GetGroupId(improvedEmail.Replace("dl-", string.Empty), accessToken);
                            if (string.IsNullOrEmpty(groupId))
                            {
                                throw new ApplicationException("Can't get group id.");
                            }
                            var members = await _groupService.GetMembers(groupId, accessToken);
                            emailList.AddRange(members.Select(x => x.Mail));
                        }
                        else
                        {
                            emailList.Add(improvedEmail);
                        }
                    }
                }
               
                return emailList;
            }
            catch (Exception e)
            {
                _loggingService.Error(e, "Error in EmailService.GetEmails");
                throw;
            }

        }
    }
}