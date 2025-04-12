using Microsoft.AspNetCore.Mvc;
using RolandTestCore.Models;
using RolandTestCore.Services;

namespace RolandTestCore.Controllers
{
    [ApiController]
    [Route("api/effects/vocoder")]
    public class VocoderController : ControllerBase
    {
        private readonly IMidiService _midiService;
        private readonly ILogger<VocoderController> _logger;

        public VocoderController(IMidiService midiService, ILogger<VocoderController> logger)
        {
            _midiService = midiService;
            _logger = logger;
        }

        /// <summary>
        /// Apply Vocoder effect with specified parameters
        /// </summary>
        /// <param name="model">Vocoder effect parameters</param>
        /// <returns>Success message</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult ApplyVocoderEffect([FromBody] VocoderModel model)
        {
            try
            {
                _midiService.SendVocoderEffect(
                    model.Type,
                    model.Param1,
                    model.Param2,
                    model.Param3,
                    model.Param4
                );
                
                return Ok(new { message = "Vocoder effect applied successfully" });
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
                _logger.LogError(ex, "Error applying Vocoder effect");
                return StatusCode(500, "Error applying Vocoder effect: " + ex.Message);
            }
        }
    }
}