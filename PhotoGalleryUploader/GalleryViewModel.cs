using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Uwp;
using Microsoft.WindowsAzure.Storage;
using Nito.AsyncEx;
using PhotoGallery.Core.Helpers;
using PhotoGalleryUploader.BackgroundTaskHelper;
using PhotoGalleryUploader.Core.Helpers;
using PhotoGalleryUploader.Core.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.System.Threading;

namespace PhotoGalleryUploader
{

    #region Aliases
    using IncrementalLoadingStorageItemCollection =
        IncrementalLoadingCollection<ThumbnailSource, StorageItemThumbnail>;

    #endregion

    public class GalleryViewModel : BindableBase
    {

        #region Fields
        private readonly CoreDispatcher Dispatcher;
        //private readonly ThreadPoolTimer _periodicTimer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallback), TimeSpan.FromSeconds(1));
        #endregion

        #region Commands
        public DelegateCommand UploadCommand { get; set; }

        public DelegateCommand<StorageFolder> SelectFolderCommand { get; set; }

        #endregion

        #region Properties

        private bool _localIsBusy;
        public bool LocalIsBusy
        {
            get => this._localIsBusy;
            set => SetProperty(ref _localIsBusy, value);
        }

        private IncrementalLoadingStorageItemCollection _thumbnails;
        public IncrementalLoadingStorageItemCollection Thumbnails
        {
            get => _thumbnails;
            private set => SetProperty(ref _thumbnails, value);
        }

        private ObservableCollection<StorageFile> _selectedFiles;
        public ObservableCollection<StorageFile> SelectedFiles
        {
            get => _selectedFiles;
            set => SetProperty(ref _selectedFiles, value);
        }

        private string _folderPath;
        public string FolderPath
        {
            get => this._folderPath;
            set => SetProperty(ref _folderPath, value);
        }

        private BackgroundTaskFacade _uploadTask;

        public BackgroundTaskFacade UploadTask
        {
            get => _uploadTask;
            set => SetProperty(ref _uploadTask, value);
        }



        private uint _uploadProgress;

        public uint UploadProgress
        {
            get => this._uploadProgress;
            set => SetProperty(ref _uploadProgress, value);
        }


        #endregion

        #region Constructors

        public GalleryViewModel()
        {
            SelectedFiles = new ObservableCollection<StorageFile>();
            UploadCommand = new DelegateCommand(Upload, CanUpload);
            SelectFolderCommand = new DelegateCommand<StorageFolder>(SelectFolder);
            PropertyChanged -= OnPropertyChanged;
            PropertyChanged += OnPropertyChanged;
            Dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            Initialize();
        }

        private uint _progress;

        public uint Progress
        {
            get => this._progress;
            set => SetProperty(ref _progress, value);
        }

