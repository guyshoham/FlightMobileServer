using FlightMobileServer.Models;
using FlightMobileServer.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FlightMobileServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        private readonly ICommandService _service;

        public CommandController(ICommandService services)
        {
            _service = services;
        }

        // POST api/command
        [HttpPost]
        [Route("command")]
        public ActionResult Post([FromBody] object json)
        {
            Command command = JsonConvert.DeserializeObject<Command>(json.ToString());

            _service.SendCommand(command);

            return Ok();
        }

        // GET api/screenshot
        [HttpGet]
        [Route("screenshot")]
        public ActionResult GetScreenshot()
        {
            string screenshot = _service.GetScreenshot();

            return Ok(screenshot);
        }
    }
}
