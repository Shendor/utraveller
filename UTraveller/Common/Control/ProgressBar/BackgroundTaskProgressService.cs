using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;

namespace UTraveller.Common.Control.ProgressBar
{
    public class BackgroundTaskProgressService : BaseTaskProgressService
    {
        public static readonly object TOKEN = "PROGRESS_BACKGROUND_TOKEN";

        public BackgroundTaskProgressService(ITaskExecutionManager taskExecutionManager)
            : base(taskExecutionManager)
        {
        }

        protected override object GetProgressBarTypeToken()
        {
            return TOKEN;
        }
    }
}
