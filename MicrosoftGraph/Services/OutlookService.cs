using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Office365.OutlookServices;
using Newtonsoft.Json;
using MicrosoftGraph.Model;

namespace MicrosoftGraph.Services
{
    public class OutlookService : IOutlookService
    {

        private readonly IHttpService _httpService;
        private readonly ILoggingService _loggingService;
        private readonly ITokenService _tokenService;

        private const string RoomEndpoint = "https://outlook.office.com/api/beta/me/findrooms";
        private const string EventEndpoint = "https://outlook.office.com/api/v2.0";

        /// <summary>
        /// Constructor for Outlook Service
        /// </summary>
        /// <param name="httpService">Instance of <see cref="IHttpService"/></param>
        /// <param name="tokenService">Instance of <see cref="ITokenService"/></param>
        /// <param name="loggingService">Instance of <see cref="ILoggingService"/></param>
        public OutlookService(IHttpService httpService, ITokenService tokenService,  ILoggingService loggingService)
        {
            _httpService = httpService;
            _tokenService = tokenService;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Get all rooms
        /// </summary>
        /// <returns>List of <see cref="Room"/></returns>
        public async Task<List<Room>> GetRooms(AutoAuthConfiguration autoAuthConfiguration)
        {
            try
            {
                var token = await _tokenService.GetAccessToken(TokenProvider.Outlook, autoAuthConfiguration);
                var httpResponseMessage = await _httpService.AuthenticatedGet(RoomEndpoint, token);
                var content = await httpResponseMessage.Content.ReadAsStringAsync();
                var findRoomResponse = JsonConvert.DeserializeObject<FindRoomResponse>(content);
                return findRoomResponse?.Value;
            }
            catch (Exception ex)
            {
                _loggingService.Error("Error in OutlookService.GetRooms", ex);
                throw;
            }
        }


        /// <summary>
        /// Schedule an event 
        /// </summary>
        /// <param name="meeting">Instance of <see cref="Event"/></param>
        /// <returns></returns>
        public async Task ScheduleEvent(Event meeting, string organizerEmail, string organizerName, AutoAuthConfiguration autoAuthConfiguration)
        {
            try
            {
                var organizer = new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = organizerEmail,
                        Name = organizerName
                    }
                };
                meeting.IsOrganizer = false;
                meeting.Organizer = organizer;
                var outlookServicesClient = new OutlookServicesClient(new Uri(EventEndpoint), () => _tokenService.GetAccessToken(TokenProvider.Outlook, autoAuthConfiguration));
                await outlookServicesClient.Me.Events.AddEventAsync(meeting);
            }
            catch (Exception ex)
            {
                _loggingService.Error("Error in OutlookService.ScheduleEvent", ex);
                throw;
            }
        }

    }
}