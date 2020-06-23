using FlightMobileServer.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace FlightMobileServer.Controllers
{
    [Route("/")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        Client _client;

        public CommandController(IConfiguration conf)
        {
            _client = Client.GetClient(conf);
        }

        // POST api/command
        [HttpPost]
        [Route("api/command")]
        public async Task<ActionResult<Result>> Post([FromBody] Command command)
        {
            try
            {
                // query validation
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var result = await _client.Execute(command);
                return result;
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
