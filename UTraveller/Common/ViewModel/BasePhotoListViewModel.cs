using GalaSoft.MvvmLight;
using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.ViewModel
{
    public abstract class BasePhotoListViewModel<T> : BaseViewModel where T : ITimeLineItem<IDateItem>
    {
        protected ICommand addPhotoCommand;
        protected INavigationService navigationService;
        protected IPhotoService photoService;
        protected IParameterContainer<string> parameterContainer;

        public BasePhotoListViewModel(INavigationService navigationService,
            IParameterContainer<string> parameterContainer, IPhotoService photoService)
        {
            this.navigationService = navigationService;
            this.parameterContainer = parameterContainer;
            this.photoService = photoService;
            addPhotoCommand = new ActionCommand(AddImages);
        }

        public ICommand AddPhotoCommand
        {
            get { return addPhotoCommand; }
        }

        public ICollection<T> TimeLineItems
        {
            get;
            set;
        }

        private void AddImages()
        {
            NavigateImageChooser();
        }

        protected abstract void NavigateImageChooser();

    }
}
