using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;

namespace PhotoGallery.Azure
{
    public class AzureLocalProxy
    {
        private static readonly CloudStorageAccount storageAccount;

        static AzureLocalProxy()
        {
            //obviously
            storageAccount = CloudStorageAccount.Parse(ResourceLoader.GetForViewIndependentUse().GetString("connectionstring"));
            //UploadImages(new List<StorageFile>());
        }

        public static async Task<bool> UploadImages(IList<StorageFile> files)
        {
            
            var tasks = new List<Task>();
            if (storageAccount != null)
            {
                var cloudBlobClient = storageAccount.CreateCloudBlobClient();
                var blobContainer = cloudBlobClient.GetContainerReference("testimages");
                //try
                //{
                await blobContainer.CreateIfNotExistsAsync();
                Debug.WriteLine("Container Created!");
                var permissions = new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob };
                await blobContainer.SetPermissionsAsync(permissions);
                Debug.WriteLine("Permissions set!");

                for (int i = 0; i < files.Count; i++)
                {
                    var blockBlob = blobContainer.GetBlockBlobReference($"ImageBlob{i}");
                    blockBlob.Properties.ContentType = files[i].ContentType;
                    tasks.Add(blockBlob.UploadFromFileAsync(files[i]));
                    Debug.WriteLine($"Went through for loop {i + 1} times out of {files.Count}!");
                }
                //}
                //catch (Exception e)
                //{
                //    var hello = e.Message;
                //}

            }
            Debug.WriteLine("Got to end of if statement!");
            if (tasks.Count > 0)
            {
                Debug.WriteLine("tasks.Count > 0!");
               var result = Task.WhenAll(tasks);
                await result;
                Debug.WriteLine("Finished Running through all tasks!");
                return !result.IsCanceled && !result.IsFaulted && result.IsCompleted;

            }
            return false;
        }
    }
}