using System.Collections.Generic;

namespace MicrosoftGraph.Model
{
    /// <summary>
    /// Response object for Find Room Outlook API
    /// </summary>
    public class FindRoomResponse
    {
        /// <summary>
        /// List of Rooms
        /// </summary>
        public List<Room> Value { get; set; }
    }
}