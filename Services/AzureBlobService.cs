using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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

            string extension =
                Path.GetExtension(file.FileName)
                    .ToLowerInvariant();

            string blobName =
                $"{Guid.NewGuid()}{extension}";

            BlobClient blobClient =
                _containerClient.GetBlobClient(blobName);

            BlobHttpHeaders headers = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            };

            using Stream stream = file.OpenReadStream();

            await blobClient.UploadAsync(
                stream,
                new BlobUploadOptions
                {
                    HttpHeaders = headers
                });

            return new ProductImage
            {
                FileName = file.FileName,
                BlobName = blobName,
                ImageUrl = blobClient.Uri.ToString(),
                UploadedAt = DateTime.UtcNow
            };
        }

        // Get all product images
        public async Task<List<ProductImage>> GetImagesAsync()
        {
            List<ProductImage> images = new();

            await foreach (
                var blobItem in _containerClient.GetBlobsAsync())
            {
                BlobClient blobClient =
                    _containerClient.GetBlobClient(blobItem.Name);

                images.Add(new ProductImage
                {
                    FileName = blobItem.Name,
                    BlobName = blobItem.Name,
                    ImageUrl = blobClient.Uri.ToString(),
                    UploadedAt =
                        blobItem.Properties.CreatedOn?.UtcDateTime
                        ?? DateTime.UtcNow
                });
            }

            return images;
        }
    }
}