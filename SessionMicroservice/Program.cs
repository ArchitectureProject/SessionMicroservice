using SessionMicroservice.Helpers;
using Microsoft.EntityFrameworkCore;
using SessionMicroservice.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// cors
services.AddCors(options =>
{
    options.AddDefaultPolicy(corsPolicyBuilder =>
    {
        Console.Out.WriteLine("Adding cors policy");
        corsPolicyBuilder.WithOrigins("*")
            .AllowAnyHeader()
            .AllowAnyMethod(); 
    });
});

services.AddControllers(options =>
{
    options.Filters.Add<AppExceptionFiltersAttribute>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpClients
builder.Services.AddHttpClient("BowlingParkApi", httpClient =>
{
    httpClient.BaseAddress = new Uri(
        Environment.GetEnvironmentVariable("BOWLINGPARKAPI_URL") ??
        "http://localhost:8000/");
});
builder.Services.AddHttpClient("JwkApi", httpClient =>
{
    httpClient.BaseAddress = new Uri(
        Environment.GetEnvironmentVariable("JWKAPI_URL") ??
        "http://localhost:8080/");
});

// Services
services.AddScoped<ISessionService, SessionService>();
services.AddScoped<IBowlingParkApiService, BowlingParkApiService>();

// DbContext
services.AddDbContext<DataContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ??
                           "Host=localhost:5432;Database=session-bdd;Username=admin;Password=aupGjXqZCMh9vKkQ";
    options.UseNpgsql(connectionString);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();