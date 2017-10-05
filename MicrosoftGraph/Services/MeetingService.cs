using System;
using Microsoft.Graph;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Office = Microsoft.Office365.OutlookServices;

namespace MicrosoftGraph.Services
{
    /// <summary>
    /// Service responsible for scheduling meetings 
    /// </summary>
    [Serializable]
    public class MeetingService : IMeetingService
    {
        private const string FindsMeetingTimeEndpoint = "https://graph.microsoft.com/v1.0/me/findMeetingTimes";
        private const string ScheduleMeetingEndpoint = "https://graph.microsoft.com/v1.0/me/events";
        private readonly IRoomService _roomService;
        private readonly IHttpService _httpService;
        private readonly ILoggingService _loggingService;

        /// <summary>
        /// Meeting Service Constructor
        /// </summary>
        /// <param name="httpService">HTTP Service instance</param>
        // /// <param name="outlookService">Outlook Service</param>
        /// <param name="roomService">Room Service instance</param>
        /// <param name="loggingService">Logging Service</param>
        public MeetingService(IHttpService httpService, IRoomService roomService, ILoggingService loggingService)
        {
            _roomService = roomService;
            _httpService = httpService;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Provides meeting times suggestions
        /// </summary>
        /// <param name="accessToken">Access Token for API</param>
        /// <param name="userFindMeetingTimesRequestBody">Request object for calling Find Meeting Times API</param>
        /// <returns>Task of <see cref="MeetingTimeSuggestionsResult"/></returns>
        public async Task<MeetingTimeSuggestionsResult> GetMeetingsTimeSuggestions(string accessToken, UserFindMeetingTimesRequestBody userFindMeetingTimesRequestBody)
        {
            try
            {
                var rooms =  await _roomService.GetRooms(accessToken);
                _roomService.AddRooms(userFindMeetingTimesRequestBody, rooms);
                var httpResponseMessage = await _httpService.AuthenticatedPost(FindsMeetingTimeEndpoint, accessToken, userFindMeetingTimesRequestBody, string.Empty);
                var meetingTimeSuggestionsResult = JsonConvert.DeserializeObject<MeetingTimeSuggestionsResult>(await httpResponseMessage.Content.ReadAsStringAsync());
                return meetingTimeSuggestionsResult;
            }
            catch (Exception ex)
            {
                _loggingService.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// Schedules meeting
        /// </summary>
        /// <param name="accessToken">Access Token for API</param>
        /// <param name="meeting">Meeting object containing all required data for scheduling meeting</param>
        /// <returns>Task of <see cref="Event"/></returns>
        public async Task<Event> ScheduleMeeting(string accessToken, Event meeting)
        {
            try
            {
                var httpResponseMessage =
                    await _httpService.AuthenticatedPost(ScheduleMeetingEndpoint, accessToken, meeting, "UTC");
                var scheduledMeeting =
                    JsonConvert.DeserializeObject<Event>(await httpResponseMessage.Content.ReadAsStringAsync());
                return scheduledMeeting;
            }
            catch (Exception ex)
            {
                _loggingService.Error(ex);
                throw;
            }
        }
    }
}