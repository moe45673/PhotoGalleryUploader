using System;
using Windows.ApplicationModel.Background;

namespace PhotoGallery.Core.BackgroundTasks
{
    public class BackgroundTaskCallbacks : IBackgroundTaskCallbacks
    {
        public BackgroundTaskCallbacks(IBackgroundTaskRegistration task, Action<BackgroundTaskRegistration, BackgroundTaskProgressEventArgs> OnProgressMethod = null, Action<BackgroundTaskRegistration, BackgroundTaskCompletedEventArgs> OnCompleteMethod = null)
        {
            TaskRegistration = task;
            if (OnProgressMethod != null && OnCompleteMethod != null)
            {
                AttachProgressAndCompletedHandlers(new BackgroundTaskProgressEventHandler(OnProgressMethod), new BackgroundTaskCompletedEventHandler(OnCompleteMethod));
            }
        }

        public void AttachProgressAndCompletedHandlers(BackgroundTaskProgressEventHandler DelegateOnProgress, BackgroundTaskCompletedEventHandler DelegateOnComplete)
        {
            TaskRegistration.Progress += DelegateOnProgress;
            TaskRegistration.Completed += DelegateOnComplete;
        }

        public IBackgroundTaskRegistration TaskRegistration { get; protected set; }
    }
}