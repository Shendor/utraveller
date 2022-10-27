using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace UTravellerModel.UTraveller.Model
{
    public class File
    {
        public File() { }

        public File(string id, string name, bool isDirectory)
            : this(name, isDirectory)
        {
            Id = id;
        }

        public File(string name, bool isDirectory)
        {
            Name = name;
            IsDirectory = isDirectory;
        }

        public string Id
        {
            get;
            set;
        }

        public bool IsDirectory
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Extension
        {
            get
            {
                if (!IsDirectory && Name != null && Name.Length > 4)
                {
                    var pointIndex = Name.IndexOf(".");
                    return Name.Substring(pointIndex + 1, Name.Length - pointIndex - 1);
                }
                return string.Empty;
            }
        }

        public string ParentName
        {
            get;
            set;
        }

        public BitmapImage Icon
        {
            get;
            set;
        }
    }
}
