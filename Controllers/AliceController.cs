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
        private readonly DataBaseContext _dataBaseContext;
        public AliceController(Response aliceResponse, DataBaseContext dataBaseContext)
        {
            _aliceResponse = aliceResponse;
            _dataBaseContext = dataBaseContext;
        }
        [HttpPost("/alice")]
        public async Task<IActionResult> WebHook([FromBody] AliceRequest request)
        {            
            try
            {
                //var aliceRequest = JsonConvert.DeserializeObject<AliceRequest>(request.ToString());
                //return null;
                var response = await _aliceResponse.AliceResponse(request, _dataBaseContext);
                return Ok(response);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
