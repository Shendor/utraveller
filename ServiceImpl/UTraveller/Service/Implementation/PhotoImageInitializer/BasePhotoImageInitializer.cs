using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace ServiceImpl.UTraveller.Service.Implementation.PhotoImageInitializer
{
    public abstract class BasePhotoImageInitializer : IImageInitializer
    {
        public abstract Task<bool> InitializeImage(Photo photoItem);


        public void DisposeImage(Photo photoItem)
        {
            try
            {
                //photoItem.Image.ClearValue(System.Windows.Media.Imaging.BitmapImage.UriSourceProperty);
                photoItem.Dispose();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in disposing image: " + e.Message);
            } 
        }
    }
}
