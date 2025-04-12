using Microsoft.OpenApi.Models;
using RolandMasterApi.Configuration;
using RolandMasterApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Swagger dokümantasyonunu ekle
builder.Services.AddSwaggerDocumentation();

// Redis servisi ekleniyor
builder.Services.AddSingleton<RedisService>();

// CORS politikası ekleniyor
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Docker içinde çalışırken tüm arayüzlere bağlan
app.Urls.Add("http://*:80");

// Configure the HTTP request pipeline.
// Swagger dokümantasyonunu etkinleştir
app.UseSwaggerDocumentation();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Redis servisini başlat
var redisService = app.Services.GetRequiredService<RedisService>();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Eski komutları temizle
logger.LogInformation("API başlatılırken eski komutlar temizleniyor...");
await redisService.CleanupOldCommandsAsync();

// Redis yanıt dinleyicisini başlat
redisService.SubscribeToResponses(response =>
{
    logger.LogInformation("Komut yanıtı alındı: {CommandId}, Cihaz: {DeviceId}, Durum: {Status}", 
        response.CommandId, response.DeviceId, response.Status);
    
    // Yanıt alınan komutları Redis'ten sil
    if (response.Status == "completed" || response.Status == "failed")
    {
        _ = redisService.DeleteRedisKeyAsync($"{RedisConfiguration.CommandKeyPrefix}{response.CommandId}");
        logger.LogInformation("Tamamlanan komut Redis'ten silindi: {CommandId}", response.CommandId);
    }
});

app.Run();
