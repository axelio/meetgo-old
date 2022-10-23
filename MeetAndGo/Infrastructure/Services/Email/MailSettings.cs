namespace MeetAndGo.Infrastructure.Services.Email
{
    public class MailSettings
    {
        public string User { get; set; }
        public string From { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string DisplayName { get; set; }
    }
}
