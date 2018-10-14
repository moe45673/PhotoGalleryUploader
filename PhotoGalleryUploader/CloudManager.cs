using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;

namespace PhotoGalleryUploader
{
    class CloudManager
    {
        private static readonly string connectionString;

        static CloudManager()
        {
            var connMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
            var resContext = ResourceContext.GetForViewIndependentUse();
            connectionString = connMap.GetValue("AzureStorageConnection", resContext).ValueAsString;
        }

        public async static Task UploadFilesToCloud(IEnumerable<StorageFile> files)
        {

        }
       
    }
}
