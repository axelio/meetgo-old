namespace MeetAndGo.Data.Models
{
    public class CompanySettings
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public int MaxDailyVisits { get; set; }
        public User Company { get; set; }
        public string UserId { get; set; }
    }
}