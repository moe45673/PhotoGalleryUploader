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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PhotoGalleryUploader
{


    public sealed partial class GalleryView : Page
    {
        public GalleryViewModel ViewModel { get; private set; }

        public readonly StorageItemAccessList futureAccessList = StorageApplicationPermissions.FutureAccessList;

        private bool isFetching { get; set;}

        public GalleryView()
        {
            this.InitializeComponent();

            DataContextChanged += (sender, events) =>
            {
                ViewModel = DataContext as GalleryViewModel;
            };                   

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

                var k = selectedFolder.CreateFileQueryWithOptions(options);

                ViewModel.SelectedFiles = new ObservableCollection<StorageFile>(await k.GetFilesAsync());    
                
                          
            }
            ViewModel.LocalIsBusy = false;
            
        }        
    }

    #region MaybeCode
    //public class Items : IIncrementalSource<BitmapImage>
    //{

    //    public List<BitmapImage> images { get; set; }
    //    public Items() : this(new List<BitmapImage>())
    //    {

    //    }

    //    public Items(List<BitmapImage> list)
    //    {
    //        images = list;
    //    }

    //    public async Task<IEnumerable<BitmapImage>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
    //    {
    //        var result = await Task.Run(() => (from i in images
    //                                           select i).Skip(pageIndex * pageSize).Take(pageSize));

    //        return result;
    //    }
    //}
    #endregion
}
