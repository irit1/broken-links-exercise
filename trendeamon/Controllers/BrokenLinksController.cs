using Microsoft.AspNetCore.Mvc;
using trendeamon.Models;
using trendeamon.Services.BrokenLinks;

namespace trendeamon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrokenLinksController : ControllerBase
    {
        private IBrokenLinksService _brokenLinksService;

        public BrokenLinksController(IBrokenLinksService brokenLinksService)
        {
            _brokenLinksService = brokenLinksService;
        }

        /// <summary>
        /// start an async request to check broken links count for given urls
        /// return a token that can be used to obtain request status
        /// </summary>
        /// <param name="request"></param>
        [HttpPost("start")]
        public IActionResult StartBrokenLinks([FromBody] BrokenLinksStartRequestModel request)
        {
            var requestId = _brokenLinksService.Start(request);
            return Ok(requestId);
        }

        /// <summary>
        /// return the status of a request that was previously initiated
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        [HttpGet("{requestId}/status")]
        public IActionResult GetRequestStatus(string requestId)
        {
            var status = _brokenLinksService.GetStatus(requestId);
            return Ok(status);
        }
    }
}
