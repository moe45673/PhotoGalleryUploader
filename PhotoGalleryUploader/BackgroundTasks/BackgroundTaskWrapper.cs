using PhotoGallery.Core.BackgroundTasks;
using PhotoGallery.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;

namespace PhotoGalleryUploader.BackgroundTaskHelper
{
    #region AliasRegion
    using OnProgressHandlerArgs = Action<BackgroundTaskRegistration, BackgroundTaskProgressEventArgs>;
    using OnCompletedHandlerArgs = Action<BackgroundTaskRegistration, BackgroundTaskCompletedEventArgs>;
    #endregion

    public class BackgroundTaskFacade : BackgroundTaskHelperBase, INotifyPropertyChanged    {


        private IBackgroundTaskCallbacks callbackHandlers;



        #region Constructors
        public BackgroundTaskFacade(string taskEntryPoint, string name, IBackgroundTrigger trigger, IBackgroundCondition condition, 
            
            BackgroundTaskRegistrationGroup group = null, bool requiresBackgroundAccess = false, OnProgressHandlerArgs OnProgressMethod = null, OnCompletedHandlerArgs OnCompleteMethod = null) 
            : this(RegisterBackgroundTask(taskEntryPoint, name, trigger, condition, group, requiresBackgroundAccess), OnProgressMethod, OnCompleteMethod)
        { }

        public BackgroundTaskFacade(IBackgroundTaskRegistration task, OnProgressHandlerArgs OnProgressMethod = null, OnCompletedHandlerArgs OnCompleteMethod = null)
        {
            callbackHandlers = new BackgroundTaskCallbacks(task, OnProgressMethod, OnCompleteMethod);            
        }
        #endregion

        /// <summary>
        /// Wrapper method for composed callback handler
        /// </summary>
        /// <param name="OnProgressMethod"></param>
        /// <param name="OnCompletedMethod"></param>
        public void AttachProgressAndCompletedHandlers(OnProgressHandlerArgs OnProgressMethod, OnCompletedHandlerArgs OnCompletedMethod)
        {
            callbackHandlers.AttachProgressAndCompletedHandlers(new BackgroundTaskProgressEventHandler(OnProgressMethod), new BackgroundTaskCompletedEventHandler(OnCompletedMethod));
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }
        #endregion
        

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                //case nameof(callbacks.Progress):

                //    OnPropertyChanged(nameof(Progress));

                //break;       
                    

            }
        }
        
    }
}
