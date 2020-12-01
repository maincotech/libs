using System.Collections.Generic;
using System.IO;

namespace System.Net
{
    /// <summary>
    /// Helper methods to assist with FTP services
    /// </summary>
    /// <remarks>Pegasus Library</remarks>
    public static partial class FtpHelper
    {
        /// <summary>
        /// Creates the directory on the FTP site.
        /// </summary>
        /// <param name="remoteFolder">The remote folder.</param>
        /// <param name="credential">The credential.</param>
        public static void CreateDirectory(string remoteFolder, NetworkCredential credential)
        {
            CreateDirectory(remoteFolder, credential, 1);
        }

        /// <summary>
        /// Creates the directory on the FTP site.
        /// </summary>
        /// <param name="remoteFolder">The remote folder.</param>
        /// <param name="credential">The credential.</param>
        /// <param name="retryAmount">The amount of times to attempt the operation.</param>
        public static void CreateDirectory(string remoteFolder, NetworkCredential credential, int retryAmount)
        {
            var numTries = 0;
            while (true)
            {
                try
                {
                    // Set up the ftp request
                    var request = (FtpWebRequest)FtpWebRequest.Create(remoteFolder);
                    request.Credentials = credential;
                    request.KeepAlive = false;
                    request.UseBinary = true;
                    request.Method = WebRequestMethods.Ftp.MakeDirectory;

                    // We don't care what we got back as long as it didn't throw.
                    using (var response = (FtpWebResponse)request.GetResponse()) { }

                    return;
                }
                catch (Exception)
                {
                    numTries++;
                    if (numTries >= retryAmount)
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Get an array of all file names in a remote folder
        /// </summary>
        /// <param name="remoteFolder">The remote folder.</param>
        /// <param name="credential">The credential.</param>
        /// <returns></returns>
        public static string[] GetRemoteFilenames(string remoteFolder, NetworkCredential credential)
        {
            return GetRemoteFilenames(remoteFolder, credential, 1);
        }

        /// <summary>
        /// Get an array of all file names in a remote folder
        /// </summary>
        /// <param name="remoteFolder">The remote folder.</param>
        /// <param name="credential">The credential.</param>
        /// <param name="retryAmount">The amount of times to attempt the operation.</param>
        /// <returns></returns>
        public static string[] GetRemoteFilenames(string remoteFolder, NetworkCredential credential, int retryAmount)
        {
            var numTries = 0;
            while (true)
            {
                try
                {
                    // Set up the ftp request
                    var request = (FtpWebRequest)FtpWebRequest.Create(remoteFolder);
                    request.Credentials = credential;
                    request.KeepAlive = false;
                    request.UseBinary = true;
                    request.Method = WebRequestMethods.Ftp.ListDirectory;

                    // Get the list of files to download
                    var filesToDownload = new List<string>();

                    using (var response = (FtpWebResponse)request.GetResponse())
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            var file = reader.ReadLine();
                            while (!string.IsNullOrEmpty(file))
                            {
                                filesToDownload.Add(file);
                                file = reader.ReadLine();
                            }
                        }
                    }

                    return filesToDownload.ToArray();
                }
                catch (Exception)
                {
                    numTries++;
                    if (numTries >= retryAmount)
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Download a single remote file into the local folder keeping the same file name
        /// </summary>
        /// <param name="remoteFile">The remote file.</param>
        /// <param name="localFolder">The local folder.</param>
        /// <param name="credential">The credential.</param>
        /// <param name="deleteFile">if set to <c>true</c> [delete file].</param>
        /// <returns>The path of the downloaded local file</returns>
        public static string DownloadFile(string remoteFile, string localFolder, NetworkCredential credential, bool deleteFile)
        {
            return DownloadFile(remoteFile, localFolder, credential, deleteFile, 1);
        }

        /// <summary>
        /// Download a single remote file into the local folder keeping the same file name
        /// </summary>
        /// <param name="remoteFile">The remote file.</param>
        /// <param name="localFolder">The local folder.</param>
        /// <param name="credential">The credential.</param>
        /// <param name="deleteFile">if set to <c>true</c> [delete file].</param>
        /// <param name="retryAmount">The amount of times to attempt the operation.</param>
        /// <returns>The path of the downloaded local file</returns>
        public static string DownloadFile(string remoteFile, string localFolder, NetworkCredential credential, bool deleteFile, int retryAmount)
        {
            var localFile = Path.Combine(localFolder, Path.GetFileName(remoteFile));
            DownloadAndRenameFile(remoteFile, localFile, credential, deleteFile, retryAmount);

            return localFile;
        }

        /// <summary>
        /// Download a single remote file to any given local name
        /// </summary>
        /// <param name="remoteFile">The remote file.</param>
        /// <param name="localFile">The local file.</param>
        /// <param name="credential">The credential.</param>
        /// <param name="deleteFile">if set to <c>true</c> [delete file].</param>
        public static void DownloadAndRenameFile(string remoteFile, string localFile, NetworkCredential credential, bool deleteFile)
        {
            DownloadAndRenameFile(remoteFile, localFile, credential, deleteFile, 1);
        }

        /// <summary>
        /// Download a single remote file to any given local name
        /// </summary>
        /// <param name="remoteFile">The remote file.</param>
        /// <param name="localFile">The local file.</param>
        /// <param name="credential">The credential.</param>
        /// <param name="deleteFile">if set to <c>true</c> [delete file].</param>
        /// <param name="retryAmount">The amount of times to attempt the operation.</param>
        public static void DownloadAndRenameFile(string remoteFile, string localFile, NetworkCredential credential, bool deleteFile, int retryAmount)
        {
            var numTries = 0;
            FileStream fs = null;
            while (true)
            {
                try
                {
                    var webClient = new WebClient();
                    webClient.UseDefaultCredentials = false;
                    webClient.Credentials = credential;
                    var data = webClient.DownloadData(remoteFile);
                    fs = File.Create(localFile, data.Length);
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                    //webClient.DownloadFile( remoteFile, localFile );
                    break;
                }
                catch (Exception)
                {
                    numTries++;
                    if (numTries >= retryAmount)
                    {
                        throw;
                    }
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }
            }

            if (deleteFile)
            {
                DeleteRemoteFile(remoteFile, credential, retryAmount);
            }
        }

        /// <summary>
        /// Download all files in an Ftp directory to a local folder
        /// </summary>
        /// <param name="remoteFolder">The remote folder.</param>
        /// <param name="localFolder">The local folder.</param>
        /// <param name="credential">The credential.</param>
        /// <param name="deleteRemoteFiles">if set to <c>true</c> [delete remote files].</param>
        /// <returns>The downloaded files</returns>
        public static string[] DownloadAllFilesInFolder(string remoteFolder, string localFolder, NetworkCredential credential, bool deleteRemoteFiles)
        {
            return DownloadAllFilesInFolder(remoteFolder, localFolder, credential, deleteRemoteFiles, 1);
        }

        /// <summary>
        /// Download all files in an Ftp directory to a local folder
        /// </summary>
        /// <param name="remoteFolder">The remote folder.</param>
        /// <param name="localFolder">The local folder.</param>
        /// <param name="credential">The credential.</param>
        /// <param name="deleteRemoteFiles">if set to <c>true</c> [delete remote files].</param>
        /// <param name="retryAmount">The amount of times to attempt the operation.</param>
        /// <returns>The downloaded files</returns>
        public static string[] DownloadAllFilesInFolder(string remoteFolder, string localFolder, NetworkCredential credential, bool deleteRemoteFiles, int retryAmount)
        {
            // Get the list of files to download
            var filesToDownload = GetRemoteFilenames(remoteFolder, credential, retryAmount);

            // Download the files
            foreach (var file in filesToDownload)
            {
                DownloadFile(Path.Combine(remoteFolder, file), localFolder, credential, deleteRemoteFiles, retryAmount);
            }

            return filesToDownload;
        }

        /// <summary>
        /// Upload a single file keeping the same name as the local file
        /// </summary>
        /// <param name="remoteFolder">The remote folder.</param>
        /// <param name="localFile">The local file.</param>
        /// <param name="credential">The credential.</param>
        public static void UploadFile(string remoteFolder, string localFile, NetworkCredential credential)
        {
            UploadFile(remoteFolder, localFile, credential, 1);
        }

        /// <summary>
        /// Upload a single file keeping the same name as the local file
        /// </summary>
        /// <param name="remoteFolder">The remote folder.</param>
        /// <param name="localFile">The local file.</param>
        /// <param name="credential">The credential.</param>
        /// <param name="retryAmount">The amount of times to attempt the operation.</param>
        public static void UploadFile(string remoteFolder, string localFile, NetworkCredential credential, int retryAmount)
        {
            var remoteFile = Path.Combine(remoteFolder, Path.GetFileName(localFile));
            UploadAndRenameFile(remoteFile, localFile, credential, retryAmount);
        }

        /// <summary>
        /// Upload a single file to a remote filename
        /// </summary>
        /// <param name="remoteFile">The remote file.</param>
        /// <param name="localFile">The local file.</param>
        /// <param name="credential">The credential.</param>
        public static void UploadAndRenameFile(string remoteFile, string localFile, NetworkCredential credential)
        {
            UploadAndRenameFile(remoteFile, localFile, credential, 1);
        }

        /// <summary>
        /// Upload a single file to a remote filename
        /// </summary>
        /// <param name="remoteFile">The remote file.</param>
        /// <param name="localFile">The local file.</param>
        /// <param name="credential">The credential.</param>
        /// <param name="retryAmount">The amount of times to attempt the operation.</param>
        public static void UploadAndRenameFile(string remoteFile, string localFile, NetworkCredential credential, int retryAmount)
        {
            var numTries = 0;
            while (true)
            {
                try
                {
                    var webClient = new WebClient();
                    webClient.UseDefaultCredentials = false;
                    webClient.Credentials = credential;
                    webClient.UploadFile(remoteFile, localFile);
                    break;
                }
                catch (Exception)
                {
                    numTries++;
                    if (numTries >= retryAmount)
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Upload all files matching the given pattern in a local folder to a remote folder
        /// </summary>
        /// <param name="remoteFolder">The remote folder.</param>
        /// <param name="localFolder">The local folder.</param>
        /// <param name="credential">The credential.</param>
        /// <param name="searchPattern">The search pattern.</param>
        public static void UploadAllFilesInFolder(string remoteFolder, string localFolder, NetworkCredential credential, string searchPattern)
        {
            UploadAllFilesInFolder(remoteFolder, localFolder, credential, searchPattern, 1);
        }

        /// <summary>
        /// Upload all files matching the given pattern in a local folder to a remote folder
        /// </summary>
        /// <param name="remoteFolder">The remote folder.</param>
        /// <param name="localFolder">The local folder.</param>
        /// <param name="credential">The credential.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="retryAmount">The amount of times to attempt the operation.</param>
        public static void UploadAllFilesInFolder(string remoteFolder, string localFolder, NetworkCredential credential, string searchPattern, int retryAmount)
        {
            var filesToUpload = Directory.GetFiles(localFolder, searchPattern);

            foreach (var file in filesToUpload)
            {
                UploadFile(remoteFolder, file, credential, retryAmount);
            }
        }

        /// <summary>
        /// Upload all files in a local folder to a remote folder
        /// </summary>
        /// <param name="remoteFolder">The remote folder.</param>
        /// <param name="localFolder">The local folder.</param>
        /// <param name="credential">The credential.</param>
        public static void UploadAllFilesInFolder(string remoteFolder, string localFolder, NetworkCredential credential)
        {
            UploadAllFilesInFolder(remoteFolder, localFolder, credential, 1);
        }

        /// <summary>
        /// Upload all files in a local folder to a remote folder
        /// </summary>
        /// <param name="remoteFolder">The remote folder.</param>
        /// <param name="localFolder">The local folder.</param>
        /// <param name="credential">The credential.</param>
        /// <param name="retryAmount">The amount of times to attempt the operation.</param>
        public static void UploadAllFilesInFolder(string remoteFolder, string localFolder, NetworkCredential credential, int retryAmount)
        {
            UploadAllFilesInFolder(remoteFolder, localFolder, credential, "*", retryAmount);
        }

        /// <summary>
        /// Delete all files in a remote folder
        /// </summary>
        /// <param name="remoteFolder">The remote folder.</param>
        /// <param name="credential">The credential.</param>
        public static void DeleteAllFilesInRemoteFolder(string remoteFolder, NetworkCredential credential)
        {
            DeleteAllFilesInRemoteFolder(remoteFolder, credential, 1);
        }

        /// <summary>
        /// Delete all files in a remote folder
        /// </summary>
        /// <param name="remoteFolder">The remote folder.</param>
        /// <param name="credential">The credential.</param>
        /// <param name="retryAmount">The amount of times to attempt the operation.</param>
        public static void DeleteAllFilesInRemoteFolder(string remoteFolder, NetworkCredential credential, int retryAmount)
        {
            var filesToDelete = GetRemoteFilenames(remoteFolder, credential, retryAmount);
            foreach (var file in filesToDelete)
            {
                DeleteRemoteFile(Path.Combine(remoteFolder, file), credential, retryAmount);
            }
        }

        /// <summary>
        /// Delete a single remote file
        /// </summary>
        /// <param name="remoteFile">The remote file.</param>
        /// <param name="credential">The credential.</param>
        public static void DeleteRemoteFile(string remoteFile, NetworkCredential credential)
        {
            DeleteRemoteFile(remoteFile, credential, 1);
        }

        /// <summary>
        /// Delete a single remote file
        /// </summary>
        /// <param name="remoteFile">The remote file.</param>
        /// <param name="credential">The credential.</param>
        /// <param name="retryAmount">The amount of times to attempt the operation.</param>
        public static void DeleteRemoteFile(string remoteFile, NetworkCredential credential, int retryAmount)
        {
            var numTries = 0;
            while (true)
            {
                try
                {
                    var request = (FtpWebRequest)FtpWebRequest.Create(remoteFile);
                    request.Credentials = credential;
                    request.KeepAlive = false;
                    request.UseBinary = true;
                    request.Method = WebRequestMethods.Ftp.DeleteFile;
                    var response = (FtpWebResponse)request.GetResponse();
                    response.Close();
                    break;
                }
                catch (Exception)
                {
                    numTries++;
                    if (numTries >= retryAmount)
                    {
                        throw;
                    }
                }
            }
        }
    }
}