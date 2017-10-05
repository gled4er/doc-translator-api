using System;
using System.Collections.Generic;

namespace MicrosoftGraph.Model
{
    /// <summary>
    /// Room List Response
    /// </summary>
    [Serializable]
    public class RoomsListResponse
    {
        /// <summary>
        /// List of rooms 
        /// </summary>
        public List<Room> Value { get; set; }

    }
}
