using Front;
using Front.Servicios;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuracion HttpClient segun el host actual del navegador.
var frontUri = new Uri(builder.HostEnvironment.BaseAddress);
var apiIsHttps = string.Equals(frontUri.Scheme, "https", StringComparison.OrdinalIgnoreCase);
var apiScheme = apiIsHttps ? "https" : "http";
var apiPort = apiIsHttps ? 7181 : 5158;
var apiBaseAddress = $"{apiScheme}://{frontUri.Host}:{apiPort}/";

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseAddress)
});

// ✅ DESCOMENTAR Y REGISTRAR SERVICIOS
builder.Services.AddScoped<IArchivoService, ArchivoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

await builder.Build().RunAsync();