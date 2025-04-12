# Roland Master API Kullanım Örnekleri

Bu döküman, Roland Master API'nin çeşitli senaryolarda nasıl kullanılacağına dair örnekler içerir.

## İçindekiler

- [Temel Efekt Kontrolü](#temel-efekt-kontrolü)
- [Slider Kontrolü](#slider-kontrolü)
- [Özel Efektler](#özel-efektler)
- [Gelişmiş Efekt Parametreleri](#gelişmiş-efekt-parametreleri)
- [Broadcast Kullanımı](#broadcast-kullanımı)
- [JavaScript/Fetch API ile Kullanım](#javascriptfetch-api-ile-kullanım)
- [C# ile Kullanım](#c-ile-kullanım)
- [Python ile Kullanım](#python-ile-kullanım)

## Temel Efekt Kontrolü

### Robot Efektini Açma

```bash
curl -X POST "https://localhost:5001/api/effect-toggles/robot/00155D4FA27F/on" \
  -H "Content-Type: application/json"
```

### Robot Efektini Kapatma

```bash
curl -X POST "https://localhost:5001/api/effect-toggles/robot/00155D4FA27F/off" \
  -H "Content-Type: application/json"
```

### Harmony Efektini Açma

```bash
curl -X POST "https://localhost:5001/api/effect-toggles/harmony/00155D4FA27F/on" \
  -H "Content-Type: application/json"
```

## Slider Kontrolü

### Pitch Ayarını Değiştirme

```bash
# Normale döndürme (merkez)
curl -X POST "https://localhost:5001/api/sliders/pitch/00155D4FA27F" \
  -H "Content-Type: application/json" \
  -d '{
    "value": 8192
  }'

# Daha yüksek ses (bir oktav yukarıda)
curl -X POST "https://localhost:5001/api/sliders/pitch/00155D4FA27F" \
  -H "Content-Type: application/json" \
  -d '{
    "value": 16383
  }'

# Daha düşük ses (bir oktav aşağıda)
curl -X POST "https://localhost:5001/api/sliders/pitch/00155D4FA27F" \
  -H "Content-Type: application/json" \
  -d '{
    "value": 0
  }'
```

### Key (Nota) Ayarını Değiştirme

```bash
# A notasına ayarlama
curl -X POST "https://localhost:5001/api/sliders/key/00155D4FA27F" \
  -H "Content-Type: application/json" \
  -d '{
    "value": 9
  }'

# C notasına ayarlama
curl -X POST "https://localhost:5001/api/sliders/key/00155D4FA27F" \
  -H "Content-Type: application/json" \
  -d '{
    "value": 0
  }'
```

### Formant Ayarını Değiştirme

```bash
curl -X POST "https://localhost:5001/api/sliders/formant/00155D4FA27F" \
  -H "Content-Type: application/json" \
  -d '{
    "value": 100
  }'
```

### Balance Ayarını Değiştirme

```bash
# Merkez
curl -X POST "https://localhost:5001/api/sliders/balance/00155D4FA27F" \
  -H "Content-Type: application/json" \
  -d '{
    "value": 64
  }'

# Sol tarafa ağırlık
curl -X POST "https://localhost:5001/api/sliders/balance/00155D4FA27F" \
  -H "Content-Type: application/json" \
  -d '{
    "value": 20
  }'

# Sağ tarafa ağırlık
curl -X POST "https://localhost:5001/api/sliders/balance/00155D4FA27F" \
  -H "Content-Type: application/json" \
  -d '{
    "value": 100
  }'
```

## Özel Efektler

### Efekt 1'i Çalıştırma

```bash
curl -X POST "https://localhost:5001/api/special-effects/effect1/00155D4FA27F" \
  -H "Content-Type: application/json"
```

### Efekt 2'yi Çalıştırma

```bash
curl -X POST "https://localhost:5001/api/special-effects/effect2/00155D4FA27F" \
  -H "Content-Type: application/json"
```

## Gelişmiş Efekt Parametreleri

### Robot Efekti Parametrelerini Ayarlama

```bash
curl -X POST "https://localhost:5001/api/effects/robot/00155D4FA27F" \
  -H "Content-Type: application/json" \
  -d '{
    "octave": 2,
    "feedbackSwitch": 1,
    "feedbackResonance": 120,
    "feedbackLevel": 160
  }'
```

### Harmony Efekti Parametrelerini Ayarlama

```bash
curl -X POST "https://localhost:5001/api/effects/harmony/00155D4FA27F" \
  -H "Content-Type: application/json" \
  -d '{
    "h1Level": 200,
    "h2Level": 150,
    "h3Level": 100,
    "h1Key": 0,
    "h2Key": 4,
    "h3Key": 7,
    "h1Gender": 128,
    "h2Gender": 128,
    "h3Gender": 128
  }'
```

### Megaphone Efekti Parametrelerini Ayarlama

```bash
curl -X POST "https://localhost:5001/api/effects/megaphone/00155D4FA27F" \
  -H "Content-Type: application/json" \
  -d '{
    "type": 0,
    "param1": 100,
    "param2": 150,
    "param3": 200,
    "param4": 180
  }'
```

## Broadcast Kullanımı

Broadcast, tüm kayıtlı cihazlara aynı komutu göndermek için kullanılır.

### Tüm Cihazlara Reverb Efekti Uygulama

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

### Tüm Cihazları Sıfırlama

```bash
curl -X POST "https://localhost:5001/api/commands/broadcast" \
  -H "Content-Type: application/json" \
  -d '{
    "effectType": "reset",
    "parameters": {}
  }'
```

## JavaScript/Fetch API ile Kullanım

Modern web uygulamalarında API'yi kullanmak için Fetch API kullanabilirsiniz:

```javascript
// Robot efektini açma
async function turnOnRobotEffect(deviceId) {
  try {
    const response = await fetch(`https://localhost:5001/api/effect-toggles/robot/${deviceId}/on`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      }
    });
    
    const data = await response.json();
    console.log('Robot effect turned on:', data);
    return data;
  } catch (error) {
    console.error('Error turning on robot effect:', error);
    throw error;
  }
}

// Pitch değerini ayarlama
async function setPitchValue(deviceId, value) {
  try {
    const response = await fetch(`https://localhost:5001/api/sliders/pitch/${deviceId}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ value })
    });
    
    const data = await response.json();
    console.log('Pitch value set:', data);
    return data;
  } catch (error) {
    console.error('Error setting pitch value:', error);
    throw error;
  }
}

// Kullanım örneği
turnOnRobotEffect('00155D4FA27F')
  .then(() => setPitchValue('00155D4FA27F', 10000))
  .catch(err => console.error('API Error:', err));
```

## C# ile Kullanım

.NET uygulamalarında HttpClient ile API'yi şu şekilde kullanabilirsiniz:

```csharp
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class RolandApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    
    public RolandApiClient(string baseUrl = "https://localhost:5001")
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }
    
    // Robot efektini açma
    public async Task<string> TurnOnRobotEffectAsync(string deviceId)
    {
        var response = await _httpClient.PostAsync(
            $"{_baseUrl}/api/effect-toggles/robot/{deviceId}/on", 
            new StringContent("{}", Encoding.UTF8, "application/json"));
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
    
    // Pitch değerini ayarlama
    public async Task<string> SetPitchValueAsync(string deviceId, int value)
    {
        var content = new
        {
            value
        };
        
        var json = JsonSerializer.Serialize(content);
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync(
            $"{_baseUrl}/api/sliders/pitch/{deviceId}", 
            stringContent);
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
    
    // Harmony parametrelerini ayarlama
    public async Task<string> SetHarmonyParametersAsync(string deviceId, object parameters)
    {
        var json = JsonSerializer.Serialize(parameters);
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync(
            $"{_baseUrl}/api/effects/harmony/{deviceId}", 
            stringContent);
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}

// Kullanım örneği
public class Program
{
    public static async Task Main()
    {
        var client = new RolandApiClient();
        
        try
        {
            // Robot efektini aç
            string result1 = await client.TurnOnRobotEffectAsync("00155D4FA27F");
            Console.WriteLine($"Robot effect turned on: {result1}");
            
            // Pitch değerini ayarla
            string result2 = await client.SetPitchValueAsync("00155D4FA27F", 10000);
            Console.WriteLine($"Pitch value set: {result2}");
            
            // Harmony parametrelerini ayarla
            var harmonyParams = new
            {
                h1Level = 200,
                h2Level = 150,
                h3Level = 100,
                h1Key = 0,
                h2Key = 4,
                h3Key = 7,
                h1Gender = 128,
                h2Gender = 128,
                h3Gender = 128
            };
            
            string result3 = await client.SetHarmonyParametersAsync("00155D4FA27F", harmonyParams);
            Console.WriteLine($"Harmony parameters set: {result3}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
```

## Python ile Kullanım

Python uygulamalarında requests kütüphanesi ile API'yi şu şekilde kullanabilirsiniz:

```python
import requests
import json

class RolandApiClient:
    def __init__(self, base_url="https://localhost:5001"):
        self.base_url = base_url
        
    def turn_on_robot_effect(self, device_id):
        """Robot efektini açar"""
        url = f"{self.base_url}/api/effect-toggles/robot/{device_id}/on"
        response = requests.post(url, json={})
        response.raise_for_status()
        return response.json()
    
    def turn_off_robot_effect(self, device_id):
        """Robot efektini kapatır"""
        url = f"{self.base_url}/api/effect-toggles/robot/{device_id}/off"
        response = requests.post(url, json={})
        response.raise_for_status()
        return response.json()
    
    def set_pitch_value(self, device_id, value):
        """Pitch değerini ayarlar"""
        url = f"{self.base_url}/api/sliders/pitch/{device_id}"
        response = requests.post(url, json={"value": value})
        response.raise_for_status()
        return response.json()
    
    def trigger_effect(self, device_id, effect_number):
        """Özel efekti tetikler (1-4)"""
        url = f"{self.base_url}/api/special-effects/effect{effect_number}/{device_id}"
        response = requests.post(url, json={})
        response.raise_for_status()
        return response.json()
    
    def set_harmony_parameters(self, device_id, params):
        """Harmony efekt parametrelerini ayarlar"""
        url = f"{self.base_url}/api/effects/harmony/{device_id}"
        response = requests.post(url, json=params)
        response.raise_for_status()
        return response.json()
    
    def broadcast_command(self, effect_type, parameters):
        """Tüm cihazlara aynı komutu gönderir"""
        url = f"{self.base_url}/api/commands/broadcast"
        data = {
            "effectType": effect_type,
            "parameters": parameters
        }
        response = requests.post(url, json=data)
        response.raise_for_status()
        return response.json()

# Kullanım örneği
if __name__ == "__main__":
    client = RolandApiClient()
    device_id = "00155D4FA27F"
    
    try:
        # Robot efektini aç
        result1 = client.turn_on_robot_effect(device_id)
        print(f"Robot effect turned on: {result1}")
        
        # Pitch değerini ayarla
        result2 = client.set_pitch_value(device_id, 10000)
        print(f"Pitch value set: {result2}")
        
        # Efekt 1'i tetikle
        result3 = client.trigger_effect(device_id, 1)
        print(f"Effect 1 triggered: {result3}")
        
        # Harmony parametrelerini ayarla
        harmony_params = {
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
        result4 = client.set_harmony_parameters(device_id, harmony_params)
        print(f"Harmony parameters set: {result4}")
        
        # Tüm cihazlara reverb efekti uygula
        result5 = client.broadcast_command("reverb", {
            "type": 0,
            "param1": 100,
            "param2": 150,
            "param3": 200,
            "param4": 180
        })
        print(f"Broadcast command sent: {result5}")
        
    except requests.exceptions.RequestException as e:
        print(f"Error: {e}")
```