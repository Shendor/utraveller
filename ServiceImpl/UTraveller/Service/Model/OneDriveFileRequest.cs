using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Model
{
    public class OneDriveFileRequest : BaseFileExplorerRequest
    {
        private static readonly string ROOT_DIRECTORY = "/me/skydrive";

        public OneDriveFileRequest(LiveConnectSession session)
            : this(ROOT_DIRECTORY, null, session)
        {
        }

        public OneDriveFileRequest(string fileName, LiveConnectSession session)
            : this(fileName, null, session)
        {
        }

        public OneDriveFileRequest(string fileName, FileFilter fileFilter, LiveConnectSession session)
        {
            FileName = fileName;
            Session = session;
            if (FileFilter == null && fileFilter == null)
            {
                FileFilter = (name) =>
                {
                    bool isMatchExtension = true;

                    isMatchExtension = Extensions.Any((ext) =>
                    {
                        var pointIndex = name.IndexOf(".");
                        if (pointIndex > 0)
                        {
                            var extension = (name.Length > 4) ?
                                name.Substring(pointIndex + 1, name.Length - pointIndex - 1) : null;
                            return extension != null && extension.Equals(ext.ToString().ToLower());
                        }
                        return false;
                    });

                    return isMatchExtension;
                };
            }
            else if (fileFilter != null)
            {
                FileFilter = fileFilter;
            }
        }

        public string FileName
        {
            get;
            set;
        }

        public string ParentName
        {
            get;
            set;
        }

        public string ChosenExtension
        {
            get;
            set;
        }

        public LiveConnectSession Session
        {
            get;
            set;
        }

        public FileFilter FileFilter
        {
            get;
            set;
        }
    }

    public delegate bool FileFilter(string fileName);
}
