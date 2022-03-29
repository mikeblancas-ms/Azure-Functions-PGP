using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using ICSharpCode.SharpZipLib.Zip;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using PgpCore;
using FuncPgp.Model;

namespace FuncPgp.Helper
{
    public static class SecurityHelper
    {
        public static string GenerateChecksumFromString(string fileContent)
        {
            var checksum = "";
            using (MemoryStream stream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(fileContent);
                    writer.Flush();
                    stream.Position = 0;
                    using (var md5 = MD5.Create())
                    {
                        byte[] hash = md5.ComputeHash(stream);
                        checksum = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }
            }

            return checksum;
        }

        public static async Task<string> ReadBlob(string filePath, string connString, bool ReturnAsChecksum, bool IsEncrypted = false)
        {
            var fileContent = "";
            CloudBlockBlob blockBlob2 = SetBlockBlob(filePath, connString);

            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    await blockBlob2.DownloadToStreamAsync(memoryStream);
                    memoryStream.Position = 0;

                    if(IsEncrypted)
                    {
                        using (var md5 = MD5.Create())
                        {
                            byte[] hash = md5.ComputeHash(memoryStream);
                            fileContent = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                        }
                    }
                    else
                    {
                        fileContent = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
                        if (ReturnAsChecksum)
                            fileContent = GenerateChecksumFromString(fileContent);
                    }
                }
                catch (Exception ex)
                {
                    fileContent = "";
                }
            }

            return Regex.Replace(fileContent.Trim(), @"\t|\n|\r", ""); 
        }

        public static async Task<Stream> ReadBlobAsStream(string filePath, string connString)
        {
            var outputStream = new MemoryStream();
            CloudBlockBlob blockBlob2 = SetBlockBlob(filePath, connString);

            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    await blockBlob2.DownloadToStreamAsync(memoryStream);
                    memoryStream.Position = 0;
                    memoryStream.CopyTo(outputStream);
                    outputStream.Position = 0;
                }
                catch (Exception ex)
                {
                    outputStream = new MemoryStream();
                }
            }

            return outputStream;
        }

        public static async Task<bool> DecryptAsync(string outputPath, string filePath, string connString, string pass = null)
        {
            var isSuccess = false;
            CloudBlockBlob blockBlob2 = SetBlockBlob(outputPath, connString);

            using (PGP pgp = new PGP())
            {
                Stream outputStream = new MemoryStream();
                var inputStream = await ReadBlobAsStream(filePath, connString);
                var privateKeyStream = await ReadBlobAsStream(Environment.GetEnvironmentVariable("PGP_PrivateKey"), connString);

                await pgp.DecryptStreamAsync(inputStream, outputStream, privateKeyStream, pass);
                outputStream.Position = 0;
                await blockBlob2.UploadFromStreamAsync(outputStream);
                isSuccess = true;
            }

            return isSuccess;
        }

        public static async Task<bool> EncryptAsync(string outputPath, string filePath, string connString, string pass = null)
        {
            var isSuccess = false;
            CloudBlockBlob blockBlob2 = SetBlockBlob(outputPath, connString);

            using (PGP pgp = new PGP())
            {
                Stream outputStream = new MemoryStream();
                var inputStream = await ReadBlobAsStream(filePath, connString);
                var publicKeyStream = await ReadBlobAsStream(Environment.GetEnvironmentVariable("PGP_PublicKey"), connString);

                if (!string.IsNullOrEmpty(pass))
                {
                    var privateKeyStream = await ReadBlobAsStream(Environment.GetEnvironmentVariable("PGP_PrivateKey"), connString);
                    await pgp.EncryptStreamAndSignAsync(inputStream, outputStream, publicKeyStream, privateKeyStream, pass);
                } else
                {
                    await pgp.EncryptStreamAsync(inputStream, outputStream, publicKeyStream);
                }
                outputStream.Position = 0;
                await blockBlob2.UploadFromStreamAsync(outputStream);
                isSuccess = true;
            }

            return isSuccess;
        }
        public static CloudBlockBlob SetBlockBlob(string path, string connString)
        {
            var fileInfo = GetFileInfo(path);

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connString);

            CloudBlobClient client = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference(fileInfo.Container);

            var fileNameWithFolder =
                        fileInfo.DirectoryName == ""
                            ? fileInfo.FileName
                            : $"{fileInfo.DirectoryName}/{fileInfo.FileName}";

            return container.GetBlockBlobReference(fileNameWithFolder);
        }
        private static FuncPgp.Model.FileInfo GetFileInfo(string fileURI)
        {
            var fx = fileURI.Split('/');
            var container = fx.Take(fx.Count() - 2).ToArray();
            var path = fx.Skip(Math.Max(0, fx.Count() - 2)).ToArray();

            string[] filePathSplit = new string[3];
            filePathSplit[0] = string.Join('/', container);
            filePathSplit[1] = path[0];
            filePathSplit[2] = path[1];

            var fileInfo = new FuncPgp.Model.FileInfo();
            fileInfo.Container = filePathSplit[0];

            if ((filePathSplit.Length - 2) > 0)
            {
                var folderName = "";

                for (var i = 1; i < filePathSplit.Length - 1; i++)
                {
                    if (folderName.Trim().Length > 0)
                    {
                        folderName += "/";
                    }

                    folderName += filePathSplit[i];
                }

                fileInfo.DirectoryName = folderName;
            }

            fileInfo.FileName = filePathSplit[filePathSplit.Length - 1];

            return fileInfo;
        }
    }
}
