namespace RolandClient.Models
{
    /// <summary>
    /// İstemci yapılandırma seçenekleri
    /// </summary>
    public class ClientOptions
    {
        /// <summary>
        /// İstemci benzersiz kimliği (her istemci için farklı olmalı)
        /// </summary>
        public string ClientId { get; set; } = string.Empty;
        
        /// <summary>
        /// İstemcinin grubu/odası/kanalı (aynı gruptaki istemciler aynı komutları alır)
        /// </summary>
        public string Group { get; set; } = "default";
        
        /// <summary>
        /// MIDI cihaz ID'si
        /// </summary>
        public int MidiDeviceId { get; set; } = 1;
        
        /// <summary>
        /// Otomatik başlatma (açılışta MIDI cihazını otomatik başlatır)
        /// </summary>
        public bool AutoInitialize { get; set; } = true;
    }
}
