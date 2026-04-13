using BlazorUI;
using BlazorUI.Interfaces;
using BlazorUI.Models;
using BlazorUI.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using static BlazorUI.Services.Typed_clients;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

using var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
// Load the appsettings.json configuration file from the server always
//builder.Configuration.AddJsonStream(await http.GetStreamAsync("appsettings.json"));
////And then load the appsettings.{}.json configurate file based on the environment
try
{
    builder.Configuration.AddJsonStream(await http.GetStreamAsync($"appsettings.{builder.HostEnvironment.Environment}.json"));
} catch (HttpRequestException ex) when(ex.StatusCode == System.Net.HttpStatusCode.NotFound)
{
}

//builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//                     .AddJsonFile($"appsettings.{builder.HostEnvironment.Environment}.json", optional: true);

var apiBaseUrl = builder.Configuration["ApiSettings:ApiBaseUrl"];
if (string.IsNullOrEmpty(apiBaseUrl))
{
    throw new InvalidOperationException("ApiBaseUrl no est� configurado en el frontend");
}


builder.Services.AddLocalStorageServices();
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddValidatorsFromAssemblyContaining<CreateAppointment>(); // FluentValidation validators
builder.Services.AddHttpClient<PublicApiClient>(client => 
{
    client.BaseAddress = new Uri(apiBaseUrl);
});// Public client

builder.Services.AddTransient<AuthMessageHandler>();
builder.Services.AddHttpClient<ProtectedApiClient>(client => 
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<AuthMessageHandler>();// Authenticated client


builder.Services.AddAuthorizationCore();


await builder.Build().RunAsync();
