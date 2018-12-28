using System.Threading;

namespace ThreadPoolExample
{
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

            lock (Lock)
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
            lock (Lock)
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
}
