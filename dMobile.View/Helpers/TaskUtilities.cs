namespace dMobile.View.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }

    public static class TaskUtilities
    {
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
