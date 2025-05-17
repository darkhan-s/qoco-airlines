namespace Qoco_Airlines.Models
{
    public class Flight
    {
        public int Id { get; set; }
        public string Registration { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string FlightNumber { get; set; } = string.Empty;
        public string DepartureAirport { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public string ArrivalAirport { get; set; } = string.Empty;
        public DateTime ArrivalTime { get; set; }
    }
}
