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

        //private bool isFetching { get; set; }

        public GalleryView()
        {
            this.InitializeComponent();

            DataContextChanged += (sender, events) =>
            {
                ViewModel = DataContext as GalleryViewModel;
                //ViewModel.PropertyChanged -= OnPropertyChanged;
                //ViewModel.PropertyChanged += OnPropertyChanged;

            };

        }

        //private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    switch (e.PropertyName)
        //    {
        //        case nameof(ViewModel.FolderPath):
        //            futureAccessList.AddOrReplace("PickedImageFolder", selectedFolder, ViewModel.FolderPath);
        //            break;
        //    }
        //}

        private async void OpenFolderButtonClicked(object sender, RoutedEventArgs e)
        {
            
            var selectedFolder = await HelperMethods.ChooseFolderAsync();

            if (selectedFolder != null)
            {
                ViewModel.SelectFolderCommand.Execute(selectedFolder);
                
                
            }
            

        }
    }
}


