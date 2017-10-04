using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace MicrosoftGraph.Model
{
    [Serializable]
    public class MeetingMetadata
    {
        /// <summary>
        /// Meeting's Subject
        /// </summary>
        public string Subject { get; set; } = "Meeting";

        /// <summary>
        /// List of attendees' names
        /// </summary>
        public List<string> Attendees { get; set; } = new List<string>();

        /// <summary>
        /// Date for meeting
        /// </summary>
        public string Date { get; set; } = DateTime.Now.ToShortDateString();
        
        /// <summary>
        /// Duration of meeting in seconds
        /// </summary>
        public string  Duration { get; set; } = "1800";

        public string[] Emails { get; set; }

        public DateTime MeetingSelectedStartTimeDatetime { get; set; }

        public DateTime MeetingSelectedEndTimeDatetime { get; set; }

        public string DurationInMinutes { get; set; }

        public string DisplaySchedule { get; set; }

        public Room SelectedRoom { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}