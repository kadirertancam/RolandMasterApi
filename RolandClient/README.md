# Roland VT-4 Client

Bu uygulama, Roland VT-4 cihazını Redis ile haberleşerek API'den gelen komutları dinleyen ve MIDI cihazına ileten bir istemci uygulamasıdır.

## Özellikler

- Redis Pub/Sub ile gerçek zamanlı komut dinleme
- NAudio.Midi kütüphanesi ile MIDI cihaz kontrolü
- Windows Service olarak çalışabilme
- Docker desteği
- Otomatik cihaz kaydı ve keşfi

## Başlangıç

### Gereksinimler

- .NET 8.0 Runtime
- Redis sunucusu
- Roland VT-4 cihazı
- MIDI arabirimi

### Kurulum

1. Uygulamayı derleyin:

```bash
dotnet build
```

2. `appsettings.json` veya `settings.json` dosyasındaki ayarları düzenleyin:

```json
{
  "ClientSettings": {
    "DeviceId": "UNIQUE_ID",  // Benzersiz bir ID (boş bırakılırsa otomatik oluşturulur)
    "DeviceName": "VT4-Client-1",  // İstemci adı
    "RedisConnectionString": "master-api-server:6379",  // Redis bağlantı adresi
    "MidiDeviceId": 1  // MIDI cihaz ID'si (0'dan başlar)
  }
}
```

3. Uygulamayı çalıştırın:

```bash
dotnet run
```

### Windows Service Olarak Yükleme

1. Uygulamayı yayınlayın:

```bash
dotnet publish -c Release -o publish
```

2. Windows Service olarak yükleyin:

```
sc create RolandVT4Client binPath="C:\path\to\publish\RolandClient.exe"
sc start RolandVT4Client
```

## Docker ile Çalıştırma

1. Docker imajını oluşturun:

```bash
docker build -t rolandclient .
```

2. Konteyneri çalıştırın:

```bash
docker run --name roland-client \
  -e "ClientSettings__RedisConnectionString=master-api:6379" \
  -e "ClientSettings__MidiDeviceId=1" \
  --network roland-network \
  rolandclient
```

## Ayarlar

| Ayar | Açıklama | Varsayılan |
|------|----------|------------|
| DeviceId | Cihaz kimliği | Otomatik GUID |
| DeviceName | Cihaz adı | Makine adı |
| MacAddress | MAC adresi | Otomatik algılanır |
| RedisConnectionString | Redis bağlantı adresi | localhost:6379 |
| RedisPassword | Redis parolası | Boş |
| MidiDeviceId | MIDI cihaz ID'si | 1 |
| AutoInitializeMidi | MIDI cihazını otomatik başlat | true |
| EnableDebugLogging | Detaylı log mesajları | false |
| LogFilePath | Log dosyası yolu | logs/rolandclient.log |

## Yapı

- **Worker.cs**: Ana servis sınıfı, arka planda çalışarak API ile iletişime geçer
- **Services/MidiService.cs**: MIDI cihazlarıyla iletişim kurar
- **Services/RedisService.cs**: Redis bağlantısını ve komut dinlemeyi yönetir
- **Services/CommandProcessor.cs**: Komutları işleyerek MIDI cihazına iletir
- **Models/ClientSettings.cs**: İstemci yapılandırma ayarları
- **Models/RolandClientSettings.cs**: Roland cihaz ayarları
- **Models/RolandEffectCommand.cs**: Komut modeli

## Efekt Komutları

İstemci, aşağıdaki efekt komutlarını dinleyebilir:

- **Robot**: Ses robotu efektleri
- **Harmony**: Harmonik efektler
- **Megaphone**: Megafon efektleri 
- **Reverb**: Yankı efektleri
- **Vocoder**: Vocoder efektleri
- **Equalizer**: Ekolayzer ayarları
- **None**: Tüm efektleri kapat
