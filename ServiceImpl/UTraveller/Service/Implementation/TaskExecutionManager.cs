using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UTraveller.Service.Api;

namespace UTraveller.Service.Implementation
{
    public class TaskExecutionManager : ITaskExecutionManager
    {
        private static CancellationTokenSource tokenSource;


        public async Task RunAsync(Action action)
        {
            //TODO: Not thread-safe
            if (tokenSource == null)
            {
                tokenSource = new CancellationTokenSource();
                var cancelToken = tokenSource.Token;
                
                try
                {
                    await Task.Factory.StartNew(action, cancelToken);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error executing task: " + ex.Message);
                    throw ex;
                }
                finally
                {
                    if (tokenSource != null)
                    {
                        DisposeTokenSource();
                    }
                }
            }
        }

        public void InterruptIfCancelRequested()
        {
            lock (this)
            {
                if (tokenSource != null && tokenSource.IsCancellationRequested)
                {
                    var token = tokenSource.Token;
                    DisposeTokenSource();
                    token.ThrowIfCancellationRequested();
                }
            }
        }


        public bool CancelCurrentTask()
        {
            lock (this)
            {
                if (tokenSource != null)
                {
                    tokenSource.Cancel();
                    return true;
                }
                return false;
            }
        }


        private void DisposeTokenSource()
        {
            tokenSource.Dispose();
            tokenSource = null;
        }


        public bool IsTaskRunned()
        {
            return tokenSource != null;
        }
    }
}
