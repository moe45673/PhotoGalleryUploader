using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace PhotoGallery.Core.BackgroundTasks
{
    public interface IBackgroundTaskCallbacks
    {
        void AttachProgressAndCompletedHandlers(BackgroundTaskProgressEventHandler DelegateOnProgress, BackgroundTaskCompletedEventHandler DelegateOnComplete);
    }
}
