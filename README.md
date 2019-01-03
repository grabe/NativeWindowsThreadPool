# NativeWindowsThreadPool

This was created to provide an example to demonstrate the concept of a native OS thread pool, at the request of a Stackoverflow user. The instantiation of the Thread class creates a managed thread, and calling Start creates a native OS thread that the managed thread gets bound to. The native OS thread is dedicated to the managed thread. Because it is managed, all the .NET synchronization mechanisms can be used.

Creating a .NET managed thread (i.e using Task or ThreadPool, not using Thread class) are scheduled via simulated multitasking sharing execution time within a native OS thread. The involved .NET scheduling overhead is generally negligible given the overhead associated with privilege execution (i.e. kernel/user mode transition, native OS thread context switching). Microsoft did an excellent job in this regard. For 99% of applications, there should be little reason to use native OS threads. There are two very good reasons to use .NET concurrency: ease of use and portability. Using .NET managed concurrency is the way to go.

Motivation for this concept (native OS thread pool) came from a need for more direct control over concurrency using Windows preemptive scheduler and so the threads are on the same playing field as the rest of the operating system. Also, on multi-core CPUs with hyper threading, the overhead associated with context switching is negligible. 

Also, my tests revealed that in comparing using .NET managed threads exclusively, vs. native OS threads, the disparity between user mode and kernel mode execution isn't discernable. The reason for this is that simulated multitasking with .NET managed threads only avoids a very small percentage of kernel mode context switching. If a .NET app has 5 threads and so 5 context switches are avoided, compare those 5 to hundreds or thousands of threads listed in Task Manager details. 

Moreover, the .NET app has to have at least 1 native OS thread. Create a console app and have it do nothing but sleep for a minute then look at Task Manager and you’ll see 5-8 threads. Thus, how much performance is really saved with avoiding a few context switches? After all, a quad core CPU can only execute 4 threads at a time so context switching would occur in the console app you just created even if there were no other threads running on thesystem.

Anyone may use this code in whole or in part as they deem fit.
