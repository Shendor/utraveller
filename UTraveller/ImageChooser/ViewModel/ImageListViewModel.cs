using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.ViewModel;

namespace UTraveller.ImageChooser.ViewModel
{
    public class ImageListViewModel : BaseViewModel
    {
        private ICollection<GroupedImagesViewModel> images;

        public ImageListViewModel()
        {
        }


        public string FolderName
        {
            get;
            set;
        }


        public ICollection<GroupedImagesViewModel> Images
        {
            get { return images; }
            set
            {
                images = value;
                RaisePropertyChanged("Images");
            }
        }

        public override string ToString()
        {
            return FolderName;
        }
    }
}
