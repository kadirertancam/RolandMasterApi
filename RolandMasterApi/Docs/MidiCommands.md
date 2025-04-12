# Roland VT-4 MIDI Komutları

Bu belge, Roland VT-4 cihazını kontrol etmek için kullanılan MIDI komutlarını içerir.

## Temel Efekt Kontrolü (On/Off)

| Efekt | Açma Komutu | Kapatma Komutu |
| ----- | ----------- | -------------- |
| Robot | B0 31 7F | B0 31 00 |
| Megaphone | B0 32 7F | B0 32 00 |
| Vocoder | B0 34 7F | B0 34 00 |
| Harmony | B0 35 7F | B0 35 00 |

## Özel Efektler

| Efekt | MIDI Komutu |
| ----- | ----------- |
| Efekt 1 | C0 01 |
| Efekt 2 | C0 02 |
| Efekt 3 | C0 03 |
| Efekt 4 | C0 04 |

## Slider Kontrolleri

| Parametre | MIDI Komutu | Değer Aralığı | Açıklama |
| --------- | ----------- | ------------- | -------- |
| Key (Nota) | B0 30 XX | 00-0B (0-11) | C=00, C#=01, D=02, D#=03, E=04, F=05, F#=06, G=07, G#=08, A=09, A#=0A, B=0B |
| Mikrofon Hassasiyeti | B0 2F XX | 00-7F (0-127) | Düşük-Yüksek |
| Ses Seviyesi | B0 2E XX | 00-7F (0-127) | Sessiz-Maksimum |
| Reverb | B0 39 XX | 00-7F (0-127) | Kuru-Islak |
| Balance | B0 38 XX | 00-7F (0-127) | Sol-Sağ |
| Formant | B0 36 XX | 00-7F (0-127) | Min-Max |
| Pitch | E0 LL MM | 0000-3FFF (0-16383) | 14-bit değer, LL: LSB, MM: MSB, 2000 (8192) merkez değerdir |

## Pitch Slider Değer Örnekleri

```
E0 7F 7E
E0 3F 7E
E0 7E 7D
E0 3E 7D
E0 7E 7C
E0 3E 7C
E0 7D 7B
E0 3D 7B
E0 7D 7A
E0 3D 7A
E0 7C 79
E0 7C 78
E0 3C 78
...
E0 00 00
```

## API Endpoints

Roland Master API'de, bu MIDI komutlarına karşılık gelen aşağıdaki HTTP endpoints bulunmaktadır:

### Efekt On/Off Endpointleri

- Robot efektini aç: `POST /api/effect-toggles/robot/{deviceId}/on`
- Robot efektini kapat: `POST /api/effect-toggles/robot/{deviceId}/off`
- Megaphone efektini aç: `POST /api/effect-toggles/megaphone/{deviceId}/on`
- Megaphone efektini kapat: `POST /api/effect-toggles/megaphone/{deviceId}/off`
- Vocoder efektini aç: `POST /api/effect-toggles/vocoder/{deviceId}/on`
- Vocoder efektini kapat: `POST /api/effect-toggles/vocoder/{deviceId}/off`
- Harmony efektini aç: `POST /api/effect-toggles/harmony/{deviceId}/on`
- Harmony efektini kapat: `POST /api/effect-toggles/harmony/{deviceId}/off`

### Özel Efekt Endpointleri

- Efekt 1'i çalıştır: `POST /api/special-effects/effect1/{deviceId}`
- Efekt 2'yi çalıştır: `POST /api/special-effects/effect2/{deviceId}`
- Efekt 3'ü çalıştır: `POST /api/special-effects/effect3/{deviceId}`
- Efekt 4'ü çalıştır: `POST /api/special-effects/effect4/{deviceId}`

### Slider Kontrol Endpointleri

- Key (nota) ayarı: `POST /api/sliders/key/{deviceId}`
- Mikrofon hassasiyeti: `POST /api/sliders/micsens/{deviceId}`
- Ses seviyesi: `POST /api/sliders/volume/{deviceId}`
- Reverb seviyesi: `POST /api/sliders/reverb/{deviceId}`
- Balance seviyesi: `POST /api/sliders/balance/{deviceId}`
- Formant seviyesi: `POST /api/sliders/formant/{deviceId}`
- Pitch seviyesi: `POST /api/sliders/pitch/{deviceId}`

### Efekt Parametreleri Endpointleri

- Robot efekti parametreleri: `POST /api/effects/robot/{deviceId}`
- Harmony efekti parametreleri: `POST /api/effects/harmony/{deviceId}`
- Megaphone efekti parametreleri: `POST /api/effects/megaphone/{deviceId}`
- Vocoder efekti parametreleri: `POST /api/effects/vocoder/{deviceId}`
- Reverb efekti parametreleri: `POST /api/effects/reverb/{deviceId}`

### Komut Endpointleri

- Komut durumu sorgulama: `GET /api/commands/{commandId}`
- Genel komut gönderme: `POST /api/commands`
- Broadcast komut gönderme: `POST /api/commands/broadcast`