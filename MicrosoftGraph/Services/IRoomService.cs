using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graph;
using MicrosoftGraph.Model;

namespace MicrosoftGraph.Services
{
    /// <summary>
    /// Room Service
    /// </summary>
    public interface IRoomService
    {
        /// <summary>
        /// Get rooms
        /// </summary>
        /// <returns>Task for list of <see cref="Room"/></returns>
        Task<List<Room>> GetRooms(string accessToken);

        /// <summary>
        /// Add rooms
        /// </summary>
        /// <param name="request">Request object</param>
        /// <param name="rooms">List of rooms</param>
        void AddRooms(UserFindMeetingTimesRequestBody request, List<Room> rooms);

    }
}
