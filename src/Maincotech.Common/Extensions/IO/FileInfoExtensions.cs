namespace System.IO
{
    public static class FileInfoExtensions
    {
        public static DriveInfo GetDriveInfo(this FileInfo file)
        {
            return new DriveInfo(file.Directory.Root.FullName);
        }
    }
}