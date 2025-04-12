using Microsoft.AspNetCore.Mvc;
using RolandTestCore.Models;
using RolandTestCore.Services;

namespace RolandTestCore.Controllers
{
    [ApiController]
    [Route("api/effects/megaphone")]
    public class MegaphoneController : ControllerBase
    {
        private readonly IMidiService _midiService;
        private readonly ILogger<MegaphoneController> _logger;

        public MegaphoneController(IMidiService midiService, ILogger<MegaphoneController> logger)
        {
            _midiService = midiService;
            _logger = logger;
        }

        /// <summary>
        /// Apply Megaphone effect with specified parameters
        /// </summary>
        /// <param name="model">Megaphone effect parameters</param>
        /// <returns>Success message</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult ApplyMegaphoneEffect([FromBody] MegaphoneModel model)
        {
            try
            {
                _midiService.SendMegaphoneEffect(
                    model.Type,
                    model.Param1,
                    model.Param2,
                    model.Param3,
                    model.Param4
                );
                
                return Ok(new { message = "Megaphone effect applied successfully" });
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
                _logger.LogError(ex, "Error applying Megaphone effect");
                return StatusCode(500, "Error applying Megaphone effect: " + ex.Message);
            }
        }
    }
}