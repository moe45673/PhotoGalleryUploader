using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using PhotoGalleryUploader.Core.Helpers;

using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Toolkit.Collections;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PhotoGalleryUploader
{


    public sealed partial class GalleryView : Page
    {
        public GalleryViewModel ViewModel { get; private set; }

        public readonly StorageItemAccessList futureAccessList = StorageApplicationPermissions.FutureAccessList;

        private bool isFetching { get; set; }

        public GalleryView()
        {
            this.InitializeComponent();

            DataContextChanged += (sender, events) =>
            {
                ViewModel = DataContext as GalleryViewModel;
                ViewModel.PropertyChanged -= OnPropertyChanged;
                ViewModel.PropertyChanged += OnPropertyChanged;
                
            };

        }



        //could be broken down better
        private async Task LoadThumbnailsFromFiles(int pageCount = 20)
        {

            ViewModel.LocalIsBusy = true;
            List<BitmapImage> images = new List<BitmapImage>();
            foreach (var file in ViewModel.SelectedFiles)
            {
                var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.PicturesView);
                var image = new BitmapImage();
                image.SetSource(thumbnail);
                images.Add(image);

            }
            //var items = new Items(images);
            ImageCollection = new ObservableCollection<BitmapImage>(images);
            ViewModel.LocalIsBusy = false;
        }

        private ObservableCollection<BitmapImage> ImageCollection;

        private async void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.SelectedFiles):

                    await LoadThumbnailsFromFiles(20);

                    break;
            }
        }

        private async void OpenFolderButtonClicked(object sender, RoutedEventArgs e)
        {
            ViewModel.LocalIsBusy = true;
            var selectedFolder = await HelperMethods.ChooseFolderAsync();

            if (selectedFolder != null)
            {



                //    // Application now has read/write access to all contents in the picked folder
                //    // (including other sub-folder contents)
                ViewModel.FolderPath = selectedFolder.Path;

                futureAccessList.AddOrReplace("PickedImageFolder", selectedFolder, ViewModel.FolderPath);

                var options = new QueryOptions(CommonFileQuery.DefaultQuery, FileExtensions.Image);
                options.FolderDepth = FolderDepth.Deep;
                


                if (selectedFolder.AreQueryOptionsSupported(options))
                {
                    uint index = 0;
                    const uint stepSize = 100;

                    var k = selectedFolder.CreateFileQueryWithOptions(options);
                    var result = await k.GetFilesAsync(index, stepSize);

                    ViewModel.SelectedFiles = new ObservableCollection<StorageFile>(result);                 

                }
            }
            ViewModel.LocalIsBusy = false;

        }
    }

    #region MaybeCode
    public class Items : IIncrementalSource<StorageItemThumbnail>
    {

        public List<StorageItemThumbnail> images { get; set; }

        public Items() : this(new List<StorageItemThumbnail>())
        {

        }

        public Items(List<StorageItemThumbnail> list)
        {
            images = list;
        }

        public async Task<IEnumerable<StorageItemThumbnail>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            return images.GetRange(pageIndex, pageSize);
            
        }
    }
    #endregion
}
