using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using NubeCasera.Datos;
using NubeCasera.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// inyeccion de dependencias de EFCORE
builder.Services.AddDbContext<AppDBContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefConnection")));

// inyectar el servicio IArchivoReferencia y ICategoriaService
builder.Services.AddScoped<IArchivoReferenciaServicio, ArchivoReferenciaServicio>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();


// configuracion de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorWasm",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.Run();
