using Microsoft.AspNetCore.Mvc;
using RolandTestCore.Models;
using RolandTestCore.Services;

namespace RolandTestCore.Controllers
{
    [ApiController]
    [Route("api/effects/reverb")]
    public class ReverbController : ControllerBase
    {
        private readonly IMidiService _midiService;
        private readonly ILogger<ReverbController> _logger;

        public ReverbController(IMidiService midiService, ILogger<ReverbController> logger)
        {
            _midiService = midiService;
            _logger = logger;
        }

        /// <summary>
        /// Apply Reverb effect with specified parameters
        /// </summary>
        /// <param name="model">Reverb effect parameters</param>
        /// <returns>Success message</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult ApplyReverbEffect([FromBody] ReverbModel model)
        {
            try
            {
                _midiService.SendReverbEffect(
                    model.Type,
                    model.Param1,
                    model.Param2,
                    model.Param3,
                    model.Param4
                );
                
                return Ok(new { message = "Reverb effect applied successfully" });
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
                _logger.LogError(ex, "Error applying Reverb effect");
                return StatusCode(500, "Error applying Reverb effect: " + ex.Message);
            }
        }
    }
}