﻿using System;
using TestApp.Authentication;
using Autofac;
using MicrosoftGraph.Services;
using MicrosoftGraph.Util;
using DocumentManagement.Services;
using System.Configuration;
    
namespace TestApp
{
    class Program
    {
        private static IContainer Container { get; set; }

        static void Main(string[] args)
        {

            var containerBuilder = new ContainerBuilder();

            #region Dependency Injection Setup 

            containerBuilder.Register<ILoggingService>(b => new LoggingService());
            containerBuilder.Register<IStorageManagementService>(b=>new StorageManagementService(b.Resolve<ILoggingService>()));
            containerBuilder.Register<ISharePointManagementService>(b => new SharePointManagementService(b.Resolve<ILoggingService>()));
            containerBuilder.Register<IHttpService>(b => new HttpService(b.Resolve<ILoggingService>()));
            containerBuilder.Register<IDocumentManagementService>(b=> new DocumentManagementService(b.Resolve<IStorageManagementService>(), b.Resolve<ISharePointManagementService>(), b.Resolve<ILoggingService>()));
            containerBuilder.Register<IRoomService>(b => new RoomService(b.Resolve<IHttpService>(), b.Resolve<ILoggingService>()));
            containerBuilder.Register<IGroupService>(b => new GroupService(b.Resolve<IHttpService>(), b.Resolve<ILoggingService>()));
            containerBuilder.Register<IMeetingService>(b => new MeetingService(b.Resolve<IHttpService>(), b.Resolve<IRoomService>(), b.Resolve<ILoggingService>()));
            containerBuilder.Register<IPeopleService>(b => new PeopleService(b.Resolve<IHttpService>(), b.Resolve<ILoggingService>()));
            containerBuilder.Register<IEmailService>(b => new EmailService(b.Resolve<IGroupService>(), b.Resolve<IPeopleService>(), b.Resolve<ILoggingService>()));
            Container = containerBuilder.Build();

            #endregion

            using (var scope = Container.BeginLifetimeScope())
            {
                // Authenticate 
                var userAccessToken =  AuthenticationHelper.GetTokenForUser(ConfigurationManager.AppSettings["AADTenant"], ConfigurationManager.AppSettings["AADAppClientID"]).Result;

                Console.WriteLine("Authentication Successful!");

                // Find emails by name
                var emailService = scope.Resolve<IEmailService>();

                // using distributions list
                var emails = emailService.GetEmails("Naomi Sato,jbrown@smdocs.onmicrosoft.com", userAccessToken).Result;

                Console.WriteLine("Email retrieved");
                foreach(var email in emails)
                {
                    Console.WriteLine(email);
                }

                // Provide Meeting Slots options by date
                var roomsService = scope.Resolve<IRoomService>();

                var rooms = roomsService.GetRooms(userAccessToken).Result;
                Console.WriteLine("Rooms Retrieved");
                foreach(var roomItem in rooms)
                {
                    Console.WriteLine($"{roomItem.Name}-{roomItem.Address}");
                }

                var roomsDictionary = DataConverter.GetRoomDictionary(rooms);

                var meetingService = scope.Resolve<IMeetingService>();
                var meetingDuration = 30;
                var date = DateTime.Now.AddDays(5);

                var userFindMeetingTimesRequestBody = DataConverter.GetUserFindMeetingTimesRequestBody(date, emails.ToArray(), normalizedDuration: meetingDuration, isOrganizerOptional: false);
                var meetingTimeSuggestion = meetingService.GetMeetingsTimeSuggestions(userAccessToken, userFindMeetingTimesRequestBody).Result;
                var meetingScheduleSuggestions = DataConverter.GetMeetingScheduleSuggestions(meetingTimeSuggestion, roomsDictionary);
                Console.WriteLine("Meeting suggestion retrieved");
                foreach (var meetingSuggestion in meetingScheduleSuggestions)
                {
                    Console.WriteLine(meetingSuggestion.Time);
                }
                // Select meeting slot and room
                var fileName = "AI05.pptx";

                var randomNumberGenerator = new Random();
                var slotIndex = randomNumberGenerator.Next(meetingScheduleSuggestions.Count);
                var slot = meetingScheduleSuggestions[slotIndex];
                var roomIndex = randomNumberGenerator.Next(meetingScheduleSuggestions[slotIndex].Rooms.Count);
                var room = slot.Rooms[roomIndex];

                Console.WriteLine($"Selected slot - {slot}");
                Console.WriteLine($"Selected room - {room.Name}");

                // Get document links
                var documentManagementService = scope.Resolve<IDocumentManagementService>();

                Console.WriteLine("Translation from Japanese to English and uploading to SharePoint for document {0} started", fileName);

                var documentLinks = documentManagementService.TranslateFile("documents", fileName, "Japanese", "English");

                Console.WriteLine("Document {0} translated from Japanese to English", fileName);

                // Schedule meeting 
                var meeting = DataConverter.GetEvent(room, emails.ToArray(), $"Discussion for document {fileName}", slot.StartTime, slot.EndTime, documentLinks.OriginalDocument, documentLinks.TranslatedDocument);
                var scheduledEvent = meetingService.ScheduleMeeting(userAccessToken, meeting).Result;

                Console.WriteLine("Meeting scheduled");

                Console.ReadLine();
            }
        }
    }
}
