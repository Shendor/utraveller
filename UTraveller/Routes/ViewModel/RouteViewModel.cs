using GalaSoft.MvvmLight;
using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.Resources;
using UTraveller.Routes.Messages;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Routes.ViewModel
{
    public class RouteViewModel : BaseViewModel
    {
        private ICommand deleteRouteCommand;

        public RouteViewModel(Route route)
        {
            Route = route;
            deleteRouteCommand = new ActionCommand(DeleteRoute);
            SelectRouteCommand = new ActionCommand(SelectRoute);
            ViewDescriptionCommand = new ActionCommand(ViewDescription);
            SelectCommandName = AppResources.Route_Select;
        }


        public ICommand DeleteRouteCommand
        {
            get { return deleteRouteCommand; }
        }


        public ICommand SelectRouteCommand
        {
            get;
            private set;
        }

        public ICommand ViewDescriptionCommand
        {
            get;
            private set;
        }
        
        public Route Route
        {
            get;
            private set;
        }

        public Brush Color
        {
            get { return new SolidColorBrush(Route.Color); }
        }

        public bool IsSelected
        {
            get;
            set;
        }

        public string SelectCommandName
        {
            get;
            set;
        }

        private void DeleteRoute()
        {
            MessengerInstance.Send<RouteDeletedMessage>(new RouteDeletedMessage(Route));
        }

        private void SelectRoute()
        {
            IsSelected = !IsSelected;
            SelectCommandName = IsSelected ? AppResources.Route_Unselect : AppResources.Route_Unselect;
            RaisePropertyChanged("IsSelected");
            RaisePropertyChanged("SelectCommandName");
        }

        private void ViewDescription()
        {
            MessengerInstance.Send<ViewRouteDescriptionMessage>(new ViewRouteDescriptionMessage(Route.Description));
        }

    }
}
