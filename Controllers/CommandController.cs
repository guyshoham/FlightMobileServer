using FlightMobileServer.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;


namespace FlightMobileServer.Controllers
{
    [Route("/")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        FlightGearClient _client;

        public CommandController()
        {
            _client = FlightGearClient.GetFlightGearClient();
        }

        // POST api/command
        [HttpPost]
        [Route("api/command")]
        public async Task<ActionResult<Result>> Post([FromBody] Command command)
        {
            // query validation
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }

            //_client.Start();
            var result = await _client.Execute(command);

            return result;
        }

        // GET /screenshot
        [HttpGet]
        [Route("screenshot")]
        public ActionResult GetScreenshot()
        {
            /*IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName()); // `Dns.Resolve()` method is deprecated.
            IPAddress ipAddress1 = ipHostInfo.AddressList[2];
            IPAddress ipAddress2 = ipHostInfo.AddressList[3];*/

            if (Request.Host.Value.Contains("10.0.2.2"))
            {
                return Redirect("http://10.0.2.2:8080/screenshot");
            }
            else
            {
                return Redirect("http://127.0.0.1:8080/screenshot");
            }
        }
    }
}
