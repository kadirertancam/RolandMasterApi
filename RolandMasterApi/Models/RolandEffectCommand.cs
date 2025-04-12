namespace RolandMasterApi.Models
{
    /// <summary>
    /// Client uygulamalarına gönderilecek efekt komutları
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
        
        // Yardımcı metodlar
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(EffectType) || EffectType == "none")
            {
                return false;
            }
            
            if (!IsBroadcast && string.IsNullOrEmpty(TargetDeviceId))
            {
                return false;
            }
            
            return true;
        }
    }
    
    /// <summary>
    /// Client uygulamalarından dönen komut yanıtları
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
