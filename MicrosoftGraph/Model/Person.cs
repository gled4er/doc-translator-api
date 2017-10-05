using System;

namespace MicrosoftGraph.Model
{
    /// <summary>
    /// Person object 
    /// </summary>
    [Serializable]
    public class Person
    {
        /// <summary>
        /// Person's display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Email address  
        /// </summary>
        public string Mail { get; set; }
    }
}