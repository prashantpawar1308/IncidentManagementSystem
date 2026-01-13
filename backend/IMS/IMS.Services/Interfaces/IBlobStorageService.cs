namespace IMS.Services
{
    public interface IBlobStorageService
    {
        /// <summary>
        /// Uploads a stream to the specified container with the provided fileName and returns the public URL.
        /// </summary>
        Task<string> UploadFileAsync(Stream stream, string containerName, string fileName, string contentType);

        /// <summary>
        /// Generates a service Shared Access Signature (SAS) token for the specified container, optionally using a
        /// stored access policy.
        /// </summary>
        /// <remarks>Use the returned SAS token to grant limited access to the container's resources
        /// without exposing account keys. The permissions and expiration of the SAS token depend on the stored access
        /// policy if provided, or on default settings if not.</remarks>
        /// <param name="container">The name of the container for which to create the SAS token. Cannot be null or empty.</param>
        /// <param name="storedPolicyName">The name of the stored access policy to associate with the SAS token. If null, the SAS token is created with
        /// ad-hoc permissions.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the SAS token string for the
        /// specified container.</returns>
        Task<string> CreateServiceSASBlobUrl(string container, string blob, string storedPolicyName);
    }
}