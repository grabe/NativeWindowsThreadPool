using System;
using System.Threading;

namespace ThreadPoolExample
{
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
