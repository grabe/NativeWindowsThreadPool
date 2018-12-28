# NativeWindowsThreadPool

This was created to provide an example to demonstrate the concept of a native OS thread pool, at the request of a Stackoverflow user.

Anyone may use the code in whole or in part as they deem fit.

.NET managed threads are scheduled via simulated multitasking within a native OS thread. The involved .NET scheduling overhead 
is generally negligable given the overhead associated with privilege execution (i.e. kernel/user mode transition, native OS thread 
context switching). Microsoft did an excellent job in this regard.

Motivation for this came from a need for more direct control over concurrency using Windows preemptive scheduler. 

