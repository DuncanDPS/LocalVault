using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using NubeCasera.Datos;
using NubeCasera.Servicios;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();

// inyeccion de dependencias de EFCORE
builder.Services.AddDbContext<AppDBContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefConnection")));

// inyeccion de dependencias de los servicios
builder.Services.AddScoped<IArchivoReferenciaServicio, ArchivoReferenciaServicio>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

builder.Services.AddScoped<IThumbnailServicio, ThumbnailServicio>();

// configuracion de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorWasm",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

// Configuración del pipeline de la aplicación
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    dbContext.Database.Migrate();
}

app.UseCors("AllowBlazorWasm");
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var thumbnailsPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
    "MisProyectos",
    "ArchivosReference",
    "thumbnails");

// Crear la carpeta si no existe
if (!Directory.Exists(thumbnailsPath))
{
    Directory.CreateDirectory(thumbnailsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(thumbnailsPath),
    RequestPath = "/thumbnails"
});

app.Run();
