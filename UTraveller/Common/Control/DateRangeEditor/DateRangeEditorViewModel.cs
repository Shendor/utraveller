using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Common.ViewModel;

namespace UTraveller.Common.Control.DateRangeEditor
{
    public class DateRangeEditorViewModel : BaseViewModel
    {
        public DateRangeEditorViewModel()
        {
            ApplyDateRangeCommand = new ActionCommand(ApplyDateRange);

            MessengerInstance.Register<DateRangeToEditChangedMessage>(this, OnDateRangeToEditChanged);
        }

        public ICommand ApplyDateRangeCommand
        {
            get;
            private set;
        }

        public DateTime StartDate
        {
            get;
            set;
        }


        public DateTime? EndDate
        {
            get;
            set;
        }

        public object Token
        {
            get;
            set;
        }

        public bool IsEndDateIncluded
        {
            get;
            set;
        }


        private void OnDateRangeToEditChanged(DateRangeToEditChangedMessage message)
        {
            Token = message.Token;
            StartDate = message.StartDate;
            EndDate = message.EndDate;
            IsEndDateIncluded = EndDate != null;
            if (EndDate == null)
            {
                EndDate = StartDate;
            }

            RaisePropertyChanged("StartDate");
            RaisePropertyChanged("EndDate");
            RaisePropertyChanged("IsEndDateIncluded");
        }


        private void ApplyDateRange()
        {
            if (!IsEndDateIncluded)
            {
                EndDate = null;
            }

            MessengerInstance.Send<DateRangeChosenMessage>(new DateRangeChosenMessage(StartDate, EndDate), Token);
        }
    }
}
