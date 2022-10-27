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

namespace UTraveller.Walkthrough.ViewModel
{
    public class WalkthroughViewModel : BaseViewModel
    {
        private static readonly string MAIN_PAGE = "/MainPage.xaml";

        private INavigationService navigationService;
        private IUserService userService;

        public WalkthroughViewModel(INavigationService navigationService, IUserService userService)
        {
            this.navigationService = navigationService;
            this.userService = userService;

            EnterAppCommand = new ActionCommand(EnterApp);
        }


        public ICommand EnterAppCommand
        {
            get;
            private set;
        }


        private void EnterApp()
        {
            var user = new User();
            userService.RegisterUser(user, string.Empty);
            userService.AuthenticateUser(user.Email, string.Empty);
            navigationService.Navigate(MAIN_PAGE);
        }
    }
}
