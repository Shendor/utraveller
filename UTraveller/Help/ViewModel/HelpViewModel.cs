using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Common.ViewModel;
using UTraveller.Service.Api;

namespace UTraveller.Help.ViewModel
{
    public class HelpViewModel : BaseViewModel
    {
        private static readonly string HELP_PAGE = "/Walkthrough/WalkthroughPage.xaml";

        private INavigationService navigationService;

        public HelpViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;

            OpenWalkthroughCommand = new ActionCommand(OpenWalkthrough);
        }


        public ICommand OpenWalkthroughCommand
        {
            get;
            set;
        }


        private void OpenWalkthrough()
        {
            navigationService.Navigate(HELP_PAGE, "false");
        }
    }
}
