using System;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace UwCore.Helpers
{
    public static class CaliburnMicroHelper
    {
        public static bool TryGuardClose(object obj)
        {
            var guard = obj as IGuardClose;

            if (guard != null)
            {
                var shouldCancel = false;
                var runningAsync = true;

                guard.CanClose(result =>
                {
                    runningAsync = false;
                    shouldCancel = !result;
                });

                if (runningAsync)
                    throw new NotSupportedException("Async CanClose is not supported.");

                return shouldCancel;
            }

            return false;
        }

        public static Task<bool> TryGuardCloseAsync(object obj)
        {
            TaskCompletionSource<bool> task = new TaskCompletionSource<bool>();

            var guard = obj as IGuardClose;
            if (guard != null)
            {
                guard.CanClose(result =>
                {
                    task.SetResult(!result);
                });
                
                return task.Task;
            }

            return Task.FromResult(false);
        }
    }
}