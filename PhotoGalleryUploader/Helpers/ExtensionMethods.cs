using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace PhotoGalleryUploader.Helpers
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

        public static IEnumerable<List<T>> CreateBatches<T>(List<T> locations, int nSize = 30)
    {
        for (int i = 0; i < locations.Count; i += nSize)
        {
            yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
        }
    }
    }
}
