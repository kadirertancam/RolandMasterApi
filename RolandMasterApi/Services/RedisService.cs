using StackExchange.Redis;
using System.Text.Json;
using RolandMasterApi.Models;

namespace RolandMasterApi.Services
{
    /// <summary>
    /// Redis veritabanına bağlanarak client ayarlarını ve komutlarını yöneten servis
    /// </summary>
    public class RedisService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly ILogger<RedisService> _logger;
        private readonly ISubscriber _subscriber;

        public RedisService(ILogger<RedisService> logger)
        {
            _logger = logger;
            
            try
            {
                // Redis bağlantısını yapılandır
                var configurationOptions = ConfigurationOptions.Parse(RedisConfiguration.RedisHost);
                
                // Parola varsa ekle
                if (!string.IsNullOrEmpty(RedisConfiguration.RedisPassword))
                {
                    configurationOptions.Password = RedisConfiguration.RedisPassword;
                }
                
                // Bağlantı seçeneklerini ayarla
                configurationOptions.ConnectTimeout = 10000; // 10 saniye
                configurationOptions.SyncTimeout = 10000;
                configurationOptions.AbortOnConnectFail = false;
                
                // Redis'e bağlan
                _redis = ConnectionMultiplexer.Connect(configurationOptions);
                _db = _redis.GetDatabase();
                _subscriber = _redis.GetSubscriber();
                
                _logger.LogInformation("Redis bağlantısı başarıyla oluşturuldu.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis bağlantısı oluşturulurken hata oluştu.");
                throw;
            }
        }

        #region Client Ayarları

