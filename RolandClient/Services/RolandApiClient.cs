using System.Net.Http.Json;
using RolandClient.Models;

namespace RolandClient.Services
{
    /// <summary>
    /// Roland API'si ile iletişim kuran HTTP istemcisi
    /// </summary>
    public class RolandApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RolandApiClient> _logger;
        
        public RolandApiClient(HttpClient httpClient, ILogger<RolandApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        
        /// <summary>
        /// MIDI cihazını başlatır
        /// </summary>
        /// <param name="deviceId">MIDI cihaz ID'si</param>
        /// <returns>Başarılı olup olmadığı</returns>
        public async Task<bool> InitializeMidiDeviceAsync(int deviceId)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/midi/initialize", new { deviceId });
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MIDI cihazı başlatılırken hata oluştu: {Error}", ex.Message);
                return false;
            }
        }
        
        /// <summary>
        /// Robot efektini uygular
        /// </summary>
        public async Task<bool> ApplyRobotEffectAsync(RobotParameters parameters)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/effects/robot", parameters);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Robot efekti uygulanırken hata oluştu: {Error}", ex.Message);
                return false;
            }
        }
        
        /// <summary>
        /// Robot efektini kapatır
        /// </summary>
        public async Task<bool> TurnOffRobotEffectAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("api/effects/robotoff", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Robot efekti kapatılırken hata oluştu: {Error}", ex.Message);
                return false;
            }
        }
        
        /// <summary>
        /// Harmony efektini uygular
        /// </summary>
        public async Task<bool> ApplyHarmonyEffectAsync(HarmonyParameters parameters)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/effects/harmony", parameters);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Harmony efekti uygulanırken hata oluştu: {Error}", ex.Message);
                return false;
            }
        }
        
        /// <summary>
        /// Harmony efektini kapatır
        /// </summary>
        public async Task<bool> TurnOffHarmonyEffectAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("api/effects/harmonyoff", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Harmony efekti kapatılırken hata oluştu: {Error}", ex.Message);
                return false;
            }
        }
        
        /// <summary>
        /// Megafon efektini uygular
        /// </summary>
        public async Task<bool> ApplyMegaphoneEffectAsync(MegaphoneParameters parameters)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/effects/megaphone", parameters);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Megafon efekti uygulanırken hata oluştu: {Error}", ex.Message);
                return false;
            }
        }
        
        /// <summary>
        /// Megaphone efektini kapatır
        /// </summary>
        public async Task<bool> TurnOffMegaphoneEffectAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("api/effects/megaphoneoff", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Megaphone efekti kapatılırken hata oluştu: {Error}", ex.Message);
                return false;
            }
        }
        
        /// <summary>
        /// Reverb efektini uygular
        /// </summary>
        public async Task<bool> ApplyReverbEffectAsync(ReverbParameters parameters)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/effects/reverb", parameters);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reverb efekti uygulanırken hata oluştu: {Error}", ex.Message);
                return false;
            }
        }
        
        /// <summary>
        /// Vocoder efektini uygular
        /// </summary>
        public async Task<bool> ApplyVocoderEffectAsync(VocoderParameters parameters)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/effects/vocoder", parameters);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Vocoder efekti uygulanırken hata oluştu: {Error}", ex.Message);
                return false;
            }
        }
        
        /// <summary>
        /// Vocoder efektini kapatır
        /// </summary>
        public async Task<bool> TurnOffVocoderEffectAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("api/effects/vocoderoff", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Vocoder efekti kapatılırken hata oluştu: {Error}", ex.Message);
                return false;
            }
        }
        
        /// <summary>
        /// Ekolayzer efektini uygular
        /// </summary>
        public async Task<bool> ApplyEqualizerEffectAsync(EqualizerParameters parameters)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/effects/equalizer", parameters);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ekolayzer efekti uygulanırken hata oluştu: {Error}", ex.Message);
                return false;
            }
        }
        
        /// <summary>
        /// Efekt 1'i uygular
        /// </summary>
        public async Task<bool> ApplyEffect1Async()
        {
            try
            {
                var response = await _httpClient.PostAsync("api/effects/effect1", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Efekt 1 uygulanırken hata oluştu: {Error}", ex.Message);
                return false;
            }
        }
        
        /// <summary>
        /// Efekt 2'yi uygular
        /// </summary>
        public async Task<bool> ApplyEffect2Async()
        {
            try
            {
                var response = await _httpClient.PostAsync("api/effects/effect2", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Efekt 2 uygulanırken hata oluştu: {Error}", ex.Message);
                return false;
            }
        }
        
        /// <summary>
        /// Efekt 3'ü uygular
        /// </summary>
        public async Task<bool> ApplyEffect3Async()
        {
            try
            {
                var response = await _httpClient.PostAsync("api/effects/effect3", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Efekt 3 uygulanırken hata oluştu: {Error}", ex.Message);
                return false;
            }
        }
        
        /// <summary>
        /// Efekt 4'ü uygular
        /// </summary>
        public async Task<bool> ApplyEffect4Async()
        {
            try
            {
                var response = await _httpClient.PostAsync("api/effects/effect4", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Efekt 4 uygulanırken hata oluştu: {Error}", ex.Message);
                return false;
            }
        }
    }
}