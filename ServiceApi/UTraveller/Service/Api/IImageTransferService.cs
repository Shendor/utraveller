using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Api
{
    public interface IImageLoaderService
    {
        Task<Stream> Load(Photo photo);
    }

    public interface IImageTransferService : IImageLoaderService
    {
        void Save(Photo photo);

        void Delete(Photo photo);
    }

}
