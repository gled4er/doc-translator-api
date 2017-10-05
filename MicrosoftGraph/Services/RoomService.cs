using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph;
using MicrosoftGraph.Model;
using Newtonsoft.Json;

namespace MicrosoftGraph.Services
{
    /// <summary>
    /// Room Service 
    /// </summary>
    [Serializable]
    public class RoomService : IRoomService
    {
        private readonly IHttpService _httpService;
        private readonly ILoggingService _loggingService;

        /// <summary>
        /// Room service constructor
        /// </summary>
        /// <param name="loggingService">Instance of <see cref="ILoggingService"/></param>
        /// <param name="httpService">Instance of <see cref="IHttpService"/></param>
        public RoomService(IHttpService httpService, ILoggingService loggingService)
        {
            _httpService = httpService;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Get all rooms 
        /// </summary>
        /// <param name="accessToken">User Acces Token</param>
        /// <returns>List of all rooms</returns>
        public async Task<List<Room>> GetRooms(string accessToken)
        {
            try
            {
                var httpResponseMessage = await _httpService.AuthenticatedGet("https://graph.microsoft.com/beta/me/findrooms", accessToken);
                var roomSearchResponse = JsonConvert.DeserializeObject<RoomsListResponse>(await httpResponseMessage.Content.ReadAsStringAsync());
                return roomSearchResponse != null ? roomSearchResponse.Value : new List<Room>();
            }
            catch(Exception ex)
            {
                _loggingService.Error(ex);
                throw;
            }
        }

        /// <summary>
        /// Add rooms to meeting time suggestion request
        /// </summary>
        /// <param name="request">Meeting time suggestion request</param>
        /// <param name="rooms">List of rooms</param>
        public void AddRooms(UserFindMeetingTimesRequestBody request, List<Room> rooms)
        {
            try
            {
                var attendees = request.Attendees as List<Attendee>;
                attendees?.AddRange(rooms.Select(room => new Attendee()
                {
                    EmailAddress = new EmailAddress()
                    {
                        Address = room.Address,
                        Name = room.Name
                    },
                    Type = AttendeeType.Optional
                }));
            }
            catch (Exception ex)
            {
                _loggingService.Error(ex);
                throw;
            }
           
        }
    }
}