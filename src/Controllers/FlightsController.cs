// Controllers/FlightsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Qoco_Airlines.Models;
using Qoco_Airlines.Services;

namespace FlightQualityAnalyzer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightService _service;
        private readonly ILogger<FlightsController> _logger;


        public FlightsController(IFlightService service, ILogger<FlightsController> logger)
        {
            _service = service;
            _logger = logger;

        }

        // GET api/flights
        [HttpGet]
        public ActionResult<IEnumerable<Flight>> GetAll()
        {
            try
            {
                var flights = _service.GetAllFlights();
                return Ok(flights);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all flights.");
                return StatusCode(500, "An error occurred while retrieving flights.");
            }
        }

        // GET api/flights/inconsistencies
        [HttpGet("inconsistencies")]
        public ActionResult<IEnumerable<Inconsistency>> GetInconsistencies()
        {
            try
            {
                var inconsistencies = _service.GetSequenceInconsistencies();
                return Ok(inconsistencies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get flight inconsistencies.");
                return StatusCode(500, "An error occurred while retrieving inconsistencies.");
            }
        }
    }
}
