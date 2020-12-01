using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Maincotech.Storage
{
    public class OSFileStorage : IBlobStorage
    {
        private readonly string _root;

        public OSFileStorage(string folder)
        {
            _root = folder;
        }

        public async Task<BlobContainer> CreateContainer(BlobContainer container)
        {
            //The Identifier is full path of the directory.
            var fullPath = Path.Combine(_root, container.Identifier);
            var dirInfo = new DirectoryInfo(fullPath);
            if (dirInfo.Exists == false)
            {
                dirInfo.Create();
            }
            if (Directory.Exists(container.Identifier))
            {
                Directory.CreateDirectory(container.Identifier);
            }
            return ConstructContainer(dirInfo);
        }

        public async Task DeleteBlob(string id)
        {
            var fullPath = Path.Combine(_root, id);
            File.Delete(fullPath);
        }

        public async Task DeleteContainer(string id)
        {
            var fullPath = Path.Combine(_root, id);
            var dirInfo = new DirectoryInfo(fullPath);
            if (dirInfo.Exists)
            {
                dirInfo.Delete(true);
            }
        }

        public async Task<bool> Exists(Blob blob)
        {
            var fullPath = Path.Combine(_root, blob.Identifier);
            var fileInfo = new FileInfo(fullPath);
            return fileInfo.Exists;
        }

        public async Task<Blob> GetBlob(string id)
        {
            var fullPath = Path.Combine(_root, id);
            var fileInfo = new FileInfo(fullPath);
            return ConstructBlob(fileInfo);
        }

        public async Task<BlobContainer> GetBlobContainerById(string id)
        {
            var blob = await GetBlob(id);
            return blob.Parent;
        }

        public async Task<BlobContainer> GetBlobContainerByPath(string path)
        {
            var fullPath = Path.Combine(_root, path);
            DirectoryInfo dirInfo = new DirectoryInfo(fullPath);
            return ConstructContainer(dirInfo);
        }

        public async Task<IList<Blob>> GetBlobs(BlobContainer container)
        {
            var fullPath = Path.Combine(_root, container.Identifier);
            var dirInfo = new DirectoryInfo(fullPath);
            return dirInfo.GetFiles().Select(fi => ConstructBlob(fi)).ToList();
        }

        public async Task<IList<BlobContainer>> GetContainers(BlobContainer container)
        {
            var fullPath = Path.Combine(_root, container.Identifier);
            var dirInfo = new DirectoryInfo(fullPath);
            return dirInfo.GetDirectories().Select(x => ConstructContainer(x)).ToList();
        }

        public async Task<string> GetOwner()
        {
            //var dirInfo = new DirectoryInfo(_root);
            return "File System";
        }

        public async Task<Quota> GetQuota()
        {
            var dirInfo = new DirectoryInfo(_root);
            DriveInfo driveInfo = dirInfo.GetDriveInfo();
            return new Quota
            {
                Total = driveInfo.TotalSize,
                Remaining = driveInfo.AvailableFreeSpace,
                Used = driveInfo.TotalSize - driveInfo.TotalFreeSpace,
            };
        }

        public async Task<Stream> OpenBlob(Blob blob)
        {
            var fullPath = Path.Combine(_root, blob.Identifier);
            var fileInfo = new FileInfo(fullPath);
            return fileInfo.OpenRead();
        }

        public async Task RenameBlob(string id, string newName, string displayName)
        {
            var fullPath = Path.Combine(_root, id);
            var fileInfo = new FileInfo(fullPath);
            var targetFileInfo = new FileInfo(Path.Combine(fileInfo.DirectoryName, newName));
            File.Move(fileInfo.FullName, targetFileInfo.FullName);
        }

        public async Task RenameContainer(string id, string newName, string displayName)
        {
            var fullPath = Path.Combine(_root, id);
            var dirInfo = new DirectoryInfo(fullPath);
            dirInfo.MoveTo(Path.Combine(dirInfo.Parent.FullName, newName));
        }

        public async Task<Blob> SaveBlob(Blob blob, Stream content)
        {
            var fullPath = Path.Combine(_root, blob.Identifier);
            var fileInfo = new FileInfo(fullPath);
            if(fileInfo.Directory.Exists == false)
            {
                fileInfo.Directory.Create();
            }
            using (var targetStream = fileInfo.OpenWrite())
            {
               await content.CopyToAsync(targetStream);
            }
            return ConstructBlob(fileInfo);
        }

        private Blob ConstructBlob(FileInfo item)
        {
            var relativePath = item.FullName.Replace(_root, string.Empty).Replace("\\", "/"); //Get the relative url from the path

            var result = new Blob
            {
                BlobStorage = this,
                Name = item.Name,
                Identifier = relativePath,
                Uri = relativePath,
                Size = item.Length,
                Parent = ConstructContainer(item.Directory),
                Metadata = new PropertyBag
                {
                    { "CreationTimeUtc", item.CreationTimeUtc },
                    { "LastWriteTimeUtc", item.LastWriteTimeUtc },
                    { "LastAccessTimeUtc", item.LastAccessTimeUtc }
                }
            };

            return result;
        }

        private BlobContainer ConstructContainer(DirectoryInfo item)
        {
            var relativePath = item.FullName.Replace(_root, string.Empty).Replace("\\", "/"); //Get the relative url from the path
            var result = new BlobContainer
            {
                BlobStorage = this,
                DisplayName = item.Name,
                Name = item.Name,
                Identifier = relativePath,
                Uri = relativePath,
                Metadata = new PropertyBag
                {
                    { "CreationTimeUtc", item.CreationTimeUtc },
                    { "LastWriteTimeUtc", item.LastWriteTimeUtc },
                    { "LastAccessTimeUtc", item.LastAccessTimeUtc }
                }
            };

            return result;
        }
    }
}