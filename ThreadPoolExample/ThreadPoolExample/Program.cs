using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ThreadPoolExample
{
    class Program
    {
        private static readonly string ThreadPoolSizeArg = "/ThreadPoolSize";
        
        static void Main(string[] args)
        {
            int threadPoolSize = GetThreadPoolSize(args);
            ThreadPoolManager threadPool = new ThreadPoolManager(threadPoolSize);

            for (int i = 0; i < threadPoolSize; i++)
            {
                threadPool.ExecuteContext(new MyThreadContext());
            }

            while (threadPool.RunningThreadCount > 0)
                Thread.Sleep(1000);

            threadPool.Shutdown();
        }

        static int GetThreadPoolSize(string[] args)
        {
            int threadPoolSize = ThreadPoolManager.DefaultThreadPoolSize;
            Func<string, string> getThreadPoolSize = (a) =>
            {
                var arg = a.Split(':');
                if (arg.Length == 2 && arg[0] == ThreadPoolSizeArg)
                    return arg[1];
                else
                    return string.Empty;
            };
            var argFilter = args.Where(a => getThreadPoolSize(a) != string.Empty).Select(a => getThreadPoolSize(a));
            if (argFilter.Any())
            {
                var arg = argFilter.First();
                Int32.TryParse(arg, out threadPoolSize);
            }

            return threadPoolSize;
        }
    }
}
