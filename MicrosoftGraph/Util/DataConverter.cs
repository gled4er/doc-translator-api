using System;
using System.Collections.Generic;
using Microsoft.Graph;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using MicrosoftGraph.Model;

namespace SampleAADv2Bot.Util
{
    /// <summary>
    /// Data Converter helper class 
    /// </summary>
    public class DataConverter
    {
        /// <summary>
        /// Get meeting rooms
        /// </summary>
        /// <param name="timeSuggestion"></param>
        /// <param name="roomsDictionary"></param>
        /// <returns>List of available rooms</returns>
        public static List<RoomRecord> GetMeetingSuggestionRooms(MeetingTimeSuggestion timeSuggestion, Dictionary<string, string> roomsDictionary)
        {
            var rooms = new List<RoomRecord>();
            if (timeSuggestion?.AttendeeAvailability == null || !timeSuggestion.AttendeeAvailability.Any() || roomsDictionary == null || !roomsDictionary.Any())
            {
                return rooms;
            }

            var counter = 1;
            foreach(var attendee in timeSuggestion.AttendeeAvailability)		
             {
                 if (!roomsDictionary.ContainsKey(attendee.Attendee.EmailAddress.Address)) continue;
                 rooms.Add(new RoomRecord()
                 {
                     Address = attendee.Attendee.EmailAddress.Address,
                     Name = roomsDictionary[attendee.Attendee.EmailAddress.Address],
                     Counter =  counter
                 });
                 counter++;
             }		
               
            return rooms;
        }

        ///// <summary>
        ///// Get authentication options 
        ///// </summary>
        ///// <returns><see cref="AuthenticationOptions" /></returns>
        //public static AuthenticationOptions GetAuthenticationOptions()
        //{
        //    var options = new AuthenticationOptions()
        //    {
        //        Authority = ConfigurationManager.AppSettings["aad:Authority"],
        //        ClientId = ConfigurationManager.AppSettings["aad:ClientId"],
        //        ClientSecret = ConfigurationManager.AppSettings["aad:ClientSecret"],
        //        Scopes = new [] { "User.Read", "Calendars.ReadWrite", "Calendars.ReadWrite.Shared","People.Read", "User.ReadBasic.All"},
        //        RedirectUrl = ConfigurationManager.AppSettings["aad:Callback"]
        //    };

        //    return options;
        //}

        /// <summary>
        /// Get request object for find meeting times API
        /// </summary>
        /// <param name="date">String representation of date</param>
        /// <param name="normalizedEmails">List of participants emails</param>
        /// <param name="normalizedDuration">Duration of the meeting</param>
        /// <param name="isOrganizerOptional">Marks if organizer is optional</param>
        /// <returns><see cref="UserFindMeetingTimesRequestBody" /></returns>
        public static UserFindMeetingTimesRequestBody GetUserFindMeetingTimesRequestBody(DateTime date, string[] normalizedEmails, int normalizedDuration, bool isOrganizerOptional = false)
        {
            var startDate = $"{date.Year:D4}-{date.Month:D2}-{date.Day:D2}T00:00:00.000Z";
            var endDate = $"{date.Year:D4}-{date.Month:D2}-{date.Day:D2}T10:00:00.000Z";
            var inputAttendee = normalizedEmails.Select(i => new Attendee()
                {
                    EmailAddress = new EmailAddress()
                    {
                        Address = i
                    }
                })
                .ToList();

            var inputDuration = new Duration(TimeSpan.FromMinutes(normalizedDuration));


            var userFindMeetingTimesRequestBody = new UserFindMeetingTimesRequestBody()
            {
                Attendees = inputAttendee,
                TimeConstraint = new TimeConstraint()
                {
                    Timeslots = new List<TimeSlot>()
                        {
                            new TimeSlot()
                            {
                                Start = new DateTimeTimeZone()
                                {
                                    DateTime = startDate,
                                    TimeZone = "UTC"
                                },
                                End = new DateTimeTimeZone()
                                {
                                    DateTime = endDate,
                                    TimeZone = "UTC"
                                }
                            }
                        }
                },
                MeetingDuration = inputDuration,
                MaxCandidates = 15,
                IsOrganizerOptional = isOrganizerOptional,
                ReturnSuggestionReasons = true,
                MinimumAttendeePercentage = 100

            };

            return userFindMeetingTimesRequestBody;

        }

        public static string ParseSpaceCharacterFromSkype(string input)
        {
            return input.Replace("&#160;", " ").Replace("&#160:^", " ");

        }

