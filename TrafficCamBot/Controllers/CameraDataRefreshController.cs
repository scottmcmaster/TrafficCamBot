using System.Configuration;
using System.Web.Http;
using TrafficCamBot.Bot;

namespace TrafficCamBot.Controllers
{
    public class CameraDataRefreshController : ApiController
    {
        readonly CameraDataServiceManager cameraDataServiceManager;
        readonly string secretCode;

        public CameraDataRefreshController(CameraDataServiceManager cameraDataServiceManager)
        {
            this.cameraDataServiceManager = cameraDataServiceManager;
            var appSettings = ConfigurationManager.AppSettings;
            secretCode = appSettings["RefreshCode"];
        }

        [Route("api/refresh/{code}/{serviceName}")]
        [HttpGet]
        public IHttpActionResult Get(string code, string serviceName)
        {
            if (code != secretCode)
            {
                return NotFound();
            }

            var service = cameraDataServiceManager.GetCameraDataService(serviceName);
            if (service == null)
            {
                return NotFound();
            }

            service.RefreshCameras();

            return Ok();
        }
    }
}