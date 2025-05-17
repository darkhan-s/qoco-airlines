using Qoco_Airlines.Models;
using System.Collections.Generic;

namespace Qoco_Airlines.Services
{
    public interface IFlightService
    {
        IEnumerable<Flight> GetAllFlights();
        IEnumerable<Inconsistency> GetSequenceInconsistencies();
    }
}
