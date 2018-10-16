using PhotoGalleryUploader.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace PhotoGalleryUploader.Converters
{
    public class FileToImageThumbnailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //var file = value as StorageFile;
            ////convert file to thumbnail
            //var thumbnail = file.ge
            ////var thumbnail = file.ConvertToThumbnailAsync(
            ////            mode: ThumbnailMode.PicturesView
            ////            ).Result;
                
            //var bmp = new BitmapImage();
            

            //bmp.SetSource(thumbnail);

            return new{ };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }





    }
}
