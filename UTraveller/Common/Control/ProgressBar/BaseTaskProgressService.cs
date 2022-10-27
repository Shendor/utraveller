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
    public abstract class BaseTaskProgressService : BaseViewModel, ITaskProgressService
    {
        private int maxValue;
        private int value;
        private bool isIndeterminate;
        private string text;
        private bool isCancelEnabled;

        protected ITaskExecutionManager taskExecutionManager;

        public BaseTaskProgressService(ITaskExecutionManager taskExecutionManager)
        {
            this.taskExecutionManager = taskExecutionManager;
        }


        public int MaxValue
        {
            get { return maxValue; }
            set
            {
                maxValue = value;
                RaisePropertyChanged("MaxValue");
            }
        }


        public int Value
        {
            get { return value; }
            set
            {
                this.value = value;
                RaisePropertyChanged("Value");
            }
        }


        public bool IsIndeterminate
        {
            get { return isIndeterminate; }
            set
            {
                isIndeterminate = value;
                RaisePropertyChanged("IsIndeterminate");
            }
        }


        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                RaisePropertyChanged("Text");
            }
        }


        public bool IsCancelEnabled
        {
            get { return isCancelEnabled; }
            set
            {
                isCancelEnabled = value;
                RaisePropertyChanged("IsCancelEnabled");
            }
        }


        public bool IsInProgress
        {
            get;
            set;
        }


        public virtual void FinishProgress()
        {
            IsInProgress = false;
            Value = 0;
            MaxValue = 100;
            Text = null;
            MessengerInstance.Send<ProgressBarChangedMessage>(new ProgressBarChangedMessage(false, Value, MaxValue), GetProgressBarTypeToken());
        }


        public void RunIndeterminateProgress(string text = null, bool isCancelEnabled = false)
        {
            OpenProgress();

            IsIndeterminate = true;
            IsInProgress = true;
            IsCancelEnabled = isCancelEnabled;
            Text = text == null ? "Waiting..." : text; //TODO: I18n
        }


        public virtual void UpdateProgress(int value, int maxValue, string text = null, bool isCancelEnabled = true)
        {
            IsCancelEnabled = isCancelEnabled;
            IsIndeterminate = false;
            IsInProgress = true;
            MaxValue = maxValue;
            Value = value;
            Text = text;
            MessengerInstance.Send<ProgressBarChangedMessage>(new ProgressBarChangedMessage(true, value, maxValue), 
                GetProgressBarTypeToken());
        }


        private void OpenProgress()
        {
            if (!IsInProgress)
            {
                MessengerInstance.Send<ProgressBarChangedMessage>(new ProgressBarChangedMessage(true, 0, 10), GetProgressBarTypeToken());
            }
        }


        protected abstract object GetProgressBarTypeToken();
    }
}
