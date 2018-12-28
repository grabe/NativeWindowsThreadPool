using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ThreadPoolExample
{
    public class ThreadPoolManager
    {
        public const int DefaultThreadPoolSize = 512;
        private List<ThreadContainer> ThreadPool { get; set; }

        public int RunningThreadCount { get { return GetRunningThreadCount(); } }
        public ThreadPoolManager(int threadPoolSize = DefaultThreadPoolSize)
        {
            ThreadPool = Enumerable.Range(0, threadPoolSize).Select(n => new ThreadPoolExample.ThreadContainer()).ToList();
        }

        public void Shutdown()
        {
            foreach (ThreadContainer thread in ThreadPool)
            {
                thread.Shutdown();
            }
        }

        public void ExecuteContext(IThreadContext context)
        {
            bool successfulInvoke = false;
            while (successfulInvoke == false)
            {
                foreach (ThreadContainer thread in ThreadPool)
                {
                    successfulInvoke = thread.ExecuteContext(context);
                    if (successfulInvoke)
                        break;
                }

                if (!successfulInvoke)
                    Thread.Sleep(250);
            }
        }

        private int GetRunningThreadCount()
        {
            return ThreadPool.Where(t => t.State == ThreadState.Running).Count();
        }
    }
}
