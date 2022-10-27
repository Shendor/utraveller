using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.SkyDrive.ViewModel
{
    internal class SkyDriveItemFactory
    {
        public static ISkyDriveItem GetSkyDriveItem(File file)
        {
            if (file.IsDirectory)
            {
                return new SkyDriveFolderViewModel(file);
            }
            else
            {
                return new SkyDriveFileViewModel(file);
            }
        }
    }
}
