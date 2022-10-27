using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.ViewModel;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.SkyDrive.ViewModel
{
    public abstract class BaseSkyDriveItemViewModel : BaseViewModel
    {
        private File file;

        public BaseSkyDriveItemViewModel(File file)
        {
            this.file = file;
        }

        public string Id
        {
            get { return file.Id; }
        }

        public bool IsDirectory
        {
            get { return file.IsDirectory; }
        }

        public string Name
        {
            get { return file.Name; }
        }

        public string Extension
        {
            get { return file.Extension; }
        }

        public string ParentName
        {
            get { return file.ParentName; }
        }
    }
}
