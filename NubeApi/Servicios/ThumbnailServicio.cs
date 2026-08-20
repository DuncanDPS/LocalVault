using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace NubeCasera.Servicios
{
    public interface IThumbnailServicio
    {
        Task<(bool exito, string rutaThumbnail)> GenerarThumbnailAsync(
            Stream archivoStream,
            string extension,
            string hashArchivo);

        Task EliminarThumbnailAsync(string rutaThumbnail);
    }

    public class ThumbnailServicio : IThumbnailServicio
    {
        private readonly string _carpetaThumbnails;
        private const int ANCHO_THUMBNAIL = 150;
        private const int ALTO_THUMBNAIL = 150;

        public ThumbnailServicio(IWebHostEnvironment env)
        {
            var carpetaBase = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "LocalVaultFiles",
                "ArchivosReference");

            _carpetaThumbnails = Path.Combine(carpetaBase, "thumbnails");

            if (!Directory.Exists(_carpetaThumbnails))
            {
                Directory.CreateDirectory(_carpetaThumbnails);
            }
        }

        public async Task<(bool exito, string rutaThumbnail)> GenerarThumbnailAsync(
            Stream archivoStream,
            string extension,
            string hashArchivo)
        {
            try
            {
                // Verificar que es una imagen
                var extensionesValidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
                if (!extensionesValidas.Contains(extension.ToLower()))
                {
                    return (false, string.Empty);
                }

                // Generar nombre único para el thumbnail
                string nombreThumbnail = $"{hashArchivo}_thumb.webp";
                string rutaThumbnail = Path.Combine(_carpetaThumbnails, nombreThumbnail);

                // Cargar imagen y generar thumbnail
                using (var image = await Image.LoadAsync(archivoStream))
                {
                    image.Mutate(x => x
                        .Resize(new ResizeOptions
                        {
                            Size = new Size(ANCHO_THUMBNAIL, ALTO_THUMBNAIL),
                            Mode = ResizeMode.Max
                        }));

                    // Guardar como WebP (más comprimido)
                    await image.SaveAsWebpAsync(rutaThumbnail);
                }

                return (true, $"/thumbnails/{nombreThumbnail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generando thumbnail: {ex.Message}");
                return (false, string.Empty);
            }
        }

        public async Task EliminarThumbnailAsync(string rutaThumbnail)
        {
            try
            {
                if (string.IsNullOrEmpty(rutaThumbnail))
                    return;

                string nombreArchivo = Path.GetFileName(rutaThumbnail);
                string rutaCompleta = Path.Combine(_carpetaThumbnails, nombreArchivo);

                if (File.Exists(rutaCompleta))
                {
                    File.Delete(rutaCompleta);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error eliminando thumbnail: {ex.Message}");
            }
        }
    }
}