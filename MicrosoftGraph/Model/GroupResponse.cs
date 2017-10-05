using System.Collections.Generic;
using Microsoft.Graph;

namespace MicrosoftGraph.Model
{
    /// <summary>
    /// Group Response 
    /// </summary>
    public class GroupResponse
    {
        /// <summary>
        /// List of groups
        /// </summary>
        public  List<Group> Value { get; set; }
    }

}