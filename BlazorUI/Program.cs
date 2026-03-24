using BlazorUI;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
//var url = "https://localhost:7013";
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(url) }); 
builder.Services.AddHttpClient("WebApi", client =>
    { 
        client.BaseAddress= new Uri("http://api:8080"); // The service name defined in the Docker Compose file
    });


await builder.Build().RunAsync();
