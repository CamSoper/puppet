using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Puppet.Executive
{
    public class AutomationTaskManager
    {
        public HashSet<AutomationTaskTokenSourcePair> TaskList { get; }

        public AutomationTaskManager()
        {
            TaskList = new HashSet<AutomationTaskTokenSourcePair>();
        }

        /// <summary>
        /// Stores the task/cancellation token source.
        /// </summary>
        /// <param name="pair">The task and cancellation token source.</param>
        public void Track(AutomationTaskTokenSourcePair pair)
        {
            lock (TaskList)
            {
                TaskList.Add(pair);
            }
        }

        /// <summary>
        /// Clears all completed tasks from the task list.
        /// </summary>
        public void RemoveCompletedTasks()
        {
            int count = 0;
            lock (TaskList)
            {
                int countBefore = TaskList.Count;
                TaskList.RemoveWhere(t => t.Task.IsCompleted);
                int countAfter = TaskList.Count;
                count = countBefore - countAfter;
                if (count > 0)
                {
                    Console.WriteLine($"{DateTime.Now} Removed {count} completed tasks. {TaskList.Count} tasks remain in progress.");
                }
            }
        }

        /// <summary>
        /// Cancels all existing automation tasks with the given name.
        /// </summary>
        /// <param name="automationType">The type of the automation to cancel.</param>
        public void CancelAllTasks(Type automationType)
        {
            lock (TaskList)
            {
                foreach (var i in TaskList.Where(t => t.Task.AutomationType == automationType && 
                    !t.Task.IsCompleted && !t.CTS.IsCancellationRequested))
                {
                    i.CTS.Cancel();
                }
            }
        }
    }
}
