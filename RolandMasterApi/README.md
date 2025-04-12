# Roland VT-4 Master API

Bu API, çoklu Roland VT-4 cihazlarının merkezi olarak yönetilmesini ve efektlerin eş zamanlı olarak uygulanmasını sağlar.

## Özellikler

- Çoklu Roland VT-4 cihazlarını yönetme
- Redis pub/sub ile gerçek zamanlı komut gönderimi
- Swagger API dokümantasyonu
- Docker desteği
- RESTful API tasarımı

## Başlangıç

### Gereksinimler

- .NET 8.0 SDK
- Redis sunucu
- Çoklu Roland VT-4 istemciler

### Kurulum

1. Repoyu klonlayın veya indirin

2. API'yi derleyin:

```bash
dotnet build
```

3. API'yi çalıştırın:

```bash
dotnet run
```

4. Swagger dokümantasyonuna erişin:

```
https://localhost:5001/swagger
```

### Docker ile Çalıştırma

Docker Compose ile API ve Redis sunucusunu birlikte çalıştırabilirsiniz:

```bash
docker-compose up -d
```

## API Endpoint'leri

### Cihaz Yönetimi

- `GET /api/devices` - Tüm Roland cihazlarını listeler
- `GET /api/devices/{deviceId}` - Belirli bir cihazın ayarlarını getirir
- `POST /api/devices` - Yeni bir cihaz kaydeder
- `PUT /api/devices/{deviceId}` - Bir cihazın ayarlarını günceller
- `DELETE /api/devices/{deviceId}` - Bir cihazı siler

### Komut Yönetimi

- `POST /api/commands` - Bir Roland cihazına komut gönderir
- `POST /api/commands/broadcast` - Tüm cihazlara aynı komutu gönderir
- `GET /api/commands/{commandId}` - Bir komutun durumunu sorgular

### Efekt Kontrolleri (On/Off)

- `POST /api/effect-toggles/robot/{deviceId}/on` - Robot efektini açar
- `POST /api/effect-toggles/robot/{deviceId}/off` - Robot efektini kapatır
- `POST /api/effect-toggles/megaphone/{deviceId}/on` - Megaphone efektini açar
- `POST /api/effect-toggles/megaphone/{deviceId}/off` - Megaphone efektini kapatır
- `POST /api/effect-toggles/vocoder/{deviceId}/on` - Vocoder efektini açar
- `POST /api/effect-toggles/vocoder/{deviceId}/off` - Vocoder efektini kapatır
- `POST /api/effect-toggles/harmony/{deviceId}/on` - Harmony efektini açar
- `POST /api/effect-toggles/harmony/{deviceId}/off` - Harmony efektini kapatır

### Özel Efektler

- `POST /api/special-effects/effect1/{deviceId}` - Efekt 1'i çalıştırır
- `POST /api/special-effects/effect2/{deviceId}` - Efekt 2'yi çalıştırır
- `POST /api/special-effects/effect3/{deviceId}` - Efekt 3'ü çalıştırır
- `POST /api/special-effects/effect4/{deviceId}` - Efekt 4'ü çalıştırır

### Slider Kontrolleri

- `POST /api/sliders/key/{deviceId}` - Key (nota) ayarını değiştirir
- `POST /api/sliders/micsens/{deviceId}` - Mikrofon hassasiyetini ayarlar
- `POST /api/sliders/volume/{deviceId}` - Ses seviyesini ayarlar
- `POST /api/sliders/reverb/{deviceId}` - Reverb seviyesini ayarlar
- `POST /api/sliders/balance/{deviceId}` - Balance seviyesini ayarlar
- `POST /api/sliders/formant/{deviceId}` - Formant seviyesini ayarlar
- `POST /api/sliders/pitch/{deviceId}` - Pitch seviyesini ayarlar

### Efekt Parametreleri

- `POST /api/effects/robot/{deviceId}` - Robot efekt parametrelerini ayarlar
- `POST /api/effects/harmony/{deviceId}` - Harmony efekt parametrelerini ayarlar
- `POST /api/effects/megaphone/{deviceId}` - Megaphone efekt parametrelerini ayarlar
- `POST /api/effects/vocoder/{deviceId}` - Vocoder efekt parametrelerini ayarlar
- `POST /api/effects/reverb/{deviceId}` - Reverb efekt parametrelerini ayarlar

## Örnek Kullanım

### Cihaz Kaydetme

```bash
curl -X POST "https://localhost:5001/api/devices" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceName": "VT-4 Client 1",
    "macAddress": "001122334455",
    "midiDeviceId": 1
  }'
```

### Robot Efektini Açma

```bash
curl -X POST "https://localhost:5001/api/effect-toggles/robot/device-id-123/on" \
  -H "Content-Type: application/json"
```

### Pitch Ayarını Değiştirme

```bash
curl -X POST "https://localhost:5001/api/sliders/pitch/device-id-123" \
  -H "Content-Type: application/json" \
  -d '{
    "value": 8192
  }'
```

### Robot Efekti Parametrelerini Ayarlama

```bash
curl -X POST "https://localhost:5001/api/effects/robot/device-id-123" \
  -H "Content-Type: application/json" \
  -d '{
    "octave": 2,
    "feedbackSwitch": 1,
    "feedbackResonance": 120,
    "feedbackLevel": 160
  }'
```

### Tüm Cihazlara Aynı Efekti Uygulama

```bash
curl -X POST "https://localhost:5001/api/commands/broadcast" \
  -H "Content-Type: application/json" \
  -d '{
    "effectType": "reverb",
    "parameters": {
      "type": 0,
      "param1": 100,
      "param2": 150,
      "param3": 200,
      "param4": 180
    }
  }'
```

## MIDI Komutları

MIDI komutları hakkında detaylı bilgi için `Docs/MidiCommands.md` dosyasına bakabilirsiniz.

## Redis Yapısı

API, verilerini Redis'te aşağıdaki formatta saklar:

- Cihaz Ayarları: `ROLAND:DEVICE:{deviceId}`
- Komutlar: `ROLAND:CMD:{commandId}`
- Komut Kanalı: `ROLAND:COMMANDS`
- Yanıt Kanalı: `ROLAND:RESPONSES`

## Proje Yapısı

- **Controllers/DevicesController.cs**: Cihaz yönetimi
- **Controllers/CommandsController.cs**: Komut yönetimi
- **Controllers/EffectsController.cs**: Efekt parametreleri yönetimi
- **Controllers/EffectToggleController.cs**: Efekt açma/kapama kontrolleri
- **Controllers/SpecialEffectsController.cs**: Özel efekt kontrolleri
- **Controllers/SliderController.cs**: Slider kontrolleri
- **Services/RedisService.cs**: Redis bağlantılarını yönetir
- **Models/RolandClientSettings.cs**: Cihaz ayarları modeli
- **Models/RolandEffectCommand.cs**: Komut modeli
- **Docs/MidiCommands.md**: MIDI komutları dokümantasyonu

## Test

HTTP test dosyası `roland_api_endpoints.http` ile API'yi kolayca test edebilirsiniz. Bu dosya, Visual Studio Code içindeki REST Client eklentisi ile kullanılabilir.

## Yardım

Yardım için Issue açabilir veya geliştirme ekibine ulaşabilirsiniz.