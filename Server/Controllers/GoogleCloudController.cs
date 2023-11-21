using Microsoft.AspNetCore.Mvc;
using Server.Interfaces;

namespace Server.Controllers
{
    [Route("GoogleCloud")]
    [ApiController]
    public class GoogleCloudController : ControllerBase
    {
        private readonly IGoogleCloud GoogleCloud;

        public GoogleCloudController(IGoogleCloud googleCloud)
        {
            GoogleCloud = googleCloud;
        }

        [HttpGet("GetMetricsFromDB")]
        public ActionResult GetInfoFromDB(
            [FromQuery(Name = "MachineType")] string MachineType,
            [FromQuery(Name = "Location")] string Location)
        {
            try
            {
                GoogleCloud.Logger.Info($"Get metrics from DB for {MachineType} {Location}");
                return Ok(GoogleCloud.GetInfoFromDB(MachineType, Location));
            }
            catch (Exception ex)
            {
                GoogleCloud.Logger.Error(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetMetricsFromTimeStamp")]
        public ActionResult GetMetricsFromTimeStamp(
            [FromQuery(Name = "MachineType")] string MachineType,
            [FromQuery(Name = "Location")] string Location,
            [FromQuery(Name = "TimeStamp")] string TimeStamp)
        {
            try
            {
                GoogleCloud.Logger.Info($"Get metrics from DB for {MachineType} {Location} from {TimeStamp}");
                return Ok(GoogleCloud.LoadItemsFromTimeStamp(MachineType, Location, TimeStamp));
            }
            catch (Exception ex)
            {
                GoogleCloud.Logger.Error(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
