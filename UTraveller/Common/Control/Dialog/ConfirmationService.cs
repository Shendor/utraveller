using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Common.ViewModel;

namespace UTraveller.Common.Control.Dialog
{
    public class ConfirmationService : BaseViewModel
    {
        private bool? isConfirmed;

        public ConfirmationService()
        {
            ConfirmCommand = new ActionCommand(Confirm);
            CancelCommand = new ActionCommand(Cancel);
        }


        public ICommand ConfirmCommand
        {
            get;
            private set;
        }


        public ICommand CancelCommand
        {
            get;
            private set;
        }

        public string Text
        {
            get;
            private set;
        }


        public async Task<bool> Show(string message)
        {
            isConfirmed = null;
            Text = message;
            MessengerInstance.Send<ShowConfirmDialogMessage>(new ShowConfirmDialogMessage(true));
            RaisePropertyChanged("Text");

            return await Task.Run<bool>(() =>
            {
                while (isConfirmed == null)
                {
                    System.Threading.Thread.Sleep(300);
                }
                return isConfirmed.Value;
            });
        }


        public async Task<bool> WaitConfirmation()
        {
            isConfirmed = null;
            return await Task.Run<bool>(() =>
            {
                while (isConfirmed == null)
                {
                    System.Threading.Thread.Sleep(300);
                }
                return isConfirmed.Value;
            });
        }


        private void Confirm()
        {
            isConfirmed = true;
        }


        private void Cancel()
        {
            isConfirmed = false;
            Text = null;
            RaisePropertyChanged("Text");
        }
    }
}
