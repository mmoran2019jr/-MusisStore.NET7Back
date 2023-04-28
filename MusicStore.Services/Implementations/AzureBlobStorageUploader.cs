using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicStore.Entities;
using MusicStore.Services.Interfaces;

namespace MusicStore.Services.Implementations;

public class AzureBlobStorageUploader : IFileUploader
{
    private readonly ILogger<AzureBlobStorageUploader> _logger;
    private readonly AppSettings _options;

    public AzureBlobStorageUploader(IOptions<AppSettings> options, ILogger<AzureBlobStorageUploader> logger)
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
            var client = new BlobServiceClient(_options.StorageConfiguration.Path);

            var container = client.GetBlobContainerClient("musicproject");

            var blob = container.GetBlobClient(fileName);

            await using var stream = new MemoryStream(Convert.FromBase64String(base64String));
            await blob.UploadAsync(stream, true);

            return $"{_options.StorageConfiguration.PublicUrl}{fileName}";

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al subir el archivo {filename} {message}", fileName, ex.Message);

            return string.Empty;
        }
    }
}