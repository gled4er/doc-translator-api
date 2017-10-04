﻿using System.Threading.Tasks;
using Microsoft.Graph;

namespace MicrosoftGraph.Services
{
    /// <summary>
    /// Meeting Service 
    /// </summary>
    public interface IMeetingService
    {
        /// <summary>
        /// Get meeting time suggestions
        /// </summary>
        /// <param name="accessToken">Access token for underlying API</param>
        /// <param name="userFindMeetingTimesRequestBody">Request object</param>
        /// <returns>Task of <see cref="MeetingTimeSuggestionsResult"/></returns>
        Task<MeetingTimeSuggestionsResult> GetMeetingsTimeSuggestions(string accessToken, UserFindMeetingTimesRequestBody userFindMeetingTimesRequestBody, Model.AutoAuthConfiguration autoAuthConfiguration);

        /// <summary>
        /// Schedule a meeting
        /// </summary>
        /// <param name="accessToken">Access token for underlying API</param>
        /// <param name="meeting">Request object for scheduling meeting</param>
        /// <returns>Task of <see cref="Event"/></returns>
        Task<Event> ScheduleMeeting(string accessToken, Event meeting);

        /// <summary>
        /// Schedule a meeting
        /// </summary>
        /// <param name="meeting">Request object for scheduling meeting</param>
        /// <returns>Task</returns>
        Task ScheduleOutlokMeeting(Microsoft.Office365.OutlookServices.Event meeting, string organizerEmail, string organizerName, Model.AutoAuthConfiguration autoAuthConfiguration);
    }
}
