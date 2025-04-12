# Roland VT-4 MIDI API Kullanım Kılavuzu

Bu döküman, Roland VT-4 ses efekt cihazını kontrol etmek için oluşturulan API'nin nasıl kullanılacağını açıklamaktadır.

## İçindekiler

1. [Genel Bakış](#genel-bakış)
2. [Başlangıç](#başlangıç)
   - [Gereksinimler](#gereksinimler)
   - [API'nin Çalıştırılması](#apinin-çalıştırılması)
3. [API Kullanımı](#api-kullanımı)
   - [MIDI Cihaz Yönetimi](#midi-cihaz-yönetimi)
   - [Efekt Kontrolleri](#efekt-kontrolleri)
4. [Örnek İstekler](#örnek-istekler)
5. [Sık Sorulan Sorular](#sık-sorulan-sorular)
6. [Sorun Giderme](#sorun-giderme)

## Genel Bakış

Roland VT-4 MIDI API, Roland VT-4 ses işlemcisini MIDI SysEx mesajları aracılığıyla programatik olarak kontrol etmek için bir RESTful API sağlar. Bu API sayesinde:

- MIDI cihazlarını listeleme ve başlatma
- Robot, Harmony, Megaphone, Reverb, Vocoder ve Equalizer efektlerini kontrol etme
- SysEx mesajlarıyla doğrudan iletişim kurma

işlemleri gerçekleştirilebilir.

## Başlangıç

### Gereksinimler

API'yi kullanmak için aşağıdaki gereksinimlere ihtiyacınız vardır:

- .NET 8.0 SDK veya daha yeni sürümü
- Roland VT-4 cihazı
- VT-4 cihazının bağlı olduğu bir MIDI arabirimi
- Postman, cURL veya API istekleri yapabilen herhangi bir araç

### API'nin Çalıştırılması

1. Proje klasörüne gidin:
   ```
   cd C:\Users\kadir\source\repos\RolandTestCore
   ```

2. API'yi çalıştırın:
   ```
   dotnet run
   ```

3. API varsayılan olarak şu adreslerde çalışır:
   - HTTP: `http://localhost:5000`
   - HTTPS: `https://localhost:5001`

4. Swagger dokümantasyonuna erişmek için tarayıcınızda şu adresi açın:
   ```
   https://localhost:5001/swagger
   ```

## API Kullanımı

### MIDI Cihaz Yönetimi

API'yi kullanmadan önce, bir MIDI cihazını başlatmanız gerekir. Bu işlem iki adımda gerçekleştirilir:

#### 1. Mevcut MIDI Cihazlarını Listeleme

```
GET /api/midi/devices
```

Bu endpoint, sisteminizde mevcut olan tüm MIDI çıkış cihazlarını listeler.

**Örnek Yanıt:**
```json
[
  {
    "id": 0,
    "name": "Microsoft GS Wavetable Synth"
  },
  {
    "id": 1,
    "name": "Roland VT-4"
  }
]
```

#### 2. MIDI Cihazını Başlatma

```
POST /api/midi/initialize
```

**İstek gövdesi:**
```json
{
  "deviceId": 1
}
```

Bu endpoint, belirtilen ID'ye sahip MIDI cihazını başlatır. `deviceId` parametresi, bir önceki adımda alınan listeden seçilmelidir.

**Örnek Yanıt:**
```json
{
  "message": "MIDI cihazı 1 başarıyla başlatıldı"
}
```

### Efekt Kontrolleri

API, Roland VT-4'teki tüm efektleri kontrol etmek için uç noktalar sunar. Her efekt için ayrı bir endpoint bulunmaktadır.

#### Robot Efekti

```
POST /api/effects/robot
```

**İstek gövdesi:**
```json
{
  "octave": 2,
  "feedbackSwitch": 1,
  "feedbackResonance": 120,
  "feedbackLevel": 200
}
```

**Parametre açıklamaları:**
- `octave`: Oktav ayarı (0-3)
  - 0: 2AŞAĞI
  - 1: AŞAĞI
  - 2: SIFIR
  - 3: YUKARI
- `feedbackSwitch`: Geri besleme açık/kapalı (0-1)
- `feedbackResonance`: Geri besleme rezonans değeri (0-255)
- `feedbackLevel`: Geri besleme seviyesi (0-255)

#### Harmony Efekti

```
POST /api/effects/harmony
```

**İstek gövdesi:**
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

**Parametre açıklamaları:**
- `h1Level`, `h2Level`, `h3Level`: Harmony kanallarının seviyeleri (0-255)
- `h1Key`, `h2Key`, `h3Key`: Harmony kanallarının tonları (0-11)
  - 0: C, 1: C#, 2: D, 3: D#, 4: E, 5: F, 6: F#, 7: G, 8: G#, 9: A, 10: A#, 11: B
- `h1Gender`, `h2Gender`, `h3Gender`: Harmony kanallarının cinsiyet parametreleri (0-255)

#### Megafon Efekti

```
POST /api/effects/megaphone
```

**İstek gövdesi:**
```json
{
  "type": 0,
  "param1": 100,
  "param2": 150,
  "param3": 200,
  "param4": 180
}
```

**Parametre açıklamaları:**
- `type`: Megafon tipi (0-3)
  - 0: Megafon
  - 1: Radyo
  - 2: BBD Chorus
  - 3: Strobo
- `param1`, `param2`, `param3`, `param4`: Megafon parametreleri (0-255)
  - Bu parametrelerin anlamı seçilen megafon tipine göre değişir

#### Reverb Efekti

```
POST /api/effects/reverb
```

**İstek gövdesi:**
```json
{
  "type": 0,
  "param1": 100,
  "param2": 150,
  "param3": 200,
  "param4": 180
}
```

**Parametre açıklamaları:**
- `type`: Reverb tipi (0-5)
  - 0: Reverb
  - 1: Echo
  - 2: Delay
  - 3: Dub Echo
  - 4: Deep Reverb
  - 5: VT Reverb
- `param1`, `param2`, `param3`, `param4`: Reverb parametreleri (0-255)
  - Bu parametrelerin anlamı seçilen reverb tipine göre değişir

#### Vocoder Efekti

```
POST /api/effects/vocoder
```

**İstek gövdesi:**
```json
{
  "type": 0,
  "param1": 100,
  "param2": 150,
  "param3": 200,
  "param4": 180
}
```

**Parametre açıklamaları:**
- `type`: Vocoder tipi (0-4)
  - 0: Vintage
  - 1: Advanced
  - 2: Talk Box
  - 3: Spell Toy
- `param1`, `param2`, `param3`, `param4`: Vocoder parametreleri (0-255)
  - Bu parametrelerin anlamı seçilen vocoder tipine göre değişir

#### Ekolayzer Efekti

```
POST /api/effects/equalizer
```

**İstek gövdesi:**
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

**Parametre açıklamaları:**
- `eqSwitch`: Ekolayzer açık/kapalı (0-1)
- `lowShelfFreq`: Düşük raf frekansı (0-127)
- `lowShelfGain`: Düşük raf kazancı (0-40)
- `lowMidFreq`: Düşük-orta frekans (0-127)
- `lowMidQ`: Düşük-orta Q (0-127)
- `lowMidGain`: Düşük-orta kazanç (0-40)
- `highMidFreq`: Yüksek-orta frekans (0-127)
- `highMidQ`: Yüksek-orta Q (0-127)
- `highMidGain`: Yüksek-orta kazanç (0-40)
- `highShelfFreq`: Yüksek raf frekansı (0-127)
- `highShelfGain`: Yüksek raf kazancı (0-40)

## Örnek İstekler

Aşağıda API'yi kullanmak için bazı örnek istekler bulunmaktadır. Bu örnekler, cURL komutları olarak verilmiştir:

### 1. MIDI Cihazlarını Listeleme

```bash
curl -X GET "https://localhost:5001/api/midi/devices" -H "accept: application/json"
```

### 2. MIDI Cihazını Başlatma

```bash
curl -X POST "https://localhost:5001/api/midi/initialize" -H "accept: application/json" -H "Content-Type: application/json" -d "{\"deviceId\":1}"
```

### 3. Robot Efektini Uygulama

```bash
curl -X POST "https://localhost:5001/api/effects/robot" -H "accept: application/json" -H "Content-Type: application/json" -d "{\"octave\":2,\"feedbackSwitch\":1,\"feedbackResonance\":120,\"feedbackLevel\":200}"
```

### 4. Harmony Efektini Uygulama

```bash
curl -X POST "https://localhost:5001/api/effects/harmony" -H "accept: application/json" -H "Content-Type: application/json" -d "{\"h1Level\":200,\"h2Level\":150,\"h3Level\":100,\"h1Key\":0,\"h2Key\":4,\"h3Key\":7,\"h1Gender\":128,\"h2Gender\":128,\"h3Gender\":128}"
```

## Sık Sorulan Sorular

### API'yi kullanmak için VT-4 cihazım açık olmalı mı?
Evet, API'nin çalışması için VT-4 cihazının bilgisayara bağlı ve açık olması gerekir.

### Birden fazla MIDI cihazı bağlıysa ne yapmalıyım?
`/api/midi/devices` endpoint'ini kullanarak tüm cihazları listeleyin ve ardından doğru cihaz ID'sini kullanarak `/api/midi/initialize` endpoint'i ile VT-4 cihazını başlatın.

### API yanıt vermediğinde ne yapmalıyım?
Önce VT-4 cihazınızın düzgün bağlandığından ve açık olduğundan emin olun. Ardından, API servisinin çalıştığını kontrol edin.

### Parametreleri yanlış gönderirsem ne olur?
API, parametre doğrulaması yapar ve geçersiz parametreler gönderildiğinde 400 Bad Request hatası döndürür. Hata mesajı, hangi parametrenin hatalı olduğunu açıklar.

## Sorun Giderme

### Hata: "MIDI cihazı bulunamadı"
Bu hata, sistemde hiç MIDI cihazı olmadığını gösterir. VT-4'ün düzgün şekilde bağlandığından emin olun ve gerekirse bilgisayarınızı yeniden başlatın.

### Hata: "MIDI cihazı başlatılamadı"
Bu hata, seçilen MIDI cihazının başlatılamadığını gösterir. Cihaz ID'sinin doğru olduğundan ve cihazın başka bir uygulama tarafından kullanılmadığından emin olun.

### Hata: "Mesaj gönderilemedi"
Bu hata, MIDI mesajı gönderilirken bir sorun oluştuğunu gösterir. VT-4 cihazınızın hala bağlı olduğunu ve doğru şekilde yapılandırıldığını kontrol edin.

---

Bu API, Roland VT-4 cihazınızı programlama yoluyla kontrol etmenizi sağlar. Herhangi bir sorunla karşılaşırsanız veya daha fazla bilgiye ihtiyaç duyarsanız, lütfen iletişime geçin.
