using System.Collections.Generic;
using Microsoft.Graph;

namespace MicrosoftGraph.Model
{
    /// <summary>
    /// Group Member Response
    /// </summary>
    public class GroupMemberResponse
    {
        /// <summary>
        /// List of users
        /// </summary>
        public  List<User> Value { get; set; }
    }
}