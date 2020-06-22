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
        Client _client;

        public CommandController()
        {
            _client = Client.getClient();
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
                    return BadRequest("Invalid data.");
                }


                var result = await _client.Execute(command);
                return result;
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
