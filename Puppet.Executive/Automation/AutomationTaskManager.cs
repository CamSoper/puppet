using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Puppet.Common.Automation;
using Puppet.Common.Telemetry;

namespace Puppet.Executive.Automation
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
        private TelemetryClient _telemetryClient;
        private Metric _taskCountMetric;

        public AutomationTaskManager(IConfiguration configuration)
        {
            _telemetryClient = AppInsights.GetTelemetryClient(configuration);
            _taskCountMetric = _telemetryClient.GetMetric("AutomationTaskCount");
            _taskList = new HashSet<AutomationTaskTokenType>();
        }

        /// <summary>
        /// Stores the task/cancellation token source.
        /// </summary>
        /// <param name="pair">The task and cancellation token source.</param>
        public void Track(Task work, CancellationTokenSource cts, Type automationType, string initiatingDeviceId)
        {

            lock (_taskList)
            {
                _taskList.Add(new AutomationTaskTokenType(work, cts, automationType, initiatingDeviceId));
                _taskCountMetric.TrackValue(_taskList.Count);
                Console.WriteLine($"{DateTime.Now} Tracking {_taskList.Count} tasks.");
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
                        _taskCountMetric.TrackValue(_taskList.Count);
                        Console.WriteLine($"{DateTime.Now} Removed {count} completed tasks. {_taskList.Count} tasks remain in progress.");
                    }
                }
            });
        }

        /// <summary>
        /// Cancels all existing automation tasks with the given type and, if applicable, intiating device.
        /// </summary>
        /// <param name="automationType">The type of the automation to cancel.</param>
        public void CancelRunningInstances(Type automationType, string initiatingDeviceId)
        {

            bool perDevice = (automationType.GetCustomAttributes<RunPerDeviceAttribute>()?.Count() > 0) ? true : false;
            Func<AutomationTaskTokenType, bool> filterAction;
            if (perDevice)
            {
                filterAction = (t) =>
                {
                    return t.AutomationType == automationType
                            && t.InitiatingDeviceId == initiatingDeviceId
                            && !t.Task.IsCompleted
                            && !t.CTS.IsCancellationRequested;
                };
            }
            else
            {
                filterAction = (t) =>
                {
                    return t.AutomationType == automationType
                            && !t.Task.IsCompleted
                            && !t.CTS.IsCancellationRequested;
                };
            }

            lock (_taskList)
            {
                foreach (var i in _taskList.Where(filterAction))
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
        public string InitiatingDeviceId { get; set; }

        public AutomationTaskTokenType(Task task, CancellationTokenSource cts, Type automationType, string initiatingDeviceId)
        {
            Task = task;
            CTS = cts;
            AutomationType = automationType;
            InitiatingDeviceId = initiatingDeviceId;
        }
    }
}
