using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;

namespace PhotoGalleryUploader
{
    class CloudManager
    {
        private static readonly string connectionString;
        private static readonly string uploadTask;
        private static readonly string downloadTask;
        

        static CloudManager()
        {
            var connMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
            var resContext = ResourceContext.GetForViewIndependentUse();
            connectionString = connMap.GetValue("AzureStorageConnection", resContext).ValueAsString;

            uploadTask = "Upload";
            downloadTask = "Download";
        }

        public async static Task UploadFilesToCloud(IEnumerable<StorageFile> files)
        {
            //var taskRegistered = false;

            //foreach(var task in BackgroundTaskRegistration.AllTasks)
            //{
            //    if(task.Value.Name == uploadTask)
            //    {
            //        taskRegistered = true;
            //        break;
            //    }
            //}

            //var builder = new BackgroundTaskBuilder();

            //builder.Name = uploadTask;
            //builder.TaskEntryPoint = "AzureConnectionTasks.Upload";
            //builder.SetTrigger(new SystemTrigger(SystemTriggerType.InternetAvailable, false));
            //var task = builder.Register();


        }
       
    }
}
