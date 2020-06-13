using FlightMobileServer.Models;
using FlightMobileServer.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
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

        public CommandController(ICommandService services, IHostingEnvironment env)
        {
            _service = services;
            //_env = env;
        }

        // POST api/command
        [HttpPost]
        [Route("api/command")]
        public ActionResult Post([FromBody] object json)
        {
            Command command = JsonConvert.DeserializeObject<Command>(json.ToString());

            _service.SendCommand(command);

            return Ok();
        }

        // GET /screenshot
        [HttpGet]
        [Route("screenshot")]
        public async Task<ActionResult> GetScreenshot()
        {
            if (Request.Host.Value.Contains("10.0.2.2"))
            {
                return Redirect("http://10.0.2.2:8080/screenshot");
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