        private void Initialize()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == AppConstants.UploadTaskName)
                {
                    UploadTask = new BackgroundTaskFacade(task.Value,
                        (reg1, args1) =>
                        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                if (args1.Progress < 100)
                                {
                                    Progress += 10;
                                }

                            }

                        );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        }, (reg2, args2) =>
                        {

                        });

                    UploadTask.PropertyChanged -= OnPropertyChanged;
                    UploadTask.PropertyChanged += OnPropertyChanged;
                    BackgroundTaskHelperBase.UpdateBackgroundTaskRegistrationStatus(task.Value.Name, true);
                    break;
                }
            }



        }





        #endregion

        private void PeriodicTimerCallback(ThreadPoolTimer timer)
        {
            if (Progress < 100)
            {

                Progress += 10;

            }
            else
            {
                //_periodicTimer.Cancel();

                //var key = _taskInstance.Task.Name;

                ////
                //// Record that this background task ran.
                ////
                //String taskStatus = (_progress < 100) ? "Canceled with reason: " + _cancelReason.ToString() : "Completed";
                //var settings = ApplicationData.Current.LocalSettings;
                //settings.Values[key] = taskStatus;
                //Debug.WriteLine("Background " + _taskInstance.Task.Name + settings.Values[key]);

                ////
                //// Indicate that the background task has completed.
                ////
                //_deferral.Complete();
            }
        }

        #region DelegateImplementations

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            UploadCommand.RaiseCanExecuteChanged();
            switch (args.PropertyName)
            {
                case nameof(SelectedFiles):
                    ThumbnailSource source = new ThumbnailSource(GetThumbnailCollectionFromFiles(SelectedFiles));
                    Thumbnails = new IncrementalLoadingStorageItemCollection(source, AppConstants.BatchSize);
                    break;



            }

        }

        private async void SelectFolder(StorageFolder selectedFolder)
        {
            LocalIsBusy = true;
            FolderPath = selectedFolder.Path;

            StorageApplicationPermissions.FutureAccessList.AddOrReplace(AppConstants.UploadFolderToken, selectedFolder, FolderPath);

            var options = new QueryOptions(CommonFileQuery.DefaultQuery, FileExtensions.Image);
            options.FolderDepth = FolderDepth.Deep;

            if (selectedFolder.AreQueryOptionsSupported(options))
            {
                uint index = 0;
                uint stepSize = Convert.ToUInt32(AppConstants.BatchSize);

                var k = selectedFolder.CreateFileQueryWithOptions(options);

                var result = await k.GetFilesAsync();
                var imageResults = result.Where(file => string.Equals(
                                                                file.ContentType.Substring(0, 5),
                                                                "image"
                                                                )
                                                );
                SelectedFiles = new ObservableCollection<StorageFile>(imageResults);
            }
            LocalIsBusy = false;
        }

        private bool CanUpload()
        {
            return SelectedFiles.Any();
        }

        private async void Upload()
        {
            //            var trigger = new ApplicationTrigger();
            //            UploadTask = new BackgroundTaskFacade("Tasks.UploadFileTask", AppConstants.UploadTaskName, trigger, null, requiresBackgroundAccess: true);

            //            UploadTask.AttachProgressAndCompletedHandlers(
            //                (task, args) => //OnProgress

            //                {
            //#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            //                        Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //                        {
            //                            if (args.Progress < 100)
            //                            {

            //                                Progress = Math.Min(100, args.Progress + 10);

            //                            }
            //                            else
            //                            {

            //                            }

            //                        }

            //                    );
            //#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            //                    },
            //                (task, args) => //OnCompleted
            //                {

            //                }
            //            );

            //UploadTask.PropertyChanged -= OnPropertyChanged;
            //UploadTask.PropertyChanged += OnPropertyChanged;
            try
            {
                PhotoGallery.Azure.AzureLocalProxy.UploadImages(SelectedFiles);
            }
            catch (Exception e)
            {
                ;
            }

            //CloudStorageAccount account;
            //var connMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
            //var resContext = ResourceContext.GetForViewIndependentUse();
            //var conn = connMap.GetValue("AzureStorageConnection", resContext).ValueAsString;
            //if (CloudStorageAccount.TryParse(conn, out account))
            //{
            //    ;
            //}
            //else
            //{
            //    // Otherwise, let the user know that they need to define the environment variable.
            //    Console.WriteLine(
            //        "A connection string has not been defined in the system environment variables. " +
            //        "Add a environment variable named 'storageconnectionstring' with your storage " +
            //        "connection string as a value.");
            //}
        }
        #endregion

        #region Internal Methods
        private IEnumerable<StorageItemThumbnail> GetThumbnailCollectionFromFiles(IEnumerable<StorageFile> files)
        {
            foreach (var file in SelectedFiles)
            {
                yield return AsyncContext.Run(() =>
                   file.GetThumbnailAsync(ThumbnailMode.PicturesView).AsTask()
               );
            }
        }

        #endregion
    }



    /// <summary>
    /// Implementation of <see cref="IIncrementalSource{StorageItemThumbnail}"/> to allow for memory paging of large lists
    /// </summary>
    public class ThumbnailSource : IIncrementalSource<StorageItemThumbnail>
    {
        private readonly List<StorageItemThumbnail> _thumbnails;

        public ThumbnailSource() : this(new List<StorageItemThumbnail>())
        {
        }

        public ThumbnailSource(IEnumerable<StorageItemThumbnail> thumbnails)
        {
            _thumbnails = new List<StorageItemThumbnail>(thumbnails);
        }

        public async Task<IEnumerable<StorageItemThumbnail>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Gets items from the collection according to pageIndex and pageSize parameters.
            var result = (from t in _thumbnails
                          select t).Skip(pageIndex * pageSize).Take(pageSize);

            return result;
        }
    }
}