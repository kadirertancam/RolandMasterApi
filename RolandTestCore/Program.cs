using Microsoft.OpenApi.Models;
using RolandTestCore.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Roland VT-4 MIDI API", Version = "v1", Description = "Roland VT-4 efektlerini MIDI aracılığıyla kontrol etmek için API" });
});

// Register MIDI service as a singleton
builder.Services.AddSingleton<IMidiService, MidiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
