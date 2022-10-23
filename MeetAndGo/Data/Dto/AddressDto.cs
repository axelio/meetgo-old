namespace MeetAndGo.Data.Dto
{
    public class AddressDto
    {
        public string Street { get; set; }
        public string Number { get; set; }
        public string CompanyName { get; set; }
        public string District { get; set; }
        public string Website { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
    }
}