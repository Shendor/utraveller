using GalaSoft.MvvmLight;
using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Common.ViewModel;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.ImageChooser.ViewModel
{
    public abstract class BasePhotoChooserViewModel : BaseViewModel
    {
        protected INavigationService navigationService;
        private ICommand chooseImagesCommand;

        public BasePhotoChooserViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            chooseImagesCommand = new ActionCommand(ChoosePhotos);
        }


        public ICommand ChooseImagesCommand
        {
            get { return chooseImagesCommand; }
        }


        public object Token
        {
            get;
            set;
        }


        public abstract void Initialize();


        protected abstract void ChoosePhotos();
        

        protected abstract void SendPhotoChosenMessage(ICollection<Photo> choosedPhotos, object token);
    }
}
