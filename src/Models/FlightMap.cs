using CsvHelper.Configuration;

namespace Qoco_Airlines.Models
{
    // CSV mapping
    public sealed class FlightMap : ClassMap<Flight>
    {
        public FlightMap()
        {
            Map(m => m.Id).Name("id");
            Map(m => m.Registration).Name("aircraft_registration_number");
            Map(m => m.Type).Name("aircraft_type");
            Map(m => m.FlightNumber).Name("flight_number");
            Map(m => m.DepartureAirport).Name("departure_airport");
            Map(m => m.DepartureTime).Name("departure_datetime");
            Map(m => m.ArrivalAirport).Name("arrival_airport");
            Map(m => m.ArrivalTime).Name("arrival_datetime");
        }
    }
}
