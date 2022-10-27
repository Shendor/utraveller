using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.SkyDrive.ViewModel
{
    internal class SkyDriveFileViewModel : BaseSkyDriveItemViewModel, ISkyDriveItem
    {
        private static readonly Uri ICON_URI = new Uri("/Assets/Icons/file.png", UriKind.Relative);

        public SkyDriveFileViewModel(File file)
            : base(file)
        {
        }

        public Uri IconUri
        {
            get { return ICON_URI; }
        }

        public void SendOpenMessage()
        {

        }

    }
}
