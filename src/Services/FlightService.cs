using CsvHelper;
using CsvHelper.Configuration;
using FlightQualityAnalyzer.Controllers;
using Microsoft.Extensions.Logging.Abstractions;
using Qoco_Airlines.Models;
using Qoco_Airlines.Services;
using System.Globalization;
using System.IO;

namespace Qoco_Airlines.Services
{
    public class FlightService : IFlightService
    {
        private readonly List<Flight> _flights = [];
        private const string DefaultFlightsFileName = "Data/flights.csv";
        private readonly ILogger<FlightsController> _logger;

        public FlightService(IEnumerable<Flight>? flights = null,
                             string? flightsFilePath = null,
                             ILogger<FlightsController>? logger = null)
        {
            _logger = logger ?? NullLogger<FlightsController>.Instance;

            if (flights is not null)
            {
                _flights = flights.ToList();
            }
            else
            {
                var path = flightsFilePath ?? Path.Combine(AppContext.BaseDirectory, DefaultFlightsFileName);

                if (!File.Exists(path))
                {
                    _logger.LogError($"Flight data file not found at path: {path}");
                    throw new FileNotFoundException($"Flight data file not found: {path}");
                }

                try
                {
                    using var reader = new StreamReader(path);
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = true
                    };

                    using var csv = new CsvReader(reader, config);
                    csv.Context.RegisterClassMap<FlightMap>();
                    _flights = csv.GetRecords<Flight>().ToList();

                    _logger.LogInformation($"Successfully loaded {_flights.Count} flight rows from file {path}.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to load flights from CSV file at path: {path}");
                    throw;
                }
            }
        }


        public IEnumerable<Flight> GetAllFlights()
        {
            _logger.LogInformation($"Retrieving all flights. Total count: {_flights.Count}" );
            return _flights; 
        }

        /// <summary>
        /// Finds inconsistencies where the arrival airport of a flight does not match the departure airport
        /// of the subsequent flight for the same aircraft registration, ordered by departure time.
        /// </summary>
        public IEnumerable<Inconsistency> GetSequenceInconsistencies()
        {
            var inconsistencies = new List<Inconsistency>();

            if (_flights == null)
            {
                _logger.LogError("Flight data is null during inconsistency check.");
                throw new ArgumentNullException($"Flight data cannot be null");
            }

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
                        _logger.LogWarning($"Inconsistency found: Flight {current.FlightNumber} arrives at {current.ArrivalAirport}, but next flight {next.FlightNumber} departs from {next.DepartureAirport}.");

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
