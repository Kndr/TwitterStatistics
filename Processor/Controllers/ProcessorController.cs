using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Processor.Entities;
using Processor.Filters;
using Processor.Managers.Interfaces;
using Serilog;
using System.Net;
using System.Threading.Tasks;

namespace Processor.Controllers
{
    [TypeFilter(typeof(ProcessorExceptionFilter))]
    [ApiController]
    [Route("[controller]")]
    public class ProcessorController : ControllerBase
    {
        private static IStatisticsManager _statisticsManager;
        private static ITweetManager _tweetManager;

        public ProcessorController(ITweetManager tweetManager, IStatisticsManager StatisticsManager)
        {
            _statisticsManager = StatisticsManager;
            _tweetManager = tweetManager;
        }

        /// <summary>
        /// Start Processing
        /// </summary>
        /// <returns></returns>
        [HttpPost("Start")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public IActionResult StartProcessing()
        {
            try
            {
                _tweetManager.StartAsync();

                _statisticsManager.StartTime();
                return new JsonResult(true);

            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Failed to start processing.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Stop Processing
        /// </summary>
        /// <returns></returns>
        [HttpPost("Stop")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public IActionResult StopProcessing()
        {
            try
            {
                _tweetManager.Stop();
                _statisticsManager.StopTime();
                return new JsonResult(true);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Failed to stop processing.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Get Tweet Statistics
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetStatistics")]
        [ProducesResponseType(typeof(TweetStatUi), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetStatisticsAsync()
        {
            try
            {
                var statistics = await _statisticsManager.GetStatisticsAsync();

                return new JsonResult(statistics);
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "Failed to generate statistics.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
