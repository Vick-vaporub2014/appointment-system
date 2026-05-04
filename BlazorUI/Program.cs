using BlazorUI;
using BlazorUI.Interfaces;
using BlazorUI.Models;
using BlazorUI.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using static BlazorUI.Models.Auth;
using static BlazorUI.Services.Typed_clients;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//using var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };

//try
//{
//    builder.Configuration.AddJsonStream(await http.GetStreamAsync($"appsettings.{builder.HostEnvironment.Environment}.json"));
//} catch (HttpRequestException ex) when(ex.StatusCode == System.Net.HttpStatusCode.NotFound)
//{
//}
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{builder.HostEnvironment.Environment}.json", optional: true, reloadOnChange: true);

//builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//                     .AddJsonFile($"appsettings.{builder.HostEnvironment.Environment}.json", optional: true);

var apiBaseUrl = builder.Configuration["ApiSettings:ApiBaseUrl"]
    ?? Environment.GetEnvironmentVariable("API_BASE_URL");
if (string.IsNullOrEmpty(apiBaseUrl))
{
    throw new InvalidOperationException("ApiBaseUrl no esta configurado en el frontend");
}


builder.Services.AddLocalStorageServices();
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthStateProvider>());
// FluentValidation validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateAppointment>(); 
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDTO>();

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
