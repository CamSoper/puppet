using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        private HashSet<AutomationTaskTokenType> _taskList;

        public AutomationTaskManager()
        {
            _taskList = new HashSet<AutomationTaskTokenType>();
        }

        /// <summary>
        /// Stores the task/cancellation token source.
        /// </summary>
        /// <param name="pair">The task and cancellation token source.</param>
        public void Track(Task work, CancellationTokenSource cts, Type automationType)
        {

            lock (_taskList)
            {
                _taskList.Add(new AutomationTaskTokenType(work, cts, automationType));
            }
        }

        /// <summary>
        /// Clears all completed tasks from the task list.
        /// </summary>
        public void RemoveCompletedTasks()
        {
            Task.Run(() =>
            {
                int count = 0;
                lock (_taskList)
                {
                    int countBefore = _taskList.Count;
                    _taskList.RemoveWhere(t => t.Task.IsCompleted);
                    int countAfter = _taskList.Count;
                    count = countBefore - countAfter;
                    if (count > 0)
                    {
                        Console.WriteLine($"{DateTime.Now} Removed {count} completed tasks. {_taskList.Count} tasks remain in progress.");
                    }
                }
            });
        }

        /// <summary>
        /// Cancels all existing automation tasks with the given type.
        /// </summary>
        /// <param name="automationType">The type of the automation to cancel.</param>
        public void CancelExistingTasks(Type automationType)
        {
            lock (_taskList)
            {
                foreach (var i in _taskList.Where(t => t.AutomationType == automationType && 
                    !t.Task.IsCompleted && !t.CTS.IsCancellationRequested))
                {
                    i.CTS.Cancel();
                }
            }
        }
    }
    class AutomationTaskTokenType
    {
        public Task Task { get; set; }
        public CancellationTokenSource CTS { get; set; }
        public Type AutomationType { get; set; }

        public AutomationTaskTokenType(Task task, CancellationTokenSource cts, Type automationType)
        {
            Task = task;
            CTS = cts;
            AutomationType = automationType;
        }
    }
}