        /// <summary>
        /// Bir Roland cihazının ayarlarını kaydeder
        /// </summary>
        public async Task SetClientSettingsAsync(RolandClientSettings settings)
        {
            var key = $"{RedisConfiguration.DeviceKeyPrefix}{settings.DeviceId}";
            
            try
            {
                var json = JsonSerializer.Serialize(settings);
                await _db.StringSetAsync(key, json);
                _logger.LogInformation("Client ayarları kaydedildi: {DeviceId}", settings.DeviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Client ayarları kaydedilirken hata oluştu: {DeviceId}", settings.DeviceId);
                throw;
            }
        }

        /// <summary>
        /// Bir Roland cihazının ayarlarını getirir
        /// </summary>
        public async Task<RolandClientSettings?> GetClientSettingsAsync(string deviceId)
        {
            // Ensure deviceId is formatted consistently
            deviceId = deviceId.Replace(":", "").Replace("-", "").ToUpper();
            var key = $"{RedisConfiguration.DeviceKeyPrefix}{deviceId}";
            
            try
            {
                _logger.LogInformation("Attempting to get client settings with key: {Key}", key);
                var json = await _db.StringGetAsync(key);
                
                if (json.IsNullOrEmpty)
                {
                    _logger.LogWarning("No data found for key: {Key}", key);
                    return null;
                }
                
                _logger.LogInformation("Found data for key: {Key}", key);
                return JsonSerializer.Deserialize<RolandClientSettings>(json!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Client ayarları alınırken hata oluştu: {DeviceId}", deviceId);
                throw;
            }
        }

        /// <summary>
        /// Tüm Roland cihazlarının listesini getirir
        /// </summary>
        public async Task<List<RolandClientSettings>> GetAllClientSettingsAsync()
        {
            var result = new List<RolandClientSettings>();
            
            try
            {
                var server = _redis.GetServer(_redis.GetEndPoints().First());
                var keys = server.Keys(pattern: $"{RedisConfiguration.DeviceKeyPrefix}*");
                
                foreach (var key in keys)
                {
                    var json = await _db.StringGetAsync(key);
                    if (!json.IsNullOrEmpty)
                    {
                        var settings = JsonSerializer.Deserialize<RolandClientSettings>(json!);
                        if (settings != null)
                        {
                            result.Add(settings);
                        }
                    }
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tüm client ayarları alınırken hata oluştu.");
                throw;
            }
        }

        /// <summary>
        /// Bir Roland cihazının ayarlarını siler
        /// </summary>
        public async Task DeleteClientSettingsAsync(string deviceId)
        {
            var key = $"{RedisConfiguration.DeviceKeyPrefix}{deviceId}";
            
            try
            {
                await _db.KeyDeleteAsync(key);
                _logger.LogInformation("Client ayarları silindi: {DeviceId}", deviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Client ayarları silinirken hata oluştu: {DeviceId}", deviceId);
                throw;
            }
        }

        #endregion

        #region Komut İşlemleri

        /// <summary>
        /// Roland cihazlarına komut gönderir
        /// </summary>
        public async Task<bool> SendCommandAsync(RolandEffectCommand command)
        {
            if (!command.IsValid())
            {
                _logger.LogWarning("Geçersiz komut: {CommandId}", command.CommandId);
                return false;
            }
            
            try
            {
                // Komutu Redis'e kaydet
                var commandKey = $"{RedisConfiguration.CommandKeyPrefix}{command.CommandId}";
                var json = JsonSerializer.Serialize(command);
                await _db.StringSetAsync(commandKey, json, TimeSpan.FromHours(1)); // 1 saat sakla
                
                // Komutu pub/sub kanalına gönder
                await _subscriber.PublishAsync(RedisConfiguration.CommandChannelName, json);
                
                _logger.LogInformation("Komut gönderildi: {CommandId}, Hedef: {TargetDeviceId}", command.CommandId, command.TargetDeviceId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Komut gönderilirken hata oluştu: {CommandId}", command.CommandId);
                return false;
            }
        }

        /// <summary>
        /// Gönderilen bir komutun durumunu alır
        /// </summary>
        public async Task<RolandEffectCommand?> GetCommandStatusAsync(string commandId)
        {
            var key = $"{RedisConfiguration.CommandKeyPrefix}{commandId}";
            
            try
            {
                var json = await _db.StringGetAsync(key);
                
                if (json.IsNullOrEmpty)
                {
                    return null;
                }
                
                return JsonSerializer.Deserialize<RolandEffectCommand>(json!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Komut durumu alınırken hata oluştu: {CommandId}", commandId);
                throw;
            }
        }

        /// <summary>
        /// Komut yanıtlarını dinlemeye başlar
        /// </summary>
        public void SubscribeToResponses(Action<RolandCommandResponse> callback)
        {
            _subscriber.Subscribe(RedisConfiguration.ResponseChannelName, (channel, message) =>
            {
                try
                {
                    var response = JsonSerializer.Deserialize<RolandCommandResponse>(message!);
                    if (response != null)
                    {
                        callback(response);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Komut yanıtı işlenirken hata oluştu");
                }
            });
            
            _logger.LogInformation("Komut yanıtları dinleniyor");
        }

        /// <summary>
        /// Komut yanıtlarını gönderir (client tarafından kullanılır)
        /// </summary>
        public async Task SendResponseAsync(RolandCommandResponse response)
        {
            try
            {
                var json = JsonSerializer.Serialize(response);
                await _subscriber.PublishAsync(RedisConfiguration.ResponseChannelName, json);
                _logger.LogInformation("Komut yanıtı gönderildi: {CommandId}, Cihaz: {DeviceId}", response.CommandId, response.DeviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Komut yanıtı gönderilirken hata oluştu: {CommandId}", response.CommandId);
            }
        }

        #endregion
        
        /// <summary>
        /// Servis kapanırken Redis bağlantısını kapatır
        /// </summary>
        /// <summary>
        /// Get all Redis keys matching a pattern
        /// </summary>
        public async Task<List<string>> GetAllRedisKeysAsync(string pattern = "*")
        {
            try
            {
                var result = new List<string>();
                var server = _redis.GetServer(_redis.GetEndPoints().First());
                var keys = server.Keys(pattern: pattern);
                
                foreach (var key in keys)
                {
                    result.Add(key.ToString());
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Redis keys");
                throw;
            }
        }
        
        /// <summary>
        /// Eski komutları Redis'ten temizler
        /// </summary>
        public async Task CleanupOldCommandsAsync()
        {
            try
            {
                _logger.LogInformation("Eski komutlar temizleniyor...");
                var keys = await GetAllRedisKeysAsync(RedisConfiguration.CommandKeyPrefix + "*");
                _logger.LogInformation("Redis'te {Count} komut bulundu", keys.Count);

                int deletedCount = 0;
                foreach (var key in keys)
                {
                    string commandJson = await _db.StringGetAsync(key);
                    if (!string.IsNullOrEmpty(commandJson))
                    {
                        try
                        {
                            var command = JsonSerializer.Deserialize<RolandEffectCommand>(commandJson);
                            
                            // 30 dakikadan eski komutları temizle
                            if (command != null && (DateTime.UtcNow - command.CreatedAt).TotalMinutes > 30)
                            {
                                await _db.KeyDeleteAsync(key);
                                deletedCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Komut işlenirken hata oluştu: {Key}, silinecek", key);
                            // JSON parse edilemiyorsa sil
                            await _db.KeyDeleteAsync(key);
                            deletedCount++;
                        }
                    }
                }
                
                _logger.LogInformation("Eski komutlar temizlendi. Toplam {DeletedCount} komut silindi", deletedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Eski komutlar temizlenirken hata oluştu");
            }
        }

        /// <summary>
        /// Get a Redis value directly by key
        /// </summary>
        public async Task<string?> GetRedisValueAsync(string key)
        {
            try
            {
                var value = await _db.StringGetAsync(key);
                return value.IsNullOrEmpty ? null : value.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Redis value for key: {Key}", key);
                return null;
            }
        }

        /// <summary>
        /// Delete a Redis key
        /// </summary>
        public async Task<bool> DeleteRedisKeyAsync(string key)
        {
            try
            {
                return await _db.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Redis key: {Key}", key);
                return false;
            }
        }
        
        public void Dispose()
        {
            _redis?.Dispose();
        }
    }
}
