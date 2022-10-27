using GalaSoft.MvvmLight;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using UTraveller.Common.ViewModel;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.ImageChooser.Model
{
    public class CheckedImageModel : BaseViewModel
    {
        private bool isChecked;
        private Photo photo;

        public CheckedImageModel(Picture picture)
        {
            photo = new Photo(picture);
        }

        public CheckedImageModel(Photo photo)
        {
            this.photo = photo;
        }

        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (this.isChecked != value)
                {
                    this.isChecked = value;
                    RaisePropertyChanged("IsChecked");
                }
            }
        }

        public BitmapImage Thumbnail
        {
            get { return Photo.Thumbnail; }
        }

        public BitmapImage Image
        {
            get { return Photo.Image; }
        }

        public Photo Photo
        {
            get { return photo; }
        }


        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            if (obj == null || !obj.GetType().Equals(this.GetType()))
            {
                return false;
            }
            var viewModel = (CheckedImageModel)obj;
            return viewModel.Photo.Equals(Photo);
        }


        public override int GetHashCode()
        {
            return Photo.GetHashCode();
        }
    }
}
