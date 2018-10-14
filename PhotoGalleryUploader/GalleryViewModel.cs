using Microsoft.WindowsAzure.Storage;
using PhotoGalleryUploader.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace PhotoGalleryUploader
{
    public class GalleryViewModel : BindableBase
    {

        private bool _localIsBusy;

        public bool LocalIsBusy
        {
            get { return this._localIsBusy; }
            set { SetProperty(ref _localIsBusy, value); }
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
            get { return this._folderPath; }
            set { SetProperty(ref _folderPath, value); }
        }

        public GalleryViewModel()
        {
            SelectedFiles = new ObservableCollection<StorageFile>();
            UploadCommand = new DelegateCommand(upload, canUpload);
            PropertyChanged -= OnPropertyChanged;
            PropertyChanged += OnPropertyChanged;

        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            UploadCommand.RaiseCanExecuteChanged();
        }

        

        public DelegateCommand UploadCommand
        {
            get;
                
            
        }

        private bool canUpload()
        {
            return SelectedFiles.Any();
        }

        private void upload()
        {
            CloudStorageAccount account;
            var connMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
            var resContext = ResourceContext.GetForViewIndependentUse();
            var conn = connMap.GetValue("AzureStorageConnection", resContext).ValueAsString;
            if (CloudStorageAccount.TryParse(conn, out account))
            {
                ;
            }
            else
            {
                // Otherwise, let the user know that they need to define the environment variable.
                Console.WriteLine(
                    "A connection string has not been defined in the system environment variables. " +
                    "Add a environment variable named 'storageconnectionstring' with your storage " +
                    "connection string as a value.");

            }
        }
    }
}
