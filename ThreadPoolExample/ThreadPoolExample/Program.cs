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

    public class ThreadContainer
    {
        private object Lock { get; } = new object();
        private ThreadState _state = ThreadState.Unstarted;
        private Thread Thread { get; set; }
        private IThreadContext Context { get; set; } = null;
        private ManualResetEvent ShutdownEvent { get; } = new ManualResetEvent(false);
        private AutoResetEvent ResumeEvent { get; } = new AutoResetEvent(false);

        public ThreadState State { get { lock (Lock) { return _state; } } }
        public ManualResetEvent TerminatedEvent { get; } = new ManualResetEvent(false);

        public ThreadContainer()
        {
            Thread = new Thread(new ThreadStart(DoWork));
            Thread.Start();
        }

        public bool ExecuteContext(IThreadContext context)
        {
            bool successfulInvoke = false;

            lock(Lock)
            {
                successfulInvoke = context != null && _state == ThreadState.Suspended;
                if (successfulInvoke)
                {
                    Context = context;
                    _state = ThreadState.Running;
                    ResumeEvent.Set();
                }
            }

            return successfulInvoke;
        }

        public void Shutdown(bool wait = true)
        {
            lock(Lock)
            {
                _state = ThreadState.StopRequested;
            }

            ShutdownEvent.Set();
            ResumeEvent.Set();
            Thread.Join();
        }

        private void DoWork()
        {
            do
            {
                Suspend();

                if (!ShutdownEvent.WaitOne(0))
                    Context.DoWork();

            } while (!ShutdownEvent.WaitOne(0));

            lock (Lock)
            {
                _state = ThreadState.Stopped;
            }

        }

        private void Suspend()
        {
            lock (Lock)
            {
                _state = ThreadState.Suspended;
            }

            ResumeEvent.WaitOne();
        }
    }

    public interface IThreadContext
    {
        void DoWork();
    }

    public class MyThreadContext : IThreadContext
    {
        public void DoWork()
        {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString() + " starting...");
            Thread.Sleep(1234);
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString() + " ending...");
        }
    }
}
