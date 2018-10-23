using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Core;
using Windows.UI.Xaml;
using PhotoGalleryUploader.Core.MVVM;
using Windows.System.Threading;

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
