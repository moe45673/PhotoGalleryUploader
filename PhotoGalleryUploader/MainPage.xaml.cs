using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PhotoGalleryUploader
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private StorageFolder _imgFolder;
        public StorageFolder ImgFolder
        {
            get { return _imgFolder; }
            set { _imgFolder = value; }
        }

        public IReadOnlyList<StorageFile> Images { get => _images; set => _images = value; }

        private IReadOnlyList<StorageFile> _images;
        

        private async void OpenFolderButtonClicked(object sender, RoutedEventArgs e)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            folderPicker.FileTypeFilter.Add("*");

            ImgFolder = await folderPicker.PickSingleFolderAsync();
            if (ImgFolder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("PickedFolderToken", ImgFolder);

                QueryOptions options;


                options = new QueryOptions(CommonFileQuery.DefaultQuery, new[] { ".png", ".jpg", ".bmp", ".tiff", ".jpeg", ".gif" });
                options.FolderDepth = FolderDepth.Deep;
                StorageFileQueryResult k = ImgFolder.CreateFileQueryWithOptions(options);
                var tempImages = await k.GetFilesAsync();
                foreach(var img in tempImages)
                {
                    var tempImg = await img.GetScaledImageAsThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView);
                    
                }

                //await 

                //IRandomAccessStreamWithContentType stream = await this.ImageEx.OpenReadAsync();



                this.textBlock.Text = "Picked folder: " + ImgFolder.Name;
            }
            else
            {
                this.textBlock.Text = "Operation cancelled.";
            }
        }
    }
}
