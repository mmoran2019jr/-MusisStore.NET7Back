using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicStore.Entities;
using MusicStore.Services.Interfaces;

namespace MusicStore.Services.Implementations;

public class FileUploader : IFileUploader
{
    private readonly ILogger<FileUploader> _logger;
    private readonly AppSettings _options;

    public FileUploader(IOptions<AppSettings> options, ILogger<FileUploader> logger)
    {
        _logger = logger;
        _options = options.Value;
    }
    public async Task<string> UploadFileAsync(string? base64String, string? fileName)
    {
        if (string.IsNullOrEmpty(base64String) || string.IsNullOrEmpty(fileName))
        {
            return string.Empty;
        }

        try
        {
            var bytes = Convert.FromBase64String(base64String);

            var path = Path.Combine(_options.StorageConfiguration.Path, fileName);

            await using var fileStream = new FileStream(path, FileMode.Create);
            await fileStream.WriteAsync(bytes, 0, bytes.Length);

            return $"{_options.StorageConfiguration.PublicUrl}{fileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al subir el archivo {filename} {message}",fileName, ex.Message);

            return string.Empty;
        }
    }
}