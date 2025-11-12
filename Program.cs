using System.Net.Http.Headers;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Data Protection (default key-ring is fine here; API key itself is encrypted and stored in SQL)
builder.Services.AddDataProtection().SetApplicationName("MvcWeatherMap");

// Dapper + services
builder.Services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<ISecretProtector, SecretProtector>();
builder.Services.AddScoped<ISecretRepository, SecretRepository>();
builder.Services.AddScoped<WeatherService>();

// NWS HttpClient
builder.Services.AddHttpClient("nws", c =>
{
    c.BaseAddress = new Uri("https://api.weather.gov/");
    c.DefaultRequestHeaders.UserAgent.ParseAdd(
        builder.Configuration["Nws:UserAgent"] ?? "MvcWeatherMap/1.0 (+contact@example.com)");
    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/geo+json"));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
