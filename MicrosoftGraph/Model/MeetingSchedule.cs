using System;
using System.Collections.Generic;

namespace MicrosoftGraph.Model
{
    /// <summary>
    /// Meeting Scheduler
    /// </summary>
    [Serializable]
    public class MeetingSchedule
    {
        /// <summary>
        /// String representing time
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// Start time
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// End time
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// List of rooms
        /// </summary>
        public List<RoomRecord> Rooms { get; set; }

        /// <summary>
        /// Custom ToString method to show time string 
        /// </summary>
        /// <returns>Time of Meeting Schedule</returns>
        public override string ToString()
        {
            return Time;
        }
    }
}