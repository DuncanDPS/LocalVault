using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using NubeCasera.Datos;
using NubeCasera.Servicios;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

// Limite de subida establecido en 5gb
const long maxUploadSize = 5L * 1024L * 1024L * 1024L;

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = maxUploadSize;
});

builder.WebHost.ConfigureKestrel(o =>
{
    o.Limits.MaxRequestBodySize = maxUploadSize;
});

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

// configuracion de autenticacion JWT

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
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
    "LocalVaultFiles",
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
