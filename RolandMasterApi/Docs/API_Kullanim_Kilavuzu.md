# Roland VT-4 Ses Efekt Yönetim API Kullanım Kılavuzu

## İçindekiler

1. [Giriş](#giriş)
2. [API Genel Bakış](#api-genel-bakış)
3. [Başlarken](#başlarken)
4. [Kimlik Doğrulama](#kimlik-doğrulama)
5. [Endpoint'ler](#endpointler)
   - [Cihaz Yönetimi](#cihaz-yönetimi)
   - [Komut Yönetimi](#komut-yönetimi)
   - [Efekt Komutları](#efekt-komutları)
6. [Örnekler](#örnekler)
7. [Hata Kodları](#hata-kodları)
8. [SSS](#sss)
9. [İletişim](#iletişim)

## Giriş

Roland VT-4 Ses Efekt Yönetim API'si, birden fazla Roland VT-4 ses efekt cihazının merkezi bir sistem üzerinden yönetilmesini sağlar. Bu API sayesinde, tüm cihazlara aynı anda efekt gönderebilir, cihazların durumlarını kontrol edebilir ve cihazları kayıt edebilirsiniz.

Bu dokümantasyon, API'nin nasıl kullanılacağını, hangi endpoint'lerin olduğunu ve hangi parametreleri desteklediğini detaylı olarak açıklar.

## API Genel Bakış

API, RESTful prensiplerine göre tasarlanmıştır ve aşağıdaki ana bileşenleri içerir:

- **Cihaz Yönetimi**: Cihazların kaydedilmesi, listelenmesi, güncellenmesi ve silinmesi
- **Komut Yönetimi**: Cihazlara komut gönderilmesi, komut durumlarının sorgulanması
- **Efekt Komutları**: Belirli efektlerin cihazlara uygulanması (Robot, Harmony, Megaphone, Reverb, Vocoder, Equalizer)
- **Broadcast Komutları**: Tüm cihazlara aynı anda komut gönderilmesi

Tüm API yanıtları JSON formatındadır.

## Başlarken

### Gereksinimler

- HTTP istemcisi (Postman, curl, veya herhangi bir programlama dili)
- API'ye erişim yetkisi
- Geçerli bir Roland VT-4 cihazı ve cihaz ID'si

### Temel URL

API'nin temel URL'i:

```
http://api-server:8080
```

API'yi yerel ortamda çalıştırıyorsanız:

```
http://localhost:8080
```

## Kimlik Doğrulama

Bu API sürümünde kimlik doğrulama zorunlu değildir. Ancak üretime alınırken güvenlik nedeniyle bir kimlik doğrulama mekanizması eklenecektir.

## Endpoint'ler

### Cihaz Yönetimi

#### Tüm cihazları listele

```
GET /api/devices
```

**Yanıt:**
```json
[
  {
    "deviceId": "device-001",
    "deviceName": "VT-4 Studio 1",
    "macAddress": "00:11:22:33:44:55",
    "isActive": true,
    "midiDeviceId": 1,
    "activeEffect": "none"
  },
  {
    "deviceId": "device-002",
    "deviceName": "VT-4 Studio 2",
    "macAddress": "AA:BB:CC:DD:EE:FF",
    "isActive": true,
    "midiDeviceId": 1,
    "activeEffect": "robot"
  }
]
```

#### Cihaz detaylarını getir

```
GET /api/devices/{deviceId}
```

**Parametreler:**
- `deviceId`: Cihazın benzersiz ID'si

**Yanıt:**
```json
{
  "deviceId": "device-001",
  "deviceName": "VT-4 Studio 1",
  "macAddress": "00:11:22:33:44:55",
  "isActive": true,
  "midiDeviceId": 1,
  "activeEffect": "none",
  "robotOctave": 2,
  "robotFeedbackSwitch": 1,
  "robotFeedbackResonance": 120,
  "robotFeedbackLevel": 160,
  "harmonyH1Level": 200,
  "harmonyH2Level": 150,
  "harmonyH3Level": 100,
  "harmonyH1Key": 0,
  "harmonyH2Key": 4,
  "harmonyH3Key": 7,
  "harmonyH1Gender": 128,
  "harmonyH2Gender": 128,
  "harmonyH3Gender": 128,
  "megaphoneType": 0,
  "megaphoneParam1": 100,
  "megaphoneParam2": 150,
  "megaphoneParam3": 200,
  "megaphoneParam4": 180,
  "reverbType": 0,
  "reverbParam1": 100,
  "reverbParam2": 150,
  "reverbParam3": 200,
  "reverbParam4": 180,
  "vocoderType": 0,
  "vocoderParam1": 100,
  "vocoderParam2": 150,
  "vocoderParam3": 200,
  "vocoderParam4": 180,
  "equalizerSwitch": 1,
  "equalizerLowShelfFreq": 40,
  "equalizerLowShelfGain": 20,
  "equalizerLowMidFreq": 60,
  "equalizerLowMidQ": 70,
  "equalizerLowMidGain": 15,
  "equalizerHighMidFreq": 80,
  "equalizerHighMidQ": 70,
  "equalizerHighMidGain": 10,
  "equalizerHighShelfFreq": 100,
  "equalizerHighShelfGain": 20,
  "masterVolume": 100,
  "isMuted": false
}
```

#### Cihaz ekle

```
POST /api/devices
```

**İstek Gövdesi:**
```json
{
  "deviceName": "VT-4 Studio 3",
  "macAddress": "11:22:33:44:55:66",
  "midiDeviceId": 1
}
```

**Yanıt:**
```json
{
  "deviceId": "device-003",
  "deviceName": "VT-4 Studio 3",
  "macAddress": "11:22:33:44:55:66",
  "isActive": true,
  "midiDeviceId": 1,
  "activeEffect": "none"
}
```

#### Cihaz güncelle

```
PUT /api/devices/{deviceId}
```

**Parametreler:**
- `deviceId`: Güncellenecek cihazın ID'si

**İstek Gövdesi:**
```json
{
  "deviceName": "VT-4 Studio 3 (Updated)",
  "isActive": true,
  "midiDeviceId": 2
}
```

**Yanıt:**
```json
{
  "deviceId": "device-003",
  "deviceName": "VT-4 Studio 3 (Updated)",
  "macAddress": "11:22:33:44:55:66",
  "isActive": true,
  "midiDeviceId": 2,
  "activeEffect": "none"
}
```

#### Cihaz sil

```
DELETE /api/devices/{deviceId}
```

**Parametreler:**
- `deviceId`: Silinecek cihazın ID'si

**Yanıt:**
```
204 No Content
```

### Komut Yönetimi

#### Komut gönder

```
POST /api/commands
```

**İstek Gövdesi:**
```json
{
  "targetDeviceId": "device-001",
  "effectType": "robot",
  "parameters": {
    "octave": 2,
    "feedbackSwitch": 1,
    "feedbackResonance": 120,
    "feedbackLevel": 160
  }
}
```

**Yanıt:**
```json
{
  "commandId": "cmd-12345",
  "message": "Komut işleme alındı"
}
```

#### Broadcast komut gönder (tüm cihazlara)

```
POST /api/commands/broadcast
```

**İstek Gövdesi:**
```json
{
  "effectType": "reverb",
  "parameters": {
    "type": 0,
    "param1": 100,
    "param2": 150,
    "param3": 200,
    "param4": 180
  }
}
```

**Yanıt:**
```json
{
  "commandId": "cmd-67890",
  "message": "Broadcast komut işleme alındı"
}
```

#### Komut durumunu sorgula

```
GET /api/commands/{commandId}
```

**Parametreler:**
- `commandId`: Sorgulanacak komutun ID'si

**Yanıt:**
```json
{
  "commandId": "cmd-12345",
  "targetDeviceId": "device-001",
  "effectType": "robot",
  "parameters": {
    "octave": 2,
    "feedbackSwitch": 1,
    "feedbackResonance": 120,
    "feedbackLevel": 160
  },
  "status": "completed",
  "createdAt": "2025-04-08T23:45:12Z",
  "isBroadcast": false
}
```

### Efekt Komutları

#### Robot Efekti

```
POST /api/commands/robot/{deviceId}
```

**Parametreler:**
- `deviceId`: Hedef cihazın ID'si

**İstek Gövdesi:**
```json
{
  "octave": 2,
  "feedbackSwitch": 1,
  "feedbackResonance": 120,
  "feedbackLevel": 160
}
```

#### Harmony Efekti

```
POST /api/commands/harmony/{deviceId}
```

**Parametreler:**
- `deviceId`: Hedef cihazın ID'si

**İstek Gövdesi:**
```json
{
  "h1Level": 200,
  "h2Level": 150,
  "h3Level": 100,
  "h1Key": 0,
  "h2Key": 4,
  "h3Key": 7,
  "h1Gender": 128,
  "h2Gender": 128,
  "h3Gender": 128
}
```

#### Megaphone Efekti

```
POST /api/commands/megaphone/{deviceId}
```

**Parametreler:**
- `deviceId`: Hedef cihazın ID'si

**İstek Gövdesi:**
```json
{
  "type": 0,
  "param1": 100,
  "param2": 150,
  "param3": 200,
  "param4": 180
}
```

#### Reverb Efekti

```
POST /api/commands/reverb/{deviceId}
```

**Parametreler:**
- `deviceId`: Hedef cihazın ID'si

**İstek Gövdesi:**
```json
{
  "type": 0,
  "param1": 100,
  "param2": 150,
  "param3": 200,
  "param4": 180
}
```

#### Vocoder Efekti

```
POST /api/commands/vocoder/{deviceId}
```

**Parametreler:**
- `deviceId`: Hedef cihazın ID'si

**İstek Gövdesi:**
```json
{
  "type": 0,
  "param1": 100,
  "param2": 150,
  "param3": 200,
  "param4": 180
}
```

#### Equalizer Efekti

```
POST /api/commands/equalizer/{deviceId}
```

**Parametreler:**
- `deviceId`: Hedef cihazın ID'si

**İstek Gövdesi:**
```json
{
  "eqSwitch": 1,
  "lowShelfFreq": 40,
  "lowShelfGain": 20,
  "lowMidFreq": 60,
  "lowMidQ": 70,
  "lowMidGain": 15,
  "highMidFreq": 80,
  "highMidQ": 70,
  "highMidGain": 10,
  "highShelfFreq": 100,
  "highShelfGain": 20
}
```

## Örnekler

### cURL ile Komut Gönderme

```bash
curl -X POST "http://localhost:8080/api/commands/robot/device-001" \
  -H "Content-Type: application/json" \
  -d '{
    "octave": 2,
    "feedbackSwitch": 1,
    "feedbackResonance": 120,
    "feedbackLevel": 160
  }'
```

### JavaScript (fetch) ile Komut Gönderme

```javascript
fetch('http://localhost:8080/api/commands/robot/device-001', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({
    octave: 2,
    feedbackSwitch: 1,
    feedbackResonance: 120,
    feedbackLevel: 160
  }),
})
.then(response => response.json())
.then(data => console.log(data))
.catch((error) => console.error('Hata:', error));
```

### C# ile Komut Gönderme

```csharp
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri("http://localhost:8080");

        var robotCommand = new
        {
            octave = 2,
            feedbackSwitch = 1,
            feedbackResonance = 120,
            feedbackLevel = 160
        };

        var json = JsonSerializer.Serialize(robotCommand);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/commands/robot/device-001", content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Başarılı: {responseContent}");
        }
        else
        {
            Console.WriteLine($"Hata: {response.StatusCode}");
        }
    }
}
```

## Hata Kodları

API aşağıdaki HTTP durum kodlarını kullanabilir:

| Durum Kodu | Açıklama |
|------------|----------|
| 200 OK | İstek başarılı |
| 201 Created | Kaynak başarıyla oluşturuldu |
| 202 Accepted | İstek kabul edildi ve işleme alındı |
| 204 No Content | İşlem başarılı, dönecek içerik yok |
| 400 Bad Request | Geçersiz istek veya parametreler |
| 404 Not Found | İstenen kaynak bulunamadı |
| 500 Internal Server Error | Sunucu hatası |

Hata yanıtları aşağıdaki formatta olacaktır:

```json
{
  "message": "Hata açıklaması"
}
```

## SSS

### Komut gönderdiğim cihaz çevrimiçi değilse ne olur?
Komut Redis'e kaydedilir ve cihaz çevrimiçi olduğunda işlenir. Komut durumunu sorguladığınızda "pending" durumunda görünecektir.

### Bir cihaza birden fazla komut gönderebilir miyim?
Evet, komutlar sırayla işlenir.

### Cihaz ID'sini nereden bulabilirim?
Cihaz ID'si, cihazı API'ye kaydederken otomatik olarak oluşturulur ve yanıtta döndürülür. Ayrıca tüm cihazları listeleyerek de görebilirsiniz.

### API'yi yerel ağımda çalıştırabilir miyim?
Evet, API'yi Docker kullanarak veya doğrudan .NET SDK ile yerel ağınızda çalıştırabilirsiniz.

## İletişim

Sorularınız veya önerileriniz için:

- E-posta: destek@rolandvt4.com
- Web sitesi: https://rolandvt4.com/iletisim
