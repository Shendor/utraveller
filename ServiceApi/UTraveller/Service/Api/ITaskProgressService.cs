using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Api
{
    public interface ITaskProgressService
    {
        void RunIndeterminateProgress(string text = null, bool isCancelEnabled = true);

        void FinishProgress();

        void UpdateProgress(int value, int maxValue, string text = null, bool isCancelEnabled = true);

        bool IsInProgress
        {
            get;
            set;
        }
    }

    public interface ICancelableTaskProgressService : ITaskProgressService
    {
        void CancelTask();

        bool IsCanceled
        {
            get;
            set;
        }
    }
}
