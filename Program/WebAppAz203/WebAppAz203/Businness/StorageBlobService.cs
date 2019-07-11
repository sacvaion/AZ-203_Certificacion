using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using Microsoft.ApplicationInsights;
namespace WebAppAz203.Businness
{
    public class StorageBlobService
    {
        private static TelemetryClient telemetry = new TelemetryClient();


        public static string SaveBlob(byte[] contentBlob, string blobName, string containerName, string contentType)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageAccountConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer containerRef = blobClient.GetContainerReference(containerName.ToLower());
            containerRef.CreateIfNotExists();
            containerRef.SetPermissions(new BlobContainerPermissions()
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });
            CloudBlockBlob blob = containerRef.GetBlockBlobReference(blobName);
            blob.DeleteIfExists();
            blob.Properties.ContentType = contentType;
            using (MemoryStream stream = new MemoryStream(contentBlob))
            {
                blob.UploadFromStream(stream);
                return blob.Uri.ToString();
            }
        }

        public static byte[] GetBlob(string blobName, string containerName)
        {
            telemetry.InstrumentationKey = ConfigurationManager.AppSettings["instrumentationKey"];

            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageAccountConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer containerRef = blobClient.GetContainerReference(containerName.ToLower());
            CloudBlockBlob blob = containerRef.GetBlockBlobReference(blobName);
            using (MemoryStream stream = new MemoryStream())
            {
                blob.DownloadToStream(stream);
                telemetry.TrackTrace("Get Image: " + blobName);
                return stream.ToArray();

            }
        }

        public static void DeleteBlob(string blobName, string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageAccountConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer containerRef = blobClient.GetContainerReference(containerName.ToLower());
            CloudBlockBlob blob = containerRef.GetBlockBlobReference(blobName);
            blob.DeleteIfExists();
        }

        public static void CloneBlob(string sourceBlobName, string targetBlobName, string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageAccountConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer containerRef = blobClient.GetContainerReference(containerName.ToLower());
            CloudBlockBlob sourceBlob = containerRef.GetBlockBlobReference(sourceBlobName);
            CloudBlockBlob targetBlob = containerRef.GetBlockBlobReference(targetBlobName);
            targetBlob.DeleteIfExists();
            targetBlob.StartCopy(sourceBlob);
        }

        public static IEnumerable<IListBlobItem> GetBlobList(string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageAccountConnectionString"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer containerRef = blobClient.GetContainerReference(containerName.ToLower());
            return containerRef.ListBlobs();
        }

    }
}