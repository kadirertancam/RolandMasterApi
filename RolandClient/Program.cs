using RolandClient;
using RolandClient.Models;
using RolandClient.Services;
using System.Text.Json;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // Client ayarlarını yükle
        var clientSettings = LoadClientSettings(hostContext.Configuration);
        services.AddSingleton(clientSettings);

        // Windows Service olarak yapılandır
        services.AddWindowsService(options =>
        {
            options.ServiceName = "RolandVT4Client";
        });

        // Servislerimizi kaydet
        services.AddSingleton<MidiService>();
        services.AddSingleton<DirectMidiSender>();
        services.AddSingleton<RedisService>();
        services.AddSingleton<CommandProcessor>();
        
        // Worker servisi ekle
        services.AddHostedService<Worker>();
    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddDebug();
        logging.AddEventLog();
        
        // Loglama seviyesini ayarla
        var clientSettings = LoadClientSettings(hostingContext.Configuration);
        if (clientSettings.EnableDebugLogging)
        {
            logging.SetMinimumLevel(LogLevel.Debug);
        }
        else
        {
            logging.SetMinimumLevel(LogLevel.Information);
        }
    })
    .Build();

await host.RunAsync();

// Yapılandırma dosyasından veya varsayılan değerlerden client ayarlarını yükle
ClientSettings LoadClientSettings(IConfiguration configuration)
{
    try
    {
        // MAC adresini önce al
        string macAddress = GetMacAddress();
        
        // appsettings.json'dan yükle
        var settings = new ClientSettings
        {
            DeviceId = configuration["ClientSettings:DeviceId"] ?? macAddress,
            DeviceName = configuration["ClientSettings:DeviceName"] ?? Environment.MachineName,
            MacAddress = macAddress, // MAC adresini doğrudan kullan
            ApiUrl = configuration["ClientSettings:ApiUrl"] ?? "http://localhost:8080",
            PollingIntervalSeconds = int.TryParse(configuration["ClientSettings:PollingIntervalSeconds"], out int interval) ? interval : 5,
            RedisConnectionString = configuration["ClientSettings:RedisConnectionString"] ?? "localhost:6379",
            RedisPassword = configuration["ClientSettings:RedisPassword"] ?? string.Empty,
            MidiDeviceId = int.TryParse(configuration["ClientSettings:MidiDeviceId"], out int deviceId) ? deviceId : 1,
            AutoInitializeMidi = bool.TryParse(configuration["ClientSettings:AutoInitializeMidi"], out bool autoInit) ? autoInit : true,
            EnableDebugLogging = bool.TryParse(configuration["ClientSettings:EnableDebugLogging"], out bool debugLog) ? debugLog : false,
            LogFilePath = configuration["ClientSettings:LogFilePath"] ?? "logs/rolandclient.log"
        };

        // settings.json dosyasından yüklemeyi dene (appsettings.json'dan yüklenmezse)
        if (File.Exists("settings.json"))
        {
            try
            {
                var json = File.ReadAllText("settings.json");
                settings = JsonSerializer.Deserialize<ClientSettings>(json) ?? settings;
            }
            catch (Exception)
            {
                // JSON dosyasını yüklerken hata olursa varsayılan değerleri kullan
            }
        }

        return settings;
    }
    catch
    {
        // Herhangi bir hata olursa varsayılan değerleri içeren yeni bir ayar nesnesi döndür
        return new ClientSettings();
    }
}

// MAC adresi alımı ve formatı düzenlemesi
string GetMacAddress()
{
    string macAddress = System.Net.NetworkInformation.NetworkInterface
        .GetAllNetworkInterfaces()
        .Where(nic => nic.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up 
               && nic.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback)
        .Select(nic => nic.GetPhysicalAddress().ToString())
        .FirstOrDefault() ?? "UNKNOWN_MAC";
        
    // MAC adresini standart formata getir: büyük harf ve iki nokta olmadan
    return macAddress.Replace(":", "").Replace("-", "").ToUpper();
}
