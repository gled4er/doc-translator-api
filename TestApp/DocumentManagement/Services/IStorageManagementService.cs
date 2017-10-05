using System.Threading.Tasks;

namespace TestApp.DocumentManagement.Services
{
    /// <summary>
    /// Storage Management Service Interface
    /// </summary>
    public interface IStorageManagementService
    {
        /// <summary>
        /// Download blob file to local file system
        /// </summary>
        /// <param name="storageContainerName">Name of storage container</param>
        /// <param name="storageFileName">Name of blob file</param>
        /// <returns></returns>
        Task<string> DownloadBlob(string storageContainerName, string storageFileName);
    }
}
