using Newtonsoft.Json;

namespace RolandClient.Models
{
    /// <summary>
    /// Redis üzerinden gönderilen efekt komutu
    /// </summary>
    public class EffectCommand
    {
        /// <summary>
        /// Efekt tipi
        /// </summary>
        public EffectType Type { get; set; }
        
        /// <summary>
        /// Hedef istemci ID'si (boş ise tüm istemcilere gönderilir)
        /// </summary>
        public string? TargetClientId { get; set; }
        
        /// <summary>
        /// Hedef grup (boş ise tüm gruplara gönderilir)
        /// </summary>
        public string? TargetGroup { get; set; }
        
        /// <summary>
        /// Efekt parametreleri (JSON formatında, efekt tipine göre değişir)
        /// </summary>
        public string ParametersJson { get; set; } = "{}";
        
        /// <summary>
        /// Parametreleri belirli bir tipe dönüştürür
        /// </summary>
        public T GetParameters<T>() where T : class, new()
        {
            if (string.IsNullOrEmpty(ParametersJson))
            {
                return new T();
            }
            
            return JsonConvert.DeserializeObject<T>(ParametersJson) ?? new T();
        }
    }
    
    /// <summary>
    /// Efekt tipleri
    /// </summary>
    public enum EffectType
    {
        Robot = 1,
        Harmony = 2,
        Megaphone = 3,
        Reverb = 4,
        Vocoder = 5,
        Equalizer = 6
    }
}
