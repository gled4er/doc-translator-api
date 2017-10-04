using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Office365.OutlookServices;
using MicrosoftGraph.Model;

namespace MicrosoftGraph.Services
{
    public interface IOutlookService
    {
        /// <summary>
        /// Get all rooms
        /// </summary>
        /// <returns>List of <see cref="Room"/></returns>
        Task<List<Room>> GetRooms(AutoAuthConfiguration autoAuthConfiguration);

        /// <summary>
        /// Schedule an event 
        /// </summary>
        /// <param name="meeting">Instance of <see cref="Event"/></param>
        /// <returns></returns>
        Task ScheduleEvent(Event meeting, string organizerEmail, string organizerName, AutoAuthConfiguration autoAuthConfiguration);
    }
}
