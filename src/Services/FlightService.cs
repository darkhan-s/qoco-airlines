using CsvHelper;
using CsvHelper.Configuration;
using Qoco_Airlines.Models;
using Qoco_Airlines.Services;
using System.Globalization;

namespace Qoco_Airlines.Services
{
    public class FlightService : IFlightService
    {
        private readonly List<Flight> _flights;
        public FlightService(IEnumerable<Flight> flights)
        {
            _flights = flights.ToList();
        }
        public FlightService()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Data", "flights.csv");

            using var reader = new StreamReader(path);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            };

            using var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<FlightMap>();
            _flights = csv.GetRecords<Flight>().ToList();
        }

        public IEnumerable<Flight> GetAllFlights() => _flights;

        public IEnumerable<Inconsistency> GetSequenceInconsistencies()
        {
            var inconsistencies = new List<Inconsistency>();

            var grouped = _flights.GroupBy(f => f.Registration);

            foreach (var group in grouped)
            {
                var ordered = group.OrderBy(f => f.DepartureTime).ToList();
                for (int i = 0; i < ordered.Count - 1; i++)
                {
                    var current = ordered[i];
                    var next = ordered[i + 1];
                    if (!string.Equals(current.ArrivalAirport, next.DepartureAirport, StringComparison.OrdinalIgnoreCase))
                    {
                        inconsistencies.Add(new Inconsistency
                        {
                            Flight = next,
                            ExpectedNextDeparture = current.ArrivalAirport
                        });
                    }
                }
            }

            return inconsistencies;
        }
    }
}
