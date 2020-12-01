using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Maincotech.Storage
{
    /// <summary>
    /// Blog Storage
    /// </summary>
    public interface IBlobStorage
    {
        Task<Blob> GetBlob(string id);

        Task<Stream> OpenBlob(Blob blob);

        Task<bool> Exists(Blob blob);

        Task<Blob> SaveBlob(Blob blob, Stream content);

        Task DeleteBlob(string id);

        Task RenameBlob(string id, string newName, string displayName);

        Task<BlobContainer> CreateContainer(BlobContainer container);

        Task DeleteContainer(string id);

        Task RenameContainer(string id, string newName, string displayName);

        Task<IList<Blob>> GetBlobs(BlobContainer container);

        Task<IList<BlobContainer>> GetContainers(BlobContainer container);

        Task<BlobContainer> GetBlobContainerById(string id);

        Task<BlobContainer> GetBlobContainerByPath(string path);

        Task<Quota> GetQuota();

        Task<string> GetOwner();
    }
}