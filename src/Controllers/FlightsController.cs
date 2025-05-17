// Controllers/FlightsController.cs
using Qoco_Airlines.Models;
using Qoco_Airlines.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlightQualityAnalyzer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightService _service;

        public FlightsController(IFlightService service)
        {
            _service = service;
        }

        // GET api/flights
        [HttpGet]
        public ActionResult<IEnumerable<Flight>> GetAll()
        {
            return Ok(_service.GetAllFlights());
        }

        // GET api/flights/inconsistencies
        [HttpGet("inconsistencies")]
        public ActionResult<IEnumerable<Inconsistency>> GetInconsistencies()
        {
            return Ok(_service.GetSequenceInconsistencies());
        }
    }
}
