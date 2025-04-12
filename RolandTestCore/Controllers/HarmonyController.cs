using Microsoft.AspNetCore.Mvc;
using RolandTestCore.Models;
using RolandTestCore.Services;

namespace RolandTestCore.Controllers
{
    [ApiController]
    [Route("api/effects/harmony")]
    public class HarmonyController : ControllerBase
    {
        private readonly IMidiService _midiService;
        private readonly ILogger<HarmonyController> _logger;

        public HarmonyController(IMidiService midiService, ILogger<HarmonyController> logger)
        {
            _midiService = midiService;
            _logger = logger;
        }

        /// <summary>
        /// Apply Harmony effect with specified parameters
        /// </summary>
        /// <param name="model">Harmony effect parameters</param>
        /// <returns>Success message</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult ApplyHarmonyEffect([FromBody] HarmonyModel model)
        {
            try
            {
                _midiService.SendHarmonyEffect(
                    model.H1Level,
                    model.H2Level,
                    model.H3Level,
                    model.H1Key,
                    model.H2Key,
                    model.H3Key,
                    model.H1Gender,
                    model.H2Gender,
                    model.H3Gender
                );
                
                return Ok(new { message = "Harmony effect applied successfully" });
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
                _logger.LogError(ex, "Error applying Harmony effect");
                return StatusCode(500, "Error applying Harmony effect: " + ex.Message);
            }
        }
    }
}