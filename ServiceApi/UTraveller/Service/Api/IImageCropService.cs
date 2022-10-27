using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UTraveller.Service.Api
{
    public interface IImageCropService
    {

        Stream CropImage(Stream origin, Point topLeft, Point bottomRight);

        void ChangeResolution(Stream origin, Stream result, int desiredWidth, int desiredHeight, int quality = 100);

        Task<Stream> ChangeResolution(Stream origin, int originWidth, int originHeight, int desiredWidth = 0, int desiredHeight = 0);

        byte[] ChangeResolutionAndGetBytes(Stream origin, int desiredWidth, int desiredHeight, int quality = 100);
    }
}
