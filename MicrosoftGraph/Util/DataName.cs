namespace SampleAADv2Bot.Util
{
    /// <summary>
    /// Class abstracting variables we are using for storing user private data
    /// </summary>
    public class DataName
    {
        public static string UserEmailString => "userEmail";
        public static string UserNameString => "userName";
        public static string MeeintingSubjectString => "meeintingSubject";
        public static string MeetingInvitationsNumInt => "meetingInvitationsNum";
        public static string MeetingDurationInt => "meetingDuration";
        public static string InvitationsEmailsStringArray => "InvitationsEmails";
        public static string MeetingSelectedDateDatetime => "meetingSelectedDate";
        public static string MeetingSelectedDateDatetimeOffset => "meetingSelectedDateDatetimeOffset";
        public static string MeetingSelectedStartTimeDatetime => "meetingSelectedStartTime";
        public static string MeetingSelectedEndTimeDatetime => "meetingSelectedEndTime";
        public static string MeetingSelectedRoomRoom => "meetingSelectedRoom";
        public static string MeetingMetadata => "meetingMetadata";
        public static string MeetingIsAuthRequired => "meeitngAuthRequired";

    }
}