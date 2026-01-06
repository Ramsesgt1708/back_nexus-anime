using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
namespace back_bd.Services
{
    public class SaveFilesAzure : ISaveFiles
    {
        private string connectionString;
        public SaveFilesAzure(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("AzureStorageConnection");

        }

        public async Task<string> SaveFile(string container, IFormFile file)
        {
            var client = new BlobContainerClient(connectionString, container);
            await client.CreateIfNotExistsAsync();
            client.SetAccessPolicy(PublicAccessType.Blob);
            var extension = Path.GetExtension(file.FileName);
            var nameFile = $"{Guid.NewGuid()}{extension}";
            var blob = client.GetBlobClient(nameFile);
            var blobHttpHeaders = new BlobHttpHeaders();
            blobHttpHeaders.ContentType = file.ContentType;
            await blob.UploadAsync(file.OpenReadStream(), blobHttpHeaders);
            return blob.Uri.ToString();
        }
        public async Task DeleteFile(string container, string? fileRoute)
        {
            if (string.IsNullOrWhiteSpace(fileRoute)) {
                return;
            }
            var client = new BlobContainerClient(connectionString, container);
            await client.CreateIfNotExistsAsync();
            var file = Path.GetFileName(fileRoute);
            var blob = client.GetBlobClient(file);
            await blob.DeleteIfExistsAsync();
        }
    }
}
