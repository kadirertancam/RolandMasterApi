using System.Text.Json;
using System.Text.Json.Serialization;

namespace RolandClient.Models
{
    /// <summary>
    /// API'den alınan efekt komutları
    /// </summary>
    public class RolandEffectCommand
    {
        // Komut kimliği ve hedef client
        public string CommandId { get; set; } = Guid.NewGuid().ToString();
        public string TargetDeviceId { get; set; } = string.Empty;
        
        // Komut bilgisi
        public string EffectType { get; set; } = "none"; // none, robot, harmony, megaphone, reverb, vocoder, equalizer
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        
        // Komut durumu
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "pending"; // pending, executing, completed, failed
        public string? ErrorMessage { get; set; }
        
        // Broadcast için komut mu?
        public bool IsBroadcast { get; set; } = false;
        
        // İlgili parametre değerini al (tip dönüşümü ile)
        public T GetParameterValue<T>(string paramName, T defaultValue)
        {
            if (Parameters.TryGetValue(paramName, out var value))
            {
                try
                {
                    if (value is JsonElement jsonElement)
                    {
                        return jsonElement.Deserialize<T>() ?? defaultValue;
                    }
                    
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
            
            return defaultValue;
        }
    }
    
    /// <summary>
    /// API'ye gönderilecek komut yanıtları
    /// </summary>
    public class RolandCommandResponse
    {
        public string CommandId { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
        public string Status { get; set; } = "completed"; // completed, failed
        public string? ErrorMessage { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
}