// <copyright file="AsyncHelpers.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Bedrock
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     Helper class to run async methods synchronously.
    /// </summary>
    public static class AsyncHelpers
    {
        /// <summary>
        /// Force asynchronous task to run synchronously.
        /// </summary>
        /// <param name="task"><see cref="Task"/> Task to run synchronously.</param>
        /// <param name="handler"><see cref="IFireAndForgetErrorHandler"/> Error handler for task.</param>
        public static async void FireAndForgetSafeAsync(this Task task, IFireAndForgetErrorHandler handler = null)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                handler?.FireAndForgetHandleError(ex);
            }
        }

        /// <summary>
        /// Synchronously executes an async Task method which has a void return value.
        /// </summary>
        /// <param name="task">The Task method to execute.</param>
        public static void RunSync(Func<Task> task)
        {
            var oldContext = SynchronizationContext.Current;
            var syncContext = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(syncContext);

            syncContext.Post(SendOrPostCallback, null);
            syncContext.BeginMessageLoop();
            SynchronizationContext.SetSynchronizationContext(oldContext);

            async void SendOrPostCallback(object state)
            {
                try
                {
                    await task();
                }
                catch (Exception e)
                {
                    syncContext.InnerException = e;
                    throw;
                }
                finally
                {
                    syncContext.EndMessageLoop();
                }
            }
        }

        /// <summary>
        /// Synchronously executes an async Task method which has a T return type.
        /// </summary>
        /// <typeparam name="T">Return Type.</typeparam>
        /// <param name="task">The Task method to execute.</param>
        /// <returns>The result of awaiting the given Task.</returns>
        public static T RunSync<T>(Func<Task<T>> task)
        {
            var oldContext = SynchronizationContext.Current;
            var syncContext = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(syncContext);

            T result = default;
            syncContext.Post(SendOrPostCallback, null);
            syncContext.BeginMessageLoop();
            SynchronizationContext.SetSynchronizationContext(oldContext);
            return result;

            async void SendOrPostCallback(object status)
            {
                try
                {
                    result = await task();
                }
                catch (Exception e)
                {
                    syncContext.InnerException = e;
                    throw;
                }
                finally
                {
                    syncContext.EndMessageLoop();
                }
            }
        }

        private class ExclusiveSynchronizationContext : SynchronizationContext
        {
            private readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);
            private readonly Queue<Tuple<SendOrPostCallback, object>> items = new Queue<Tuple<SendOrPostCallback, object>>();
            private bool done;

            public Exception InnerException { get; set; }

            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("We cannot send to our same thread");
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                lock (items)
                {
                    items.Enqueue(Tuple.Create(d, state));
                }

                workItemsWaiting.Set();
            }

            public void EndMessageLoop()
            {
                Post(_ => done = true, null);
            }

            public void BeginMessageLoop()
            {
                while (!done)
                {
                    Tuple<SendOrPostCallback, object> task = null;
                    lock (items)
                    {
                        if (items.Count > 0)
                        {
                            task = items.Dequeue();
                        }
                    }

                    if (task != null)
                    {
                        task.Item1(task.Item2);
                        if (InnerException != null) // the method threw an exception
                        {
                            throw new AggregateException("AsyncHelpers.Run method threw an exception.", InnerException);
                        }
                    }
                    else
                    {
                        workItemsWaiting.WaitOne();
                    }
                }
            }

            public override SynchronizationContext CreateCopy()
            {
                return this;
            }
        }
    }
}