        public static UserFindMeetingTimesRequestBody GetUserFindMeetingTimesRequestBody(DateTimeOffset date, string[] normalizedEmails, int normalizedDuration, bool isOrganizerOptional)
        {
            var startDate = date;

            // TBD: should be input by user
            var dateDuration = new TimeSpan(1, 0, 0, 0);

            var jstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            var endDate = new DateTimeOffset(date.AddDays(dateDuration.Days).Year, date.AddDays(dateDuration.Days).Month, date.AddDays(dateDuration.Days).Day, 0, 0, 0, jstTimeZoneInfo.BaseUtcOffset);            

            var startDateQuery = $"{startDate.UtcDateTime.Year:D4}-{startDate.UtcDateTime.Month:D2}-{startDate.UtcDateTime.Day:D2}" +
                                 $"T{startDate.UtcDateTime.Hour:D2}:{startDate.UtcDateTime.Minute:D2}:{startDate.UtcDateTime.Second:D2}.{startDate.UtcDateTime.Millisecond:D3}Z";
            var endDateQuery = $"{endDate.UtcDateTime.Year:D4}-{endDate.UtcDateTime.Month:D2}-{endDate.UtcDateTime.Day:D2}" +
                               $"T{endDate.UtcDateTime.Hour:D2}:{endDate.UtcDateTime.Minute:D2}:{endDate.UtcDateTime.Second:D2}.{endDate.UtcDateTime.Millisecond:D3}Z";

            var inputAttendee = normalizedEmails.Select(i => new Attendee()
                {
                    EmailAddress = new EmailAddress()
                    {
                        Address = i
                    }
                })
                .ToList();

            var inputDuration = new Duration(TimeSpan.FromMinutes(normalizedDuration));


            var userFindMeetingTimesRequestBody = new UserFindMeetingTimesRequestBody()
            {
                Attendees = inputAttendee,
                TimeConstraint = new TimeConstraint()
                {
                    Timeslots = new List<TimeSlot>()
                    {
                        new TimeSlot()
                        {
                            Start = new DateTimeTimeZone()
                            {
                                DateTime = startDateQuery,
                                TimeZone = "UTC"
                            },
                            End = new DateTimeTimeZone()
                            {
                                DateTime = endDateQuery,
                                TimeZone = "UTC"
                            }
                        }
                    }
                },
                MeetingDuration = inputDuration,
                MaxCandidates = 15,
                IsOrganizerOptional = isOrganizerOptional,
                ReturnSuggestionReasons = true,
                MinimumAttendeePercentage = 100

            };

            return userFindMeetingTimesRequestBody;

        }

        /// <summary>
        /// Get event request object for scheduling a meeting 
        /// </summary>
        /// <param name="selectedRoom">Selected room</param>
        /// <param name="normalizedEmails">List of participant emails</param>
        /// <param name="subject">Name of the meeting</param>
        /// <param name="startTime">Starting time</param>
        /// <param name="endTime">End time</param>
        /// <returns><see cref="Event" /></returns>
        public static Event GetEvent(Room selectedRoom, string[] normalizedEmails, string subject, DateTime startTime, DateTime endTime)
        {
            var attendees = normalizedEmails.Select(email => new Attendee
                {
                    EmailAddress = new EmailAddress()
                    {
                        Address = email
                    }
                })
                .ToList();
            attendees.Add(new Attendee()
            {
                EmailAddress = new EmailAddress()
                {
                    Name = selectedRoom.Name,
                    Address = selectedRoom.Address
                }
            });

            var meeting = new Event()
            {
                Subject = subject,
                Start = new DateTimeTimeZone()
                {
                    DateTime = startTime.ToString(CultureInfo.InvariantCulture),
                    TimeZone = "UTC"
                },
                End = new DateTimeTimeZone()
                {
                    DateTime = endTime.ToString(CultureInfo.InvariantCulture),
                    TimeZone = "UTC"
                },
                Location = new Location()
                {
                    DisplayName = selectedRoom.Name,
                    LocationEmailAddress = selectedRoom.Address
                },
                Attendees = attendees
            };

            return meeting;
        }

