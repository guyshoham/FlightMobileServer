using FlightMobileServer.Models;
using FlightMobileServer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FlightMobileServer.Controllers
{
    [Route("/")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        private readonly ICommandService _service;
        //static readonly HttpClient client = new HttpClient();
        //private readonly IHostingEnvironment _env;
        private readonly FlightGearClient _client;

        public CommandController(ICommandService services)
        {
            _service = services;
            _client = new FlightGearClient();
        }

        // POST api/command
        [HttpPost]
        [Route("api/command")]
        public async Task<ActionResult<Command>> Post([FromBody] Command command)
        {
            // query validation
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data.");
            }

            _client.Start();
            await _client.Execute(command);

            return Ok();

        }

        // GET /screenshot
        [HttpGet]
        [Route("screenshot")]
        public async Task<ActionResult> GetScreenshot()
        {
            if (Request.Host.Value.Contains("10.0.2.2"))
            {
                return Redirect("http://10.0.2.2:5000/screenshot");
            }
            else
            {
                return Redirect("http://127.0.0.1:8080/screenshot");
            }
        }

        /*private async Task<HttpResponseMessage> GetScreenshotFromSimluator()
        {
            string url = "http://localhost:8080/screenshot";

            //HttpResponseMessage response = await client.GetAsync(url);
            HttpResponseMessage response = await client.GetAsync(url);
            //client.DownloadFile(new Uri(url), @"c:\temp\image35.png");

            return response;
        }*/
    }
}
