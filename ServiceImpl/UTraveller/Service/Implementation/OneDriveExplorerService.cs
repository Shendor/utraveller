using Microsoft.Live;
using UTraveller.Service.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;
using ServiceImpl.UTraveller.Service.Implementation;
using UTraveller.Service.Model;

namespace UTraveller.Service.Implementation
{
    public class OneDriveExplorerService : IFileExplorerService<OneDriveFileRequest>
    {

        public async Task<ICollection<File>> GetFiles(OneDriveFileRequest request)
        {
            var files = new List<File>();
            LiveConnectClient liveClient = new LiveConnectClient(request.Session);
            LiveOperationResult operationResult = await liveClient.GetAsync(request.FileName + "/files");
            
            if (operationResult != null)
            {
                List<object> data = (List<object>)operationResult.Result["data"];
                if (data != null)
                {
                    foreach (IDictionary<string, object> content in data)
                    {
                        var isDirectory = content["type"].Equals("folder") || content["type"].Equals("album") ? true : false;
                        var fileName = (string)content["name"];
                        var file = new File((string)content["id"], (string)content["name"], isDirectory);
                        if (isDirectory || (!isDirectory && request.FileFilter(fileName)))
                        {
                            files.Add(file);
                        }
                        file.ParentName = (string)content["parent_id"];
                    }
                }
            }
            return files;
        }

        public async Task<System.IO.Stream> ReadFile(OneDriveFileRequest request)
        {
            LiveConnectClient liveClient = new LiveConnectClient(request.Session);

            try
            {
                var downloadedFile = await liveClient.DownloadAsync(request.FileName + "/content");
                return downloadedFile != null ? downloadedFile.Stream : null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error reading route file: " + ex.Message);
                return null;
            }
        }

    }
}
