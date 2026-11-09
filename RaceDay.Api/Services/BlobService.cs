using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RaceDay.Api.Services;

public class BlobService
{
    private readonly BlobContainerClient _containerClient;

    public BlobService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureBlobStorage");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "The Azure Blob Storage connection string is not configured.");
        }

        var containerName = configuration["AzureBlobStorage:ContainerName"] ?? "uploads";
        _containerClient = new BlobContainerClient(connectionString, containerName);
    }

    public async Task<string> UploadFileAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        await _containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var extension = Path.GetExtension(file.FileName);
        var blobName = $"{Guid.NewGuid():N}{extension}";
        var blobClient = _containerClient.GetBlobClient(blobName);

        await using var stream = file.OpenReadStream();
        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = string.IsNullOrWhiteSpace(file.ContentType)
                    ? "application/octet-stream"
                    : file.ContentType
            }
        };

        await blobClient.UploadAsync(stream, options, cancellationToken);
        return blobClient.Uri.ToString();
    }
}
