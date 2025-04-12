using StackExchange.Redis;
using System.Text.Json;
using RolandClient.Models;

namespace RolandClient.Services
{
    /// <summary>
    /// Redis bağlantısını yöneten ve komutları dinleyen servis
    /// </summary>
    public class RedisService : IDisposable
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly ISubscriber _subscriber;
        private readonly ILogger<RedisService> _logger;
        private readonly ClientSettings _clientSettings;
        
        // Redis kanal ve anahtar isimleri
        private const string CommandChannelName = "ROLAND:COMMANDS";
        private const string ResponseChannelName = "ROLAND:RESPONSES";
        private readonly string _deviceKey;
        
        public RedisService(ILogger<RedisService> logger, ClientSettings clientSettings)
        {
            _logger = logger;
            _clientSettings = clientSettings;
            
            // Ensure MAC address is formatted properly for use as deviceId (no colons, uppercase)
            var formattedMacAddress = _clientSettings.MacAddress.Replace(":", "").Replace("-", "").ToUpper();
            _deviceKey = $"ROLAND:DEVICE:{formattedMacAddress}";   // Use MAC address consistently
            
            _logger.LogInformation("Using device key: {DeviceKey}", _deviceKey);
            
            try
            {
                // Redis bağlantısını yapılandır
                var configurationOptions = ConfigurationOptions.Parse(_clientSettings.RedisConnectionString);
                
                // Parola varsa ekle
                if (!string.IsNullOrEmpty(_clientSettings.RedisPassword))
                {
                    configurationOptions.Password = _clientSettings.RedisPassword;
                }
                
                // Bağlantı seçeneklerini ayarla
                configurationOptions.ConnectTimeout = 10000; // 10 saniye
                configurationOptions.SyncTimeout = 10000;
                configurationOptions.AbortOnConnectFail = false;
                
                // Redis'e bağlan
                _redis = ConnectionMultiplexer.Connect(configurationOptions);
                _db = _redis.GetDatabase();
                _subscriber = _redis.GetSubscriber();
                
                _logger.LogInformation("Redis bağlantısı başarıyla oluşturuldu: {RedisConnectionString}", _clientSettings.RedisConnectionString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis bağlantısı oluşturulurken hata oluştu: {RedisConnectionString}", _clientSettings.RedisConnectionString);
                throw;
            }
        }

        /// <summary>
        /// Cihaz bilgilerini Redis'e kaydet
        /// </summary>
        public async Task RegisterDeviceAsync(RolandClientSettings settings)
        {
            try
            {
                var json = JsonSerializer.Serialize(settings);
                await _db.StringSetAsync(_deviceKey, json);
                _logger.LogInformation("Cihaz bilgileri Redis'e kaydedildi: {DeviceId}", settings.DeviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz bilgileri Redis'e kaydedilirken hata oluştu: {DeviceId}", settings.DeviceId);
            }
        }

        /// <summary>
        /// API'den gelen ayarları güncelle
        /// </summary>
        public async Task UpdateSettingsAsync(RolandClientSettings settings)
        {
            try
            {
                var json = JsonSerializer.Serialize(settings);
                await _db.StringSetAsync(_deviceKey, json);
                _logger.LogInformation("Cihaz ayarları güncellendi: {DeviceId}", settings.DeviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz ayarları güncellenirken hata oluştu: {DeviceId}", settings.DeviceId);
            }
        }

        /// <summary>
        /// Redis'ten cihaz ayarlarını alır
        /// </summary>
        public async Task<RolandClientSettings?> GetDeviceSettingsAsync(string deviceId)
        {
            try
            {
                // DeviceId formatını standartlaştır
                string formattedDeviceId = deviceId.Replace(":", "").Replace("-", "").ToUpper();
                string key = $"ROLAND:DEVICE:{formattedDeviceId}";
                
                _logger.LogInformation("Redis'ten cihaz ayarları alınıyor: {DeviceId}, Key: {Key}", formattedDeviceId, key);
                
                var json = await _db.StringGetAsync(key);
                
                if (json.IsNullOrEmpty)
                {
                    _logger.LogWarning("Redis'te cihaz ayarları bulunamadı: {DeviceId}", formattedDeviceId);
                    return null;
                }
                
                var settings = System.Text.Json.JsonSerializer.Deserialize<RolandClientSettings>(json!);
                _logger.LogInformation("Redis'ten cihaz ayarları alındı: {DeviceId}, ActiveEffect: {ActiveEffect}", 
                    formattedDeviceId, settings?.ActiveEffect);
                    
                return settings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis'ten cihaz ayarları alınırken hata oluştu: {DeviceId}, {Message}", deviceId, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Komut kanalını dinlemeye başla
        /// </summary>
        public void SubscribeToCommands(Action<RolandEffectCommand> commandHandler)
        {
            _subscriber.Subscribe(CommandChannelName, (channel, message) =>
            {
                try
                {
                    var command = JsonSerializer.Deserialize<RolandEffectCommand>(message!);
                    
                    if (command != null)
                    {
                        // Format both target and my device IDs for consistent comparison
                        string myDeviceId = _clientSettings.MacAddress.Replace(":", "").Replace("-", "").ToUpper();
                        string targetDeviceId = command.TargetDeviceId.Replace(":", "").Replace("-", "").ToUpper();
                    
                        _logger.LogInformation(
                            "Komut alındı: {CommandId}, TargetDeviceId: {TargetDeviceId}, MyDeviceId: {MyDeviceId}, Broadcast: {IsBroadcast}, EffectType: {EffectType}",
                            command.CommandId, targetDeviceId, myDeviceId, command.IsBroadcast, command.EffectType);
                    
                        // Bu cihaza gelen veya broadcast komutları işle
                        if (command.IsBroadcast || targetDeviceId == myDeviceId)
                        {
                            _logger.LogInformation("Komut işleniyor: {CommandId}, Efekt: {EffectType}", 
                                command.CommandId, command.EffectType);
                            
                            commandHandler(command);
                        }
                        else
                        {
                            _logger.LogWarning("Komut bu cihaz için değil, atlıyorum. CommandId: {CommandId}, TargetDeviceId: {TargetDeviceId}, MyDeviceId: {MyDeviceId}", 
                                command.CommandId, targetDeviceId, myDeviceId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Komut işlenirken hata oluştu");
                }
            });
            
            _logger.LogInformation("Redis komut kanalı dinleniyor: {Channel}", CommandChannelName);
        }

        /// <summary>
        /// Komut yanıtı gönder ve Redis'ten komutu temizle
        /// </summary>
        public async Task SendCommandResponseAsync(RolandCommandResponse response)
        {
            try
            {
                var json = JsonSerializer.Serialize(response);
                await _subscriber.PublishAsync(ResponseChannelName, json);
                _logger.LogInformation("Komut yanıtı gönderildi: {CommandId}, Durum: {Status}", 
                    response.CommandId, response.Status);
                
                // İşlenen komutu Redis'ten sil
                string commandKey = $"ROLAND:CMD:{response.CommandId}";
                bool deleted = await _db.KeyDeleteAsync(commandKey);
                _logger.LogInformation("Komut Redis'ten silindi: {CommandId}, Başarılı: {Deleted}", 
                    response.CommandId, deleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Komut yanıtı gönderilirken hata oluştu: {CommandId}", response.CommandId);
            }
        }

        /// <summary>
        /// Tüm eski komutları Redis'ten temizle
        /// </summary>
        public async Task CleanupOldCommandsAsync()
        {
            try
            {
                _logger.LogInformation("Eski komutlar temizleniyor...");
                var server = _redis.GetServer(_redis.GetEndPoints().First());
                var keys = server.Keys(pattern: "ROLAND:CMD:*").ToList();
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
                            
                            // 10 dakikadan eski komutları temizle
                            if (command != null && (DateTime.UtcNow - command.CreatedAt).TotalMinutes > 10)
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
        /// Redis kaynaklarını temizle
        /// </summary>
        public void Dispose()
        {
            _redis?.Dispose();
        }
    }
}