using UTraveller.Service.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.ViewModel;
using System.Windows.Input;
using Microsoft.Expression.Interactivity.Core;
using System.Windows;

namespace UTraveller.Common.Control.ProgressBar
{
    public class TaskProgressService : BaseTaskProgressService, ICancelableTaskProgressService
    {
        public static readonly object TOKEN = "PROGRESS_DIALOG_TOKEN";

        public TaskProgressService(ITaskExecutionManager taskExecutionManager)
            : base(taskExecutionManager)
        {

            CancelTaskCommand = new ActionCommand(CancelTask);
        }


        public ICommand CancelTaskCommand
        {
            get;
            private set;
        }


        public void CancelTask()
        {
            //taskExecutionManager.CancelCurrentTask();
            IsCanceled = true;

            Text = "Canceling current task..."; //TODO: i18n
            RaisePropertyChanged("Text");
        }


        public override void FinishProgress()
        {
            IsCanceled = false;
            base.FinishProgress();
        }


        public override void UpdateProgress(int value, int maxValue, string text = null, bool isCancelEnabled = true)
        {
            if (!IsCanceled)
            {
                base.UpdateProgress(value, maxValue, text, isCancelEnabled);
            }
        }


        public bool IsCanceled
        {
            get;
            set;
        }


        protected override object GetProgressBarTypeToken()
        {
            return TOKEN;
        }
    }
}
