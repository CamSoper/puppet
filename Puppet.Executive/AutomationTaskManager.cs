using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Puppet.Executive
{
    /// <summary>
    /// This class maintains a collection of AutomationTasks and their
    /// CancellationTokenSources. This is so long-running tasks are kept 
    /// in scope until they're done running, along with their cancellation
    /// tokens, so they can be cancelled if another AutomationTask running 
    /// the same IAutomation comes along.
    /// </summary>
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
        /// Cancels all existing automation tasks with the given type.
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
