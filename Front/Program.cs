using Front;
using Front.Servicios;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuracion HttpClient 
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7181/")
});

// ✅ DESCOMENTAR Y REGISTRAR SERVICIOS
builder.Services.AddScoped<IArchivoService, ArchivoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

await builder.Build().RunAsync();