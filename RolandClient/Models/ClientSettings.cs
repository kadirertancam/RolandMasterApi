namespace RolandClient.Models
{
    /// <summary>
    /// Client uygulamasının yapılandırma ayarları
    /// </summary>
    public class ClientSettings
    {
        // Cihaz tanımlama bilgileri
        public string DeviceId { get; set; } = Guid.NewGuid().ToString(); // MAC adresi DeviceId olarak kullanılacak, bu alan yedek
        public string DeviceName { get; set; } = Environment.MachineName;
        public string MacAddress { get; set; } = GetMacAddress();
        
        // API bağlantı bilgileri
        public string ApiUrl { get; set; } = "http://localhost:8080";
        public int PollingIntervalSeconds { get; set; } = 5;
        
        // Redis bağlantı bilgileri
        public string RedisConnectionString { get; set; } = "localhost:6379";
        public string RedisPassword { get; set; } = string.Empty;
        
        // MIDI cihaz bilgileri
        public int MidiDeviceId { get; set; } = 1;
        public byte? RolandDeviceId{ get; set; } = 1;
        public bool AutoInitializeMidi { get; set; } = true;
        
        // Log ayarları
        public bool EnableDebugLogging { get; set; } = false;
        public string LogFilePath { get; set; } = "logs/rolandclient.log";
        
        // MAC adresini al ve formatını düzenle (tüm : karakterlerini temizle ve büyük harfe çevir)
        private static string GetMacAddress()
        {
            try
            {
                string macAddress = System.Net.NetworkInformation.NetworkInterface
                    .GetAllNetworkInterfaces()
                    .Where(nic => nic.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up 
                           && nic.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback)
                    .Select(nic => nic.GetPhysicalAddress().ToString())
                    .FirstOrDefault() ?? "UNKNOWN_MAC";
                    
                // MAC adresini standart formata getir: büyük harf ve iki nokta olmadan
                // Format MAC address consistently - no colons or hyphens, uppercase
                string formattedMac = macAddress.Replace(":", "").Replace("-", "").ToUpper();
                Console.WriteLine($"Using MAC address: {formattedMac}");
                return formattedMac;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting MAC address: {ex.Message}");
                return "00155D4FA27F"; // Provide your device's MAC as fallback
            }
        }
    }
}