        /// <summary>
        /// Format meeting date-time details in friendlier format
        /// </summary>
        /// <param name="startTime">Start time</param>
        /// <param name="endTime">End time</param>
        /// <param name="timeOffset">Time offset</param>
        /// <param name="counter">Optional counter for better UI in Skype for Business</param>
        /// <returns>Friendly string of date & time of the meeting</returns>
        public static string GetFormatedTime(DateTime startTime, DateTime endTime, int? counter, int timeOffset = 9)
        {
            var formattedTime = counter.HasValue == false ? 
                $"{startTime.AddHours(timeOffset):yyyy-MM-dd} -  {startTime.AddHours(timeOffset).ToShortTimeString()}  - {endTime.AddHours(9).ToShortTimeString()}" : 
                $"<b>{counter}:</b> {startTime.AddHours(timeOffset):yyyy-MM-dd} -  {startTime.AddHours(timeOffset).ToShortTimeString()}  - {endTime.AddHours(9).ToShortTimeString()}";
            return formattedTime;
        }

        /// <summary>
        /// Setting culture for a specified method
        /// </summary>
        /// <param name="culture">Selected culture for localization. Instance of <see cref="Culture"/></param>
        public static void SetCulture(Culture culture)
        {
            var cultureValue = string.Empty;
            switch (culture)
            {
                case Culture.English:
                    cultureValue = "en-US";
                    break;
                case Culture.Japanese:
                    cultureValue = "ja-JP";
                    break;
            }
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(cultureValue);
        }

        /// <summary>
        /// Convert duration from seconds string to minutes int 
        /// </summary>
        /// <param name="durationValue"></param>
        /// <returns>Duration in minutes</returns>
        public static int GetDurationInMinutesInt(string durationValue)
        {
            int.TryParse(durationValue, out int duration);
            var durationInMinutes = duration / 60;
            return durationInMinutes;
        }

        /// <summary>
        /// Modify text to follow Title Case formatting
        /// </summary>
        /// <param name="name">Input text</param>
        /// <returns></returns>
        public static string ToTitleCase(string name)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
        }

        /// <summary>
        /// Provide room dictionary where the key is room email address and value is room name
        /// </summary>
        /// <param name="rooms">List of <see cref="Room"/></param>
        /// <returns>Dictionary of rooms, key - email address and value is room name</returns>
        public static Dictionary<string, string> GetRoomDictionary(List<Room> rooms)
        {
            return rooms.ToDictionary(room => room.Address, room => room.Name);
        }

        /// <summary>
        /// Get event request object for scheduling a meeting 
        /// </summary>
        /// <param name="selectedRoom">Selected room</param>
        /// <param name="normalizedEmails">List of participant emails</param>
        /// <param name="subject">Name of the meeting</param>
        /// <param name="startTime">Starting time</param>
        /// <param name="endTime">End time</param>
        /// <returns><see cref="Event" /></returns>
        public static Microsoft.Office365.OutlookServices.Event GetOutlookEvent(Room selectedRoom, string[] normalizedEmails, string subject, DateTime startTime, DateTime endTime)
        {
            var attendees = normalizedEmails.Select(email => new Microsoft.Office365.OutlookServices.Attendee
                {
                    EmailAddress = new Microsoft.Office365.OutlookServices.EmailAddress()
                    {
                        Address = email
                    }
                })
                .ToList();
            attendees.Add(new Microsoft.Office365.OutlookServices.Attendee()
            {
                EmailAddress = new Microsoft.Office365.OutlookServices.EmailAddress()
                {
                    Name = selectedRoom.Name,
                    Address = selectedRoom.Address
                }
            });

            var meeting = new Microsoft.Office365.OutlookServices.Event
            {
                Start = new Microsoft.Office365.OutlookServices.DateTimeTimeZone()
                {
                    DateTime = startTime.ToString(CultureInfo.InvariantCulture),
                    TimeZone = "UTC"
                },
                End = new Microsoft.Office365.OutlookServices.DateTimeTimeZone
                {
                    DateTime = endTime.ToString(CultureInfo.InvariantCulture),
                    TimeZone = "UTC"
                },
                Subject = subject,
                Attendees = attendees
            };
            return meeting;
        }

        /// <summary>
        /// Get authentication configuration for the project
        /// </summary>
        /// <returns>Instance of <see cref="AuthenticationConfiguration"/></returns>
        public static AuthenticationConfiguration GetAuthenticationConfiguration(string authenticationMode)
        {
            var authenticationConfiguration = AuthenticationConfiguration.All;
            var authenticationValue = authenticationMode;
            switch (authenticationValue)
            {
                case "All":
                    authenticationConfiguration = AuthenticationConfiguration.All;
                    break;
                case "Manual":
                    authenticationConfiguration = AuthenticationConfiguration.Manual;
                    break;
                case "Auto":
                    authenticationConfiguration = AuthenticationConfiguration.Auto;
                    break;
            }

            return authenticationConfiguration;
        }
    }
}