using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using ABCRetail.Models;

namespace ABCRetail.Services
{
    public class AzureFileService
    {
        private readonly ShareClient _shareClient;

        public AzureFileService(IConfiguration configuration)
        {
            string? connectionString =
                configuration.GetConnectionString("AzureStorage");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Azure Storage connection string is not configured.");
            }

            _shareClient = new ShareClient(
                connectionString,
                "abc-retail-files");

            _shareClient.CreateIfNotExists();
        }

        // Upload a file
        public async Task<StoredFile> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException(
                    "The selected file is empty.");
            }

            ShareDirectoryClient directoryClient =
                _shareClient.GetRootDirectoryClient();

            ShareFileClient fileClient =
                directoryClient.GetFileClient(file.FileName);

            await fileClient.CreateAsync(file.Length);

            using Stream stream = file.OpenReadStream();

            await fileClient.UploadAsync(stream);

            return new StoredFile
            {
                FileName = file.FileName,
                FilePath = file.FileName,
                FileSize = file.Length,
                UploadedAt = DateTime.UtcNow
            };
        }

        // Get all files
        public async Task<List<StoredFile>> GetFilesAsync()
        {
            List<StoredFile> files = new();

            ShareDirectoryClient directoryClient =
                _shareClient.GetRootDirectoryClient();

            await foreach (ShareFileItem item in directoryClient.GetFilesAndDirectoriesAsync())
            {
                if (!item.IsDirectory)
                {
                    ShareFileClient fileClient =
                        directoryClient.GetFileClient(item.Name);

                    ShareFileProperties properties =
                        await fileClient.GetPropertiesAsync();

                    files.Add(new StoredFile
                    {
                        FileName = item.Name,
                        FilePath = item.Name,
                        FileSize = properties.ContentLength,
                        UploadedAt = properties.LastModified.UtcDateTime
                    });
                }
            }

            return files;
        }

        // Download a file
        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            ShareDirectoryClient directoryClient =
                _shareClient.GetRootDirectoryClient();

            ShareFileClient fileClient =
                directoryClient.GetFileClient(fileName);

            ShareFileDownloadInfo download =
                await fileClient.DownloadAsync();

            return download.Content;
        }
    }
}