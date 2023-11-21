using Microsoft.AspNetCore.Mvc;
using Server.Interfaces;

namespace Server.Controllers
{
    [Route("AzureCloud")]
    [ApiController]
    public class AzureCloudController : ControllerBase
    {
        private readonly IAzureCloud AzureCloud;

        public AzureCloudController(IAzureCloud azureCloud)
        {
            AzureCloud = azureCloud;
        }

        [HttpGet("GetMetricsFromDB")]
        public ActionResult GetInfoFromDB(
            [FromQuery(Name = "MachineType")] string MachineType,
            [FromQuery(Name = "Location")] string Location)
        {
            try
            {
                AzureCloud.Logger.Info($"Get metrics from DB for {MachineType} {Location}");
                return Ok(AzureCloud.GetInfoFromDB(MachineType, Location));
            }
            catch(Exception ex) 
            {
                AzureCloud.Logger.Error(ex.Message);
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
                AzureCloud.Logger.Info($"Get metrics from DB for {MachineType} {Location} from {TimeStamp}");
                return Ok(AzureCloud.LoadItemsFromTimeStamp(MachineType, Location, TimeStamp));
            }
            catch (Exception ex)
            {
                AzureCloud.Logger.Error(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
