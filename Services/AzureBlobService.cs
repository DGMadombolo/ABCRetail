using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using ABCRetail.Models;

namespace ABCRetail.Services
{
    public class AzureBlobService
    {
        private readonly BlobContainerClient _containerClient;

        public AzureBlobService(IConfiguration configuration)
        {
            string? connectionString =
                configuration.GetConnectionString("AzureStorage");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Azure Storage connection string is not configured.");
            }

            _containerClient = new BlobContainerClient(
                connectionString,
                "product-images");

            _containerClient.CreateIfNotExists();
        }

        // Upload product image
        public async Task<ProductImage> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException(
                    "The selected file is empty.");
            }

            // Get the original file extension
            string extension =
                Path.GetExtension(file.FileName)
                    .ToLowerInvariant();

            // Generate a unique Blob name
            // This prevents files with the same name from overwriting each other.
            string blobName =
                $"{Guid.NewGuid()}{extension}";

            BlobClient blobClient =
                _containerClient.GetBlobClient(blobName);

            // Keep the correct image content type
            BlobHttpHeaders headers = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            };

            using Stream stream = file.OpenReadStream();

            // Upload image and save the original filename as metadata
            await blobClient.UploadAsync(
                stream,
                new BlobUploadOptions
                {
                    HttpHeaders = headers,

                    Metadata = new Dictionary<string, string>
                    {
                        ["OriginalFileName"] = file.FileName
                    }
                });

            return new ProductImage
            {
                // Display the original filename
                FileName = file.FileName,

                // Keep the unique Blob name internally
                BlobName = blobName,

                // Generate a temporary secure URL
                ImageUrl = GenerateSasUrl(blobClient),

                UploadedAt = DateTime.UtcNow
            };
        }

        // Get all product images
        public async Task<List<ProductImage>> GetImagesAsync(CancellationToken cancellationToken = default)
        {
            List<ProductImage> images = new();

            await foreach (var blobItem in _containerClient.GetBlobsAsync(
                traits: BlobTraits.Metadata,
                states: BlobStates.None,
                prefix: null,
                cancellationToken: cancellationToken))
            {
                BlobClient blobClient =
                    _containerClient.GetBlobClient(blobItem.Name);

                string originalFileName = blobItem.Name;

                if (blobItem.Metadata.TryGetValue(
                    "OriginalFileName",
                    out string? storedFileName))
                {
                    originalFileName = storedFileName;
                }

                images.Add(new ProductImage
                {
                    FileName = originalFileName,
                    BlobName = blobItem.Name,
                    ImageUrl = GenerateSasUrl(blobClient),
                    UploadedAt =
                        blobItem.Properties.CreatedOn?.UtcDateTime
                        ?? DateTime.UtcNow
                });
            }

            return images;
        }

        // Generate a temporary read-only SAS URL
        private string GenerateSasUrl(BlobClient blobClient)
        {
            BlobSasBuilder sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerClient.Name,

                BlobName = blobClient.Name,

                Resource = "b",

                // SAS URL expires after 1 hour
                ExpiresOn =
                    DateTimeOffset.UtcNow.AddHours(1)
            };

            // Give the URL read-only permission
            sasBuilder.SetPermissions(
                BlobSasPermissions.Read);

            return blobClient
                .GenerateSasUri(sasBuilder)
                .ToString();
        }
    }
}