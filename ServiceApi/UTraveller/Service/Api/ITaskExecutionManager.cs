using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Api
{
    public interface ITaskExecutionManager
    {
        Task RunAsync(Action action);

        void InterruptIfCancelRequested();

        bool CancelCurrentTask();

        bool IsTaskRunned();
    }
}
