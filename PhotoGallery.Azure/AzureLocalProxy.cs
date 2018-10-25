using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
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

        public static async Task UploadImages(IList<StorageFile> files)
        {
            if (storageAccount != null)
            {
                var cloudBlobClient = storageAccount.CreateCloudBlobClient();
                var blobContainer = cloudBlobClient.GetContainerReference("testimages");
                try
                { 
                if(await blobContainer.CreateIfNotExistsAsync())
                    {
                        var permissions = new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob };
                        await blobContainer.SetPermissionsAsync(permissions);


                        for (int i = 0; i < files.Count; i++)
                        {
                            var blockBlob = blobContainer.GetBlockBlobReference($"ImageBlob{i}");
                            blockBlob.Properties.ContentType = files[i].ContentType;
                            await blockBlob.UploadFromFileAsync(files[i]);
                        }
                    }
                }catch(Exception e)
                {
                    ;
                }

               
            }
        }
    }
}