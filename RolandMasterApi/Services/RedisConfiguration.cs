namespace RolandMasterApi.Services
{
    /// <summary>
    /// Redis bağlantı ayarları
    /// </summary>
    public static class RedisConfiguration
    {
        // Redis bağlantı bilgileri - ortam değişkenlerinden alınıyor veya varsayılan değerler kullanılıyor
        public static string RedisHost = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ?? "localhost";
        public static string RedisPassword = Environment.GetEnvironmentVariable("REDIS_PASSWORD") ?? string.Empty;
        public static int RedisPort = int.TryParse(Environment.GetEnvironmentVariable("REDIS_PORT"), out int port) ? port : 6379;
        
        // Redis anahtarları için prefix
        public static string DeviceKeyPrefix = "ROLAND:DEVICE:";
        public static string CommandKeyPrefix = "ROLAND:CMD:";
        public static string CommandChannelName = "ROLAND:COMMANDS";
        public static string ResponseChannelName = "ROLAND:RESPONSES";
    }
}
