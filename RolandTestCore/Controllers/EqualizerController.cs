using Microsoft.AspNetCore.Mvc;
using RolandTestCore.Models;
using RolandTestCore.Services;

namespace RolandTestCore.Controllers
{
    [ApiController]
    [Route("api/effects/equalizer")]
    public class EqualizerController : ControllerBase
    {
        private readonly IMidiService _midiService;
        private readonly ILogger<EqualizerController> _logger;

        public EqualizerController(IMidiService midiService, ILogger<EqualizerController> logger)
        {
            _midiService = midiService;
            _logger = logger;
        }

        /// <summary>
        /// Apply Equalizer effect with specified parameters
        /// </summary>
        /// <param name="model">Equalizer effect parameters</param>
        /// <returns>Success message</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult ApplyEqualizerEffect([FromBody] EqualizerModel model)
        {
            try
            {
                _midiService.SendEqualizerEffect(
                    model.EqSwitch,
                    model.LowShelfFreq,
                    model.LowShelfGain,
                    model.LowMidFreq,
                    model.LowMidQ,
                    model.LowMidGain,
                    model.HighMidFreq,
                    model.HighMidQ,
                    model.HighMidGain,
                    model.HighShelfFreq,
                    model.HighShelfGain
                );
                
                return Ok(new { message = "Equalizer effect applied successfully" });
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
                _logger.LogError(ex, "Error applying Equalizer effect");
                return StatusCode(500, "Error applying Equalizer effect: " + ex.Message);
            }
        }
    }
}