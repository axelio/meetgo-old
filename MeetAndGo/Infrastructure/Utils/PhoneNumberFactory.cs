namespace MeetAndGo.Infrastructure.Utils
{
    public static class PhoneNumberFactory
    {
        public static string CreatePolishNumber(string number) => $"+48{number}";
    }
}