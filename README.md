# NativeWindowsThreadPool

This was created to provide an example to demonstrate the concept of a native OS thread pool, at the request of a Stackoverflow user.
The instantiation of the Thread class creates a managed thread, and calling Start creates a native OS thread that the managed thread gets bound to. The native OS thread is dedicated to the managed thread. Because it is managed, all the .NET synchronization mechanisms can 
be used.

.NET managed threads are scheduled via simulated multitasking within a native OS thread. The involved .NET scheduling overhead 
is generally negligible given the overhead associated with privilege execution (i.e. kernel/user mode transition, native OS thread 
context switching). Microsoft did an excellent job in this regard.

Motivation for this came from a need for more direct control over concurrency using Windows preemptive scheduler. 

Anyone may use the code in whole or in part as they deem fit.
