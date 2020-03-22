using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using static Gzip.Test.PlatformDependentDataConfiguration;
using ThreadState = System.Threading.ThreadState;

namespace Gzip.Test
{
    internal static class ThreadPool
    {
        private static readonly List<WaitHandle> WaitHandles = new List<WaitHandle>(capacity: CoresCount);
        private static readonly List<Task> Tasks = new List<Task>(capacity: CoresCount);

        public static Task SyncedTask(Action cb)
        {
            var syncEvent = new AutoResetEvent(false);
            WaitHandles.Add(syncEvent);
            
            var task = new Task(new Thread(() =>
                {
                    cb();
                    syncEvent.Set();
                }
            ));

            Tasks.Add(task);

            return task;

        }

        public static Task SingleTask(Action fn)
        {
            var threadFuncType = typeof(ThreadStart);
            var methodInfo = fn.GetMethodInfo();
            var target = fn.Target;
            var threadStart = Delegate.CreateDelegate(threadFuncType, target, methodInfo) as ThreadStart;

            return new Task(new Thread(threadStart));
            
        }

        internal static void AwaitAll()
        {
            foreach (var task in Tasks.Where(task => task.State == Task.TaskState.New))
            {
                task.Run();
            }
            WaitHandle.WaitAll(WaitHandles.ToArray());
            Tasks.Clear();
            WaitHandles.ForEach(sync => sync.Dispose());
            WaitHandles.Clear();
        }
        
        internal class Task
        {
            internal TaskState State => MapToState(_worker.ThreadState);
            private Thread _worker;
            internal Task(Thread thread)
            {
                _worker = thread;
            }

            internal void Run() => _worker.Start();

            internal enum TaskState : byte
            {
                Default = 0,
                New = 1,
                Running = 2, 
                Finished = 3,
            }

            private TaskState MapToState(ThreadState threadState) =>
                threadState switch
                {
                    ThreadState.Unstarted => TaskState.New,
                    ThreadState.Running => TaskState.Running,
                    ThreadState.Stopped => TaskState.Finished,
                    _ => TaskState.Default
                };
            
        }
    }
}