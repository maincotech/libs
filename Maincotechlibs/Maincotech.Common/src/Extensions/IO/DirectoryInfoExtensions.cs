namespace System.IO
{
    using System.Threading.Tasks;

    public static class DirectoryInfoExtensions
    {
        public static DriveInfo GetDriveInfo(this DirectoryInfo dir)
        {
            return new DriveInfo(dir.Root.FullName);
        }

        public static void CopyFilesRecursively(this DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            }
            foreach (FileInfo file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.FullName, file.Name));
            }
        }

        public static async Task CopyFilesRecursivelyAsync(this DirectoryInfo source, DirectoryInfo target, int maxConcurrency = 5)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                await CopyFilesRecursivelyAsync(dir, target.CreateSubdirectory(dir.Name));
            }
            await source.GetFiles().ParallelForEachAsync(async (fileInfo) =>
            {
                using var srcStream = fileInfo.OpenRead();
                using var targetStream = File.OpenWrite(Path.Combine(target.FullName, fileInfo.Name));
                await srcStream.CopyToAsync(targetStream);
            }, maxConcurrency);
            foreach (FileInfo file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.FullName, file.Name));
            }
        }
    }
}