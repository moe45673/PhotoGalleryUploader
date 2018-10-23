using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace PhotoGalleryUploader.Core.Helpers
{
    public static class ExtensionMethods
    {

        public async static Task<StorageItemThumbnail> ConvertToThumbnailAsync(this StorageFile file, uint requestedSize = 300, ThumbnailMode mode = ThumbnailMode.SingleItem, ThumbnailOptions thumbnailOptions = ThumbnailOptions.UseCurrentScale)
        {
            return await file.GetThumbnailAsync(mode, requestedSize, thumbnailOptions).AsTask().ConfigureAwait(false);
        }
    }

    public static class HelperMethods
    {
        public static async Task<StorageFolder> ChooseFolderAsync()
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            folderPicker.FileTypeFilter.Add("*");
            return await folderPicker.PickSingleFolderAsync();
        }

        public static async Task<IEnumerable<List<T>>> CreateBatchesAsync<T>(this List<T> locations, int nSize = 30, int startingIndex = 0)
        {            
            return await Task.Run(() =>
            {
                IEnumerable<List<T>> Batches = new List<List<T>>();
                for (int i = startingIndex; i < locations.Count; i += nSize)
                {
                    Batches.ElementAt(i).AddRange(locations.GetRange(i, Math.Min(nSize, locations.Count - i)));
                }
                return Batches;
            }).ConfigureAwait(false);                
        }

        public static IEnumerable<List<T>> CreateBatches<T>(this List<T> locations, int nSize = AppConstants.BatchSize, int startingIndex = 0)
        {
            IEnumerable<List<T>> Batches = new List<List<T>>();
            for (int i = startingIndex; i < locations.Count; i += nSize)
            {
                Batches.ElementAt(i).AddRange(locations.GetRange(i, Math.Min(nSize, locations.Count - i)));
            }
            return Batches;
        }


    }
}
