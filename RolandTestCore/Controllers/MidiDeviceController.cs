using Microsoft.AspNetCore.Mvc;
using RolandTestCore.Models;
using RolandTestCore.Services;

namespace RolandTestCore.Controllers
{
    [ApiController]
    [Route("api/midi")]
    public class MidiDeviceController : ControllerBase
    {
        private readonly IMidiService _midiService;
        private readonly ILogger<MidiDeviceController> _logger;

        public MidiDeviceController(IMidiService midiService, ILogger<MidiDeviceController> logger)
        {
            _midiService = midiService;
            _logger = logger;
        }

        /// <summary>
        /// Get all available MIDI devices
        /// </summary>
        /// <returns>List of available MIDI devices</returns>
        [HttpGet("devices")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<MidiDeviceModel>> GetDevices()
        {
            try
            {
                var devices = _midiService.GetAvailableMidiDevices()
                    .Select(d => new MidiDeviceModel { Id = d.Id, Name = d.Name });
                
                return Ok(devices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting MIDI devices");
                return StatusCode(500, "Error retrieving MIDI devices: " + ex.Message);
            }
        }

        /// <summary>
        /// Initialize a MIDI device
        /// </summary>
        /// <param name="model">MIDI device initialization information</param>
        /// <returns>Success message</returns>
        [HttpPost("initialize")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult InitializeMidi([FromBody] MidiInitializeModel model)
        {
            try
            {
                _midiService.Initialize(model.DeviceId);
                return Ok(new { message = $"MIDI device {model.DeviceId} initialized successfully" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid MIDI device ID");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing MIDI device");
                return StatusCode(500, "Error initializing MIDI device: " + ex.Message);
            }
        }
    }
}