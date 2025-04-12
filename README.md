# Roland VT-4 Kontrolör Sistemi

Bu projede birden fazla Roland VT-4 ses kartını merkezi bir API üzerinden yönetebileceğiniz bir sistem bulunmaktadır. Sistem, bir merkezi API ve birden fazla client uygulamasından oluşmaktadır. API ile clientlar arasındaki iletişim Redis üzerinden sağlanır.

## Sistem Bileşenleri

### 1. Roland Master API

API, tüm Roland VT-4 cihazlarının merkezi yönetimini sağlar. Redis üzerinden komutları yayınlar ve cihaz durumlarını takip eder.

- RESTful API tasarımı
- Redis pub/sub ile gerçek zamanlı komut iletimi
- Swagger API dokümantasyonu
- Docker desteği

[API Dokümantasyonu](./RolandMasterApi/README.md)

### 2. Roland Client

Client uygulaması, Roland VT-4 cihazına bağlanarak API'den gelen komutları dinler ve MIDI komutlarını cihaza iletir.

- Windows Service olarak çalışabilme
- NAudio.Midi kütüphanesi ile MIDI cihaz kontrolü
- Redis pub/sub dinleme
- Docker desteği

[Client Dokümantasyonu](./RolandClient/README.md)

## Mimariye Genel Bakış

```
┌─────────────┐           ┌─────────┐            ┌──────────────┐
│ Roland      │           │         │            │ Roland VT-4  │
│ Master API  │ ◄────────►│ Redis   │ ◄─────────►│ Client 1     │
│             │           │         │            │              │
└─────────────┘           └─────────┘            └──────┬───────┘
                              ▲                         │
                              │                         ▼
                              │                  ┌──────────────┐
                              │                  │ Roland VT-4  │
                              └──────────────────┤ Client 2     │
                                                 │              │
                                                 └──────┬───────┘
                                                        │
                                                        ▼
                                                 ┌──────────────┐
                                                 │ Roland VT-4  │
                                                 │ Client N     │
                                                 │              │
                                                 └──────────────┘
```

## Kurulum ve Çalıştırma

### Gereksinimleri

- .NET 8.0 SDK
- Redis sunucu (lokal veya uzak)
- Roland VT-4 cihazları
- MIDI arabirim(ler)

### Docker ile Tüm Sistemi Çalıştırma

```bash
# Repository kök dizininde
docker-compose up -d
```

Bu komut:
1. Redis sunucusunu başlatır
2. Master API'yi oluşturur ve çalıştırır
3. Client servislerini oluşturur ve çalıştırır

### Manuel Kurulum

#### 1. Master API'yi Çalıştırma:

```bash
cd RolandMasterApi
dotnet run
```

API Swagger arayüzü: https://localhost:5001/swagger

#### 2. Client Uygulamasını Çalıştırma:

```bash
cd RolandClient
dotnet run
```

## Kullanım Örneği

1. Birden fazla client uygulamasını farklı MIDI cihazlarına bağlı olarak çalıştırın
2. Master API'yi kullanarak tüm cihazlara aynı anda efekt komutları gönderin:

```bash
# Tüm cihazlara robot efekti uygula
curl -X POST "http://localhost:8080/api/commands/broadcast" \
  -H "Content-Type: application/json" \
  -d '{
    "effectType": "robot",
    "parameters": {
      "octave": 2,
      "feedbackSwitch": 1,
      "feedbackResonance": 120,
      "feedbackLevel": 200
    }
  }'
```

## Özelleştirme

Her client konfigürasyonu `appsettings.json` dosyasında veya ortam değişkenleri ile özelleştirilebilir. Docker Compose kullanıyorsanız, `docker-compose.yml` dosyasında environment değişkenlerini ayarlayabilirsiniz.

## Lisans

Bu proje MIT lisansı altında lisanslanmıştır.
