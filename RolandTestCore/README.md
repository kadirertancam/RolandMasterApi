# Roland VT-4 MIDI API

Bu proje, Roland VT-4 ses işlemcisini MIDI SysEx mesajları aracılığıyla kontrol etmek için bir RESTful API sağlar.

## Özellikler

- MIDI cihazlarını listeleme ve başlatma
- Robot, Harmony, Megaphone, Reverb, Vocoder ve Equalizer efektlerini kontrol etme
- Tam Swagger API dokümantasyonu

## Başlarken

### Gereksinimler

- .NET 8.0 SDK
- Roland VT-4 cihazı
- MIDI arabirimi

### Kurulum

1. Projeyi klonlayın veya indirin
2. API'yi çalıştırın:

```bash
cd RolandTestCore
dotnet run
```

3. Tarayıcınızda Swagger dokümantasyonunu açın:

```
https://localhost:5001/swagger
```

## Kullanım

API'yi kullanmak için izlenmesi gereken adımlar:

1. `/api/midi/devices` endpoint'ini çağırarak mevcut MIDI cihazlarını listeleyin
2. `/api/midi/initialize` endpoint'ini çağırarak Roland VT-4 cihazını başlatın
3. Çeşitli efekt endpoint'lerini çağırarak cihazı kontrol edin

Detaylı kullanım kılavuzu için [KullanimKilavuzu.md](./Docs/KullanimKilavuzu.md) dosyasına göz atın.

## API Endpoints

| Endpoint | Metod | Açıklama |
|----------|-------|----------|
| `/api/midi/devices` | GET | Mevcut MIDI cihazlarını listeler |
| `/api/midi/initialize` | POST | MIDI cihazını başlatır |
| `/api/effects/robot` | POST | Robot efektini kontrol eder |
| `/api/effects/harmony` | POST | Harmony efektini kontrol eder |
| `/api/effects/megaphone` | POST | Megafon efektini kontrol eder |
| `/api/effects/reverb` | POST | Reverb efektini kontrol eder |
| `/api/effects/vocoder` | POST | Vocoder efektini kontrol eder |
| `/api/effects/equalizer` | POST | Ekolayzer parametrelerini ayarlar |

## Örnek

Robot efektini uygulamak için:

```bash
curl -X POST "https://localhost:5001/api/effects/robot" \
     -H "Content-Type: application/json" \
     -d "{\"octave\":2,\"feedbackSwitch\":1,\"feedbackResonance\":120,\"feedbackLevel\":200}"
```

## Katkıda Bulunma

1. Bu projeyi fork edin
2. Kendi branch'inizi oluşturun (`git checkout -b ozellik/yeni-ozellik`)
3. Değişikliklerinizi commit edin (`git commit -am 'Yeni özellik: xyz'`)
4. Branch'inizi push edin (`git push origin ozellik/yeni-ozellik`)
5. Bir Pull Request oluşturun

## Lisans

Bu proje MIT lisansı altında lisanslanmıştır.
