using FlightQualityAnalyzer.Controllers;
using Microsoft.Extensions.Logging;
using Qoco_Airlines.Models;
using Qoco_Airlines.Services;
using Xunit;

namespace Qoco_Airlines_Test
{
    public class FlightServiceTests
    {
        // Helper to build a service around an in‑memory flight list
        private static FlightService BuildService(IEnumerable<Flight> flights) =>
            new FlightService(flights);

        #region File fetching tests
        [Fact]
        public void CsvFile_ShouldLoadFlightsSuccessfully()
        {
            var svc = new FlightService();
            var flights = svc.GetAllFlights().ToList();

            Assert.NotEmpty(flights);

            Assert.All(flights, f => Assert.False(string.IsNullOrWhiteSpace(f.Registration)));
        }

        [Fact]
        public void FlightService_ShouldThrowWhenWrongFilePathProvided()
        {
            var wrongPath = "nonexistent/path/flights.csv";

            var ex = Assert.Throws<FileNotFoundException>(() =>
            {
                var svc = new FlightService(flightsFilePath: wrongPath);
            });

            Assert.Contains(wrongPath, ex.Message);
        }
        #endregion

        #region Tests for api/flights 
        [Fact]
        public void EmptyFlightList_ShouldReturnNoFlights_AndNoInconsistencies()
        {
            var svc = BuildService(new List<Flight>());
            Assert.Empty(svc.GetAllFlights());
            Assert.Empty(svc.GetSequenceInconsistencies());
        }

        [Fact]
        public void SingleFlight_ShouldReturnThatFlight_AndNoInconsistencies()
        {
            var flights = new[]
            {
                new Flight { Id = 1, Registration = "ABC", DepartureAirport = "HEL", ArrivalAirport = "LHR", DepartureTime = DateTime.Parse("2024-01-01T10:00:00"), ArrivalTime = DateTime.Parse("2024-01-01T12:00:00") }
            };
            var svc = BuildService(flights);

            var all = svc.GetAllFlights().ToList();
            Assert.Single(all);
            Assert.Equal("ABC", all[0].Registration);

            Assert.Empty(svc.GetSequenceInconsistencies());
        }
        #endregion

        #region Tests for api/flights/inconsistencies
        [Fact]
        public void TwoFlights_InCorrectSequence_ShouldProduceNoInconsistency()
        {
            var flights = new[]
            {
                new Flight { Id = 1, Registration = "XYZ", DepartureAirport = "JFK", ArrivalAirport = "AMS", DepartureTime = DateTime.Parse("2024-01-01T08:00:00"), ArrivalTime = DateTime.Parse("2024-01-01T16:00:00") },
                new Flight { Id = 2, Registration = "XYZ", DepartureAirport = "AMS", ArrivalAirport = "CDG", DepartureTime = DateTime.Parse("2024-01-01T18:00:00"), ArrivalTime = DateTime.Parse("2024-01-01T19:30:00") }
            };
            var svc = BuildService(flights);

            Assert.Empty(svc.GetSequenceInconsistencies());
        }

        [Fact]
        public void TwoFlights_MismatchedSequence_ShouldReportOneInconsistency()
        {
            var flights = new[]
            {
                new Flight { Id = 1, Registration = "XYZ", DepartureAirport = "JFK", ArrivalAirport = "AMS", DepartureTime = DateTime.Parse("2024-01-01T08:00:00"), ArrivalTime = DateTime.Parse("2024-01-01T16:00:00") },
                new Flight { Id = 2, Registration = "XYZ", DepartureAirport = "CDG", ArrivalAirport = "LHR", DepartureTime = DateTime.Parse("2024-01-01T18:00:00"), ArrivalTime = DateTime.Parse("2024-01-01T20:00:00") }
            };
            var svc = BuildService(flights);

            var inconsistencies = svc.GetSequenceInconsistencies().ToList();
            Assert.Single(inconsistencies);

            var inc = inconsistencies[0];
            Assert.Equal(2, inc.Flight.Id);
            Assert.Equal("AMS", inc.ExpectedNextDeparture);
        }

        [Fact]
        public void MultipleRegistrations_ShouldOnlyCompareWithinSameAircraft()
        {
            var flights = new[]
            {
                // Plane A: correct chain
                new Flight { Id = 1, Registration = "A", DepartureAirport = "HEL", ArrivalAirport = "LHR", DepartureTime = DateTime.Parse("2024-01-01T08:00:00"), ArrivalTime = DateTime.Parse("2024-01-01T10:00:00") },
                new Flight { Id = 2, Registration = "A", DepartureAirport = "LHR", ArrivalAirport = "CDG", DepartureTime = DateTime.Parse("2024-01-01T12:00:00"), ArrivalTime = DateTime.Parse("2024-01-01T14:00:00") },

                // Plane B: broken chain
                new Flight { Id = 3, Registration = "B", DepartureAirport = "SFO", ArrivalAirport = "SEA", DepartureTime = DateTime.Parse("2024-01-02T09:00:00"), ArrivalTime = DateTime.Parse("2024-01-02T11:00:00") },
                new Flight { Id = 4, Registration = "B", DepartureAirport = "LAX", ArrivalAirport = "PDX", DepartureTime = DateTime.Parse("2024-01-02T12:00:00"), ArrivalTime = DateTime.Parse("2024-01-02T14:00:00") },
            };
            var svc = BuildService(flights);

            var incs = svc.GetSequenceInconsistencies().ToList();
            Assert.Single(incs);
            Assert.Equal(4, incs[0].Flight.Id);
            Assert.Equal("SEA", incs[0].ExpectedNextDeparture);
        }

        #endregion   
    }
}
