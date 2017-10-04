using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph;
using MicrosoftGraph.Model;

namespace MicrosoftGraph.Services
{
    /// <summary>
    /// Room Service 
    /// </summary>
    [Serializable]
    public class RoomService : IRoomService
    {
        private readonly IOutlookService _outlookService;
        private readonly ILoggingService _loggingService;

        /// <summary>
        /// Room service constructor
        /// </summary>
        /// <param name="outlookService">Instance of <see cref="IOutlookService"/></param>
        /// <param name="loggingService">Instance of <see cref="ILoggingService"/></param>
        public RoomService(IOutlookService outlookService, ILoggingService loggingService)
        {
            _outlookService = outlookService;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Get all rooms 
        /// </summary>
        /// <returns>List of all rooms</returns>
        public async Task<List<Room>> GetRooms(AutoAuthConfiguration autoAuthConfiguration)
        {
            try
            {
                var rooms = await _outlookService.GetRooms(autoAuthConfiguration);
                return rooms;
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