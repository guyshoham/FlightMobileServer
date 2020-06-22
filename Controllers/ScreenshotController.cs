using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlightMobileServer.Controllers
{
    [Route("")]
    [ApiController]
    public class ScreenshotController : ControllerBase
    {
        private readonly string host, port;
        readonly int timeout = 7000;

        public ScreenshotController(IConfiguration conf)
        {
            this.host = conf.GetValue<string>("Logging:ScreenshotHostPort:Host");
            this.port = conf.GetValue<string>("Logging:ScreenshotHostPort:Port");
        }

        // GET /screenshot
        [HttpGet]
        [Route("screenshot")]
        public async Task<IActionResult> GetScreenshot()
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(timeout)
            };

            try
            {
                // create url
                string url = "http://" + host + ":" + port + "/screenshot";

                // send http GET request.
                var response = await client.GetAsync(url);

                // waiting for response...
                var image = await response.Content.ReadAsStreamAsync();
                var screenshot = File(image, "img/jpg");

                return screenshot;
            }
            catch (Exception)
            {
                return NotFound("Error: in method GET screenshot\n");
            }
        }
    }
}
