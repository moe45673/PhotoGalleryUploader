using Windows.ApplicationModel.Background;

namespace PhotoGallery.Core.BackgroundTasks
{
    public interface IBackgroundTaskCallbacks
    {
        void AttachProgressAndCompletedHandlers(BackgroundTaskProgressEventHandler DelegateOnProgress, BackgroundTaskCompletedEventHandler DelegateOnComplete);
    }
}