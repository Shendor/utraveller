using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Api
{
    public interface IZipExtractService
    {
        Task<Stream> Unzip(Stream zipStreamArchive);
    }
}
