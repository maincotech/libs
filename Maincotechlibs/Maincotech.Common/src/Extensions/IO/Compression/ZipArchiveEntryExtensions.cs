namespace System.IO.Compression
{
    public static class ZipArchiveEntryExtensions
    {
        /// <summary>
        /// Extract the ZipArchiveEntry to targart folder
        /// </summary>
        /// <param name="this">ZipArchiveEntry's instance</param>
        /// <param name="targetPath">The extract to path</param>
        public static void ExtractTo(this ZipArchiveEntry @this, string targetPath)
        {
            var fullPath = Path.Combine(targetPath, @this.FullName);
            if (string.IsNullOrEmpty(@this.Name))
            {
                var di = new DirectoryInfo(fullPath);
                if (di.Exists == false)
                {
                    di.Create();
                }
            }
            else
            {
                var fileInfo = new FileInfo(fullPath);
                if (fileInfo.Directory.Exists == false)
                {
                    fileInfo.Directory.Create();
                }

                if (fileInfo.Exists)
                {
                    if (fileInfo.LastWriteTime != @this.LastWriteTime)
                    {
                        fileInfo.Delete();
                        @this.ExtractToFile(fullPath);
                    }
                }
                else
                {
                    @this.ExtractToFile(fullPath);
                }
            }
        }
    }
}