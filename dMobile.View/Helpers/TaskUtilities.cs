// <copyright file="TaskUtilities.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.View.Helpers
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Task utilities.
    /// </summary>
    public static class TaskUtilities
    {
        /// <summary>
        /// Force asynchronous task to run synchronously.
        /// </summary>
        /// <param name="task"><see cref="Task"/> Task to run synchronously.</param>
        /// <param name="handler"><see cref="IErrorHandler"/> Error handler for task.</param>
        public static async void FireAndForgetSafeAsync(this Task task, IErrorHandler handler = null)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                handler?.HandleError(ex);
            }
        }
    }
}