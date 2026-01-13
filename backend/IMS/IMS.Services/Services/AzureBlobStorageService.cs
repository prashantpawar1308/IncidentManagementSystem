using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace IMS.Services
{
    public class AzureBlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public AzureBlobStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string> UploadFileAsync(Stream stream, string containerName, string fileName, string contentType)
        {
            var container = _blobServiceClient.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.None);

            var blobClient = container.GetBlobClient(fileName);

            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            };

            await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });
            return blobClient.Uri.ToString();
        }

        public Task<string> CreateServiceSASBlobUrl(string container, string blob, string storedPolicyName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(container);

            // Check if BlobContainerClient object has been authorized with Shared Key
            if (!containerClient.CanGenerateSasUri)
            {
                // Client object is not authorized via Shared Key
                return Task.FromResult<string>(null);
            }

            // If a specific blob was provided, generate a blob-level SAS
            if (!string.IsNullOrEmpty(blob))
            {
                var blobClient = containerClient.GetBlobClient(blob);

                var sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = containerClient.Name,
                    BlobName = blob,
                    Resource = "b"
                };

                if (string.IsNullOrWhiteSpace(storedPolicyName))
                {
                    sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddDays(1);
                    sasBuilder.SetPermissions(BlobSasPermissions.Read);
                }
                else
                {
                    sasBuilder.Identifier = storedPolicyName;
                }

                Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
                return Task.FromResult(sasUri.AbsoluteUri);
            }

            // Otherwise generate a container-level SAS (do not set BlobName)
            var containerSasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = containerClient.Name,
                Resource = "c"
            };

            if (string.IsNullOrWhiteSpace(storedPolicyName))
            {
                containerSasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddDays(2);
                containerSasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
            }
            else
            {
                containerSasBuilder.Identifier = storedPolicyName;
            }

            Uri containerSasUri = containerClient.GenerateSasUri(containerSasBuilder);
            return Task.FromResult(containerSasUri.AbsoluteUri);
        }
    }
}