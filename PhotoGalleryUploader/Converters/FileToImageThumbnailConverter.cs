﻿using PhotoGalleryUploader.Helpers;
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
            var file = value as StorageFile;
            //convert file to thumbnail
            var thumbnail = Task.Run(async () => 
                await file.ConvertToThumbnailAsync(
                        mode: ThumbnailMode.PicturesView
                    )
                    .ConfigureAwait(false)
            ).Result;
            //convert to Bitmap
            var bmp = new BitmapImage();
            bmp.SetSource(thumbnail);
            return bmp;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        

        

    }
}
