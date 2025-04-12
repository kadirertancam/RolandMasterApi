using Microsoft.AspNetCore.Mvc;
using RolandTestCore.Models;
using RolandTestCore.Services;

namespace RolandTestCore.Controllers
{
    [ApiController]
    [Route("api/effects/robot")]
    public class RobotController : ControllerBase
    {
        private readonly IMidiService _midiService;
        private readonly ILogger<RobotController> _logger;

        public RobotController(IMidiService midiService, ILogger<RobotController> logger)
        {
            _midiService = midiService;
            _logger = logger;
        }

        /// <summary>
        /// Apply Robot effect with specified parameters
        /// </summary>
        /// <param name="model">Robot effect parameters</param>
        /// <returns>Success message</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult ApplyRobotEffect([FromBody] RobotModel model)
        {
            try
            {
                _midiService.SendRobotEffect(
                    model.Octave,
                    model.FeedbackSwitch,
                    model.FeedbackResonance,
                    model.FeedbackLevel
                );
                
                return Ok(new { message = "Robot effect applied successfully" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "MIDI device not initialized");
                return BadRequest("MIDI device not initialized. Call api/midi/initialize first.");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogWarning(ex, "Invalid parameter");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying Robot effect");
                return StatusCode(500, "Error applying Robot effect: " + ex.Message);
            }
        }
    }
}