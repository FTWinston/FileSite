using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FileSite.Services
{
    public class FileService
    {
        public FileService()
        {
            if (Container == null)
                Container = GetFileContainer();
        }

        public static CloudBlobContainer Container { get; private set; }

        private static CloudBlobContainer GetFileContainer()
        {
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            var containerName = CloudConfigurationManager.GetSetting("ContainerName");
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            if (container.CreateIfNotExists())
            {
                var permissions = new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob };
                container.SetPermissions(permissions);
            }

            return container;
        }

        public Uri Upload(string name, Stream data)
        {
            CloudBlockBlob blob = Container.GetBlockBlobReference(name);
            blob.UploadFromStream(data);
            return ConvertUri(blob.Uri);
        }

        public void Delete(string name)
        {
            CloudBlockBlob blob = Container.GetBlockBlobReference(name);
            blob.Delete();
        }

        public IEnumerable<Uri> ListFiles()
        {
            // see "List the Blobs in a Container" here https://azure.microsoft.com/en-gb/documentation/articles/storage-dotnet-how-to-use-blobs/
            foreach (IListBlobItem blob in Container.ListBlobs(null, true))
            {
                yield return ConvertUri(blob.Uri);
            }
        }

        private static Uri CustomUri { get; set; }

        private static Uri ConvertUri(Uri uri)
        {
            if (CustomUri == null)
            {
                var customUrl = CloudConfigurationManager.GetSetting("CustomUrl");
                if (string.IsNullOrEmpty(customUrl))
                    return uri;
                CustomUri = new Uri(customUrl);
            }
            
            return new Uri(string.Format("{0}://{1}{2}", CustomUri.Scheme, CustomUri.Host, uri.PathAndQuery), UriKind.Absolute);
        }
    }
}