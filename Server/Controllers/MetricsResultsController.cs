using Microsoft.AspNetCore.Mvc;
using Server.Interfaces;

namespace Server.Controllers
{
    [Route("MetricsResults")]
    [ApiController]
    public class MetricsResultsController : ControllerBase
    {
        private readonly IMetricsResults MetricsResults;

        public MetricsResultsController(IMetricsResults metricsResults)
        {
            MetricsResults = metricsResults;
        }


        [HttpGet("GetMetrics")]
        public ActionResult GetInfoFromDB()
        {
            try
            {
                return Ok(MetricsResults.GetMetricsResultsFromDB());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
