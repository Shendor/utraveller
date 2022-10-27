using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;

namespace UTraveller.Service.Implementation
{
    public class ZipExtractService : IZipExtractService
    {

        public async Task<Stream> Unzip(Stream zipStreamArchive)
        {
            return await Task.Run<Stream>(() =>
            {
                try
                {
                    var result = new MemoryStream();

                    ZipInputStream zipInputStream = new ZipInputStream(zipStreamArchive);
                    ZipEntry zipEntry = zipInputStream.GetNextEntry();
                    if (zipEntry != null)
                    {
                        String entryFileName = zipEntry.Name;
                        byte[] buffer = new byte[4096];

                        StreamUtils.Copy(zipInputStream, result, buffer);
                    }
                    return result;

                }
                catch (Exception)
                {
                    return null;
                }
            });
        }
    }
}
