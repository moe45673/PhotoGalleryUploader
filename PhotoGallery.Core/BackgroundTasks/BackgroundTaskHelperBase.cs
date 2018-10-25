using System.Collections.Generic;
using Windows.ApplicationModel.Background;
using Windows.Storage;

namespace PhotoGallery.Core.Helpers
{
    public abstract class BackgroundTaskHelperBase
    {
        #region quick and dirty TaskGroup

        protected class TaskDictionary : Dictionary<string, TaskMetaData>
        {
            /// <summary>
            /// Hides base Add() method.
            /// </summary>
            /// <param name="key">Always equal to the Taskname field from <paramref name="value"/></param>
            /// <param name="value"></param>
            protected new void Add(string key, TaskMetaData value)
            {
                Add(value);
            }

            protected virtual void Add(TaskMetaData value)
            {
                Add(value.TaskName, value.TaskProgress, value.IsRegistered, value.RequiresBackgroundAccess);
            }

            public virtual void Add(string taskName, string taskProgress = "", bool isRegistered = false, bool requiresBackgroundAccess = false)
            {
                if (
                    !Tasks.ContainsKey(taskName)
                    )
                {
                    base.Add(taskName, new TaskMetaData(taskName, taskProgress, isRegistered, requiresBackgroundAccess));
                }
            }
        }

        protected struct TaskMetaData
        {
            public string TaskName { get; }
            public string TaskProgress { get; set; }
            public bool IsRegistered { get; set; }
            public bool RequiresBackgroundAccess { get; set; }

            public TaskMetaData(string taskName, string taskProgress = "", bool isRegistered = false, bool requiresBackgroundAccess = false)
            {
                TaskName = taskName;
                TaskProgress = taskProgress;
                IsRegistered = isRegistered;
                RequiresBackgroundAccess = requiresBackgroundAccess;
            }
        }

        protected static TaskDictionary _tasks;

        protected static TaskDictionary Tasks
        {
            get => _tasks;
            set => _tasks = value;
        }

        #endregion quick and dirty TaskGroup

        static BackgroundTaskHelperBase()
        {
            _tasks = new TaskDictionary();
        }

        /// <summary>
        /// Register a background task with the specified taskEntryPoint, name, trigger,
        /// and condition (optional).
        /// </summary>
        /// <param name="taskEntryPoint">Task entry point for the background task.</param>
        /// <param name="name">A name for the background task.</param>
        /// <param name="trigger">The trigger for the background task.</param>
        /// <param name="condition">An optional conditional event that must be true for the task to fire.</param>
        public static BackgroundTaskRegistration RegisterBackgroundTask(string taskEntryPoint, string name, IBackgroundTrigger trigger, IBackgroundCondition condition, BackgroundTaskRegistrationGroup group = null, bool requiresBackgroundAccess = false)
        {
            if (TaskRequiresBackgroundAccess(name))
            {
                // If the user denies access, the task will not run.
                var requestTask = BackgroundExecutionManager.RequestAccessAsync();
            }

            var builder = new BackgroundTaskBuilder();

            builder.Name = name;
            builder.TaskEntryPoint = taskEntryPoint;
            builder.SetTrigger(trigger);

            if (condition != null)
            {
                builder.AddCondition(condition);

                //
                // If the condition changes while the background task is executing then it will
                // be canceled.
                //
                builder.CancelOnConditionLoss = true;
            }

            if (group != null)
            {
                builder.TaskGroup = group;
            }

            BackgroundTaskRegistration task = builder.Register();
            Tasks.Add(task.Name, requiresBackgroundAccess: requiresBackgroundAccess);

            UpdateBackgroundTaskRegistrationStatus(name, true);

            //
            // Remove previous completion status.
            //
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values.Remove(name);

            return task;
        }

        /// <summary>
        /// Unregister background tasks with specified name.
        /// </summary>
        /// <param name="name">Name of the background task to unregister.</param>
        public static void UnregisterBackgroundTasks(string name, BackgroundTaskRegistrationGroup group = null)
        {
            //
            // If the given task group is registered then loop through all background tasks associated with it
            // and unregister any with the given name.
            //
            if (group != null)
            {
                foreach (var cur in group.AllTasks)
                {
                    if (cur.Value.Name == name)
                    {
                        cur.Value.Unregister(true);
                    }
                }
            }
            else
            {
                //
                // Loop through all ungrouped background tasks and unregister any with the given name.
                //
                foreach (var cur in BackgroundTaskRegistration.AllTasks)
                {
                    if (cur.Value.Name == name)
                    {
                        cur.Value.Unregister(true);
                    }
                }
            }

            UpdateBackgroundTaskRegistrationStatus(name, false);
        }

        /// <summary>
        /// Retrieve a registered background task group. If no group is registered with the given id,
        /// then create a new one and return it.
        /// </summary>
        /// <returns>The task group associated with the given id</returns>
        public static BackgroundTaskRegistrationGroup GetTaskGroup(string id, string groupName)
        {
            var group = BackgroundTaskRegistration.GetTaskGroup(id);

            if (group == null)
            {
                group = new BackgroundTaskRegistrationGroup(id, groupName);
            }

            return group;
        }

        /// <summary>
        /// Store the registration status of a background task with a given name.
        /// </summary>
        /// <param name="name">Name of background task to store registration status for.</param>
        /// <param name="registered">TRUE if registered, FALSE if unregistered.</param>
        public static void UpdateBackgroundTaskRegistrationStatus(string name, bool registered)
        {
            var task = Tasks[name];
            task.IsRegistered = registered;
        }

        /// <summary>
        /// Get the registration / completion status of the background task with
        /// given name.
        /// </summary>
        /// <param name="name">Name of background task to retreive registration status.</param>
        public static string GetBackgroundTaskStatus(string name)
        {
            var registered = false;
            var task = Tasks[name];
            registered = task.IsRegistered;

            var status = registered ? "Registered" : "Unregistered";

            object taskStatus;
            var settings = ApplicationData.Current.LocalSettings;
            if (settings.Values.TryGetValue(name, out taskStatus))
            {
                status += " - " + taskStatus.ToString();
            }

            return status;
        }

        /// <summary>
        /// Determine if task with given name requires background access.
        /// </summary>
        /// <param name="name">Name of background task to query background access requirement.</param>
        public static bool TaskRequiresBackgroundAccess(string name)
        {
            return Tasks[name].RequiresBackgroundAccess;
        }
    }
}