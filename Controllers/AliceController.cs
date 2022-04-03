using HypothyroBot.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using HypothyroBot.Models.Alice_API;
using System.Threading.Tasks;

namespace HypothyroBot.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AliceController : ControllerBase
    {
        private readonly Response _aliceResponse;
        private readonly ApplicationContext _dbContext;
        public AliceController(Response aliceResponse, ApplicationContext dataBaseContext)
        {
            _aliceResponse = aliceResponse;
            _dbContext = dataBaseContext;
        }
        [HttpPost("/alice")]
        public async Task<IActionResult> WebHook([FromBody] AliceRequest request)
        {            
            try
            {
                var response = await _aliceResponse.AliceResponse(request, _dbContext);
                return Ok(response);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